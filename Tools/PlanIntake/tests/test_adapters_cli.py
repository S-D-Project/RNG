import contextlib
import io
import json
from pathlib import Path
from types import SimpleNamespace
import tempfile
from unittest import TestCase
from unittest.mock import patch

from support import PLAN, REPO, StateCase, raw_issue
from rng_intake.adapters import AoRead, GitHubRead, Inventory, ReadFailure
from rng_intake.cli import main, parser
from rng_intake.collector import evaluate
from rng_intake.errors import IntakeError, SafetyBlock


def response(values, headers=None, status=200):
    lines = [f"HTTP/2.0 {status} OK"]
    lines += [f"{k}: {v}" for k, v in (headers or {}).items()]
    return SimpleNamespace(stdout="\r\n".join(lines) + "\r\n\r\n" + json.dumps(values),
                           returncode=0 if status == 200 else 1, stderr="")


class AdapterTests(TestCase):
    def test_all_pages_use_get_and_prs_are_excluded_before_classification(self):
        calls = []
        def runner(args):
            calls.append(args)
            if len(calls) == 1:
                values = [raw_issue(n, plan_id=f"plan-20261111-{n:08x}") for n in range(1, 101)]
                return response(values, {"Link": f'<https://api.github.com/repos/{REPO}/issues?page=2>; rel="next"',
                                         "X-Poll-Interval": "90"})
            return response([raw_issue(101, plan_id="plan-20261111-00000001"),
                             dict(raw_issue(102), pull_request={})])
        inventory = GitHubRead(REPO, runner).list_issues()
        self.assertEqual((inventory.pages, len(inventory.issues), inventory.poll_interval), (2, 102, 90))
        results = evaluate(inventory.issues, REPO)
        self.assertEqual(len(results), 101)
        self.assertEqual(results[0]["classification"], "duplicate_plan_id")
        self.assertEqual(results[-1]["classification"], "duplicate_plan_id")
        self.assertTrue(all(args[args.index("--method") + 1] == "GET" for args in calls))
        self.assertIn("state=all", calls[0][-1])
        self.assertIn("sort=created", calls[0][-1])

    def test_partial_page_failure_returns_no_complete_inventory(self):
        count = 0
        def runner(args):
            nonlocal count
            count += 1
            if count == 1:
                return response([raw_issue()], {"Link": f'<https://api.github.com/repos/{REPO}/issues?page=2>; rel="next"'})
            return response({"message": "service unavailable"}, status=503)
        with self.assertRaises(ReadFailure):
            GitHubRead(REPO, runner).list_issues()
        self.assertEqual(count, 2)

    def test_unexpected_pagination_host_and_cycles_are_rejected(self):
        for target in ("https://example.com/steal", f"https://api.github.com/repos/{REPO}/pulls"):
            with self.subTest(target=target), self.assertRaises(ReadFailure):
                GitHubRead(REPO, lambda _: response([], {"Link": f'<{target}>; rel="next"'})).list_issues()
        endpoint = f"https://api.github.com/repos/{REPO}/issues?state=all&sort=created&direction=asc&per_page=100"
        with self.assertRaises(ReadFailure):
            GitHubRead(REPO, lambda _: response([], {"Link": f'<{endpoint}>; rel="next"'})).list_issues()

    def test_rate_limit_is_not_treated_as_empty_success(self):
        read = GitHubRead(REPO, lambda _: response({"message": "rate limited"}, {"Retry-After": "120"}, 429))
        with self.assertRaises(ReadFailure) as caught:
            read.list_issues()
        self.assertGreaterEqual(caught.exception.delay, 120)

    def test_orchestrator_is_discovered_and_scoped_to_project_and_role(self):
        data = [{"id": "changed-session-id", "projectId": "rng", "role": "orchestrator", "status": "working"},
                {"id": "other", "projectId": "rng", "role": "worker", "status": "working"}]
        calls = []
        def runner(args):
            calls.append(args)
            return SimpleNamespace(stdout=json.dumps({"data": data}), returncode=0)
        adapter = AoRead("rng", runner)
        self.assertEqual(adapter.orchestrator(), "changed-session-id")
        self.assertIn("--include-terminated", calls[0])
        with self.assertRaises(SafetyBlock):
            adapter.orchestrator("other")
        data.append({"id": "second", "projectId": "rng", "role": "orchestrator", "status": "working"})
        with self.assertRaises(SafetyBlock):
            adapter.orchestrator()

    def test_installed_backend_cannot_enable_external_effects(self):
        adapter = AoRead("rng", lambda _: self.fail("no external command allowed"))
        self.assertFalse(adapter.capabilities.conditional_claim)
        self.assertFalse(adapter.capabilities.correlated_spawn)
        for action in (adapter.claim, adapter.send, adapter.spawn, adapter.lookup, adapter.delivery_receipts):
            with self.assertRaises(SafetyBlock):
                action()


class CliTests(StateCase):
    def test_stateless_dry_run_does_not_touch_ledger_or_call_any_write(self):
        snapshot = self.root / "issues.json"
        snapshot.write_text(json.dumps([raw_issue()]), encoding="utf-8")
        before = (self.directory / "state.sqlite3").read_bytes()
        output = io.StringIO()
        with contextlib.redirect_stdout(output), patch("rng_intake.cli.AoRead") as ao:
            code = main(["--repo", REPO, "--workspace", str(self.workspace),
                         "--state-dir", str(self.root / "must-not-exist"), "inspect", "--input", str(snapshot),
                         "--skip-ao"])
            ao.assert_not_called()
        value = json.loads(output.getvalue())
        self.assertEqual(code, 0)
        self.assertFalse(value["automatic_intake"])
        self.assertFalse(value["external_writes"])
        self.assertFalse((self.root / "must-not-exist").exists())
        self.assertEqual((self.directory / "state.sqlite3").read_bytes(), before)

    def test_partial_fetch_failure_persists_error_without_advancing_checkpoint(self):
        output = io.StringIO()
        with patch("rng_intake.cli.binding_root", return_value=self.bindings), \
             patch("rng_intake.cli.worktree_roots", return_value=[self.workspace]), \
             patch("rng_intake.cli.GitHubRead.list_issues", side_effect=ReadFailure("page 2 failed")), \
             contextlib.redirect_stdout(output):
            code = main(["--repo", REPO, "--workspace", str(self.workspace),
                         "--state-dir", str(self.directory), "poll"])
        self.assertEqual(code, 2)
        self.assertEqual(self.store.db.execute("SELECT count(*) FROM scans").fetchone()[0], 0)
        self.assertIn("page 2 failed", output.getvalue())
        self.assertGreater(self.store.db.execute("SELECT count(*) FROM events").fetchone()[0], 0)

    def test_single_observation_cycle_never_creates_intake_or_assignment(self):
        output = io.StringIO()
        inventory = Inventory([raw_issue()], "2026-11-11T14:00:00Z", 1, 60)
        with patch("rng_intake.cli.binding_root", return_value=self.bindings), \
             patch("rng_intake.cli.worktree_roots", return_value=[self.workspace]), \
             patch("rng_intake.cli.GitHubRead.list_issues", return_value=inventory), \
             patch("rng_intake.cli.AoRead.orchestrator", return_value="existing-orchestrator"), \
             contextlib.redirect_stdout(output):
            code = main(["--repo", REPO, "--workspace", str(self.workspace), "--project", "rng",
                         "--state-dir", str(self.directory), "poll"])
        self.assertEqual(code, 0)
        self.assertEqual(self.store.db.execute("SELECT count(*) FROM scans").fetchone()[0], 1)
        self.assertEqual(self.store.db.execute("SELECT count(*) FROM intakes").fetchone()[0], 0)
        self.assertEqual(self.store.db.execute("SELECT count(*) FROM assignments").fetchone()[0], 0)
        self.assertFalse(json.loads(output.getvalue())["automatic_intake"])

    def test_restart_honors_durable_rate_limit_deadline_before_any_read(self):
        from datetime import datetime, timedelta, timezone
        deadline = (datetime.now(timezone.utc) + timedelta(minutes=10)).isoformat()
        with self.store.write("read_failure", {"retry_at": deadline}):
            pass
        output = io.StringIO()
        with patch("rng_intake.cli.binding_root", return_value=self.bindings), \
             patch("rng_intake.cli.worktree_roots", return_value=[self.workspace]), \
             patch("rng_intake.cli.GitHubRead.list_issues") as fetch, \
             contextlib.redirect_stdout(output):
            code = main(["--repo", REPO, "--workspace", str(self.workspace),
                         "--state-dir", str(self.directory), "poll"])
            fetch.assert_not_called()
        self.assertEqual(code, 2)
        self.assertIn("durable read backoff active", output.getvalue())

    def test_cli_has_no_approval_override_or_auto_intake_switch(self):
        with contextlib.redirect_stderr(io.StringIO()), self.assertRaises(SystemExit):
            parser().parse_args(["--approve", "poll"])

    def test_malformed_local_plan_blocks_a_complete_intake_inventory(self):
        target = self.workspace / "Docs" / "Plans"
        target.mkdir(parents=True)
        (target / f"{PLAN}-broken.md").write_text("id: missing front matter", encoding="utf-8")
        snapshot = self.root / "issues.json"
        snapshot.write_text(json.dumps([raw_issue()]), encoding="utf-8")
        output = io.StringIO()
        with contextlib.redirect_stdout(output):
            code = main(["--repo", REPO, "--workspace", str(self.workspace), "inspect", "--input", str(snapshot)])
        self.assertEqual(code, 2)
        self.assertIn("local PLAN inventory incomplete", output.getvalue())
