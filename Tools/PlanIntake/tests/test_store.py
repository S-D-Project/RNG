import json
from pathlib import Path
import sqlite3
import subprocess
import sys
from unittest.mock import patch

from support import PLAN, REPO, StateCase, raw_issue
from rng_intake.collector import evaluate
from rng_intake.errors import RecoveryRequired, SafetyBlock
from rng_intake.store import FileLock, Store


class StoreTests(StateCase):
    def test_state_inside_any_worktree_is_rejected(self):
        with self.assertRaises(SafetyBlock):
            Store.initialize(self.workspace / "state", self.bindings, "other/repo", [self.workspace])

    def test_existing_binding_cannot_silently_switch_to_a_new_path(self):
        with self.assertRaises(RecoveryRequired):
            Store.initialize(self.root / "new-state", self.bindings, REPO, [self.workspace])
        with self.assertRaises(RecoveryRequired):
            Store(self.root / "new-state", self.bindings, REPO, [self.workspace])

    def test_missing_database_is_not_recreated(self):
        self.store.close()
        path = self.directory / "state.sqlite3"
        path.unlink()
        with self.assertRaises(RecoveryRequired):
            Store(self.directory, self.bindings, REPO, [self.workspace])
        self.assertFalse(path.exists())

    def test_past_database_restore_is_detected_using_independent_binding(self):
        self.store.close()
        path = self.directory / "state.sqlite3"
        old = path.read_bytes()
        self.store = Store(self.directory, self.bindings, REPO, [self.workspace])
        self.scan()
        self.store.close()
        path.write_bytes(old)
        with self.assertRaises(RecoveryRequired):
            Store(self.directory, self.bindings, REPO, [self.workspace])

    def test_committed_sql_with_missing_sidecar_ack_recovers_chain_prefix(self):
        with patch.object(self.store, "_anchor", side_effect=OSError("ack interrupted")):
            with self.assertRaises(OSError):
                self.scan()
        self.reopen()
        self.assertEqual(self.store.db.execute("SELECT count(*) FROM scans").fetchone()[0], 1)
        anchor = json.loads(self.store.binding.read_text())
        self.assertEqual(anchor["revision"], 1)

    def test_failed_sql_commit_does_not_advance_scan_checkpoint(self):
        self.store.db.execute("PRAGMA query_only=ON")
        with self.assertRaises(sqlite3.Error):
            self.scan()
        self.assertEqual(self.store.db.execute("SELECT count(*) FROM scans").fetchone()[0], 0)

    def test_issue_id_change_is_repair_not_new_plan(self):
        self.scan()
        rows = evaluate([raw_issue(plan_id="plan-20261111-aabbccdd")], REPO, store=self.store)
        self.assertEqual(rows[0]["classification"], "identity_changed")
        self.store.record_scan(rows, "2026-11-11T14:01:00Z")
        row = self.store.db.execute("SELECT * FROM observations").fetchone()
        self.assertEqual(row["first_plan_id"], PLAN)

    def test_partial_scan_transaction_rolls_back_all_observations(self):
        rows = [{"issue": 1, "source": "same", "body_hash": "x", "classification": "metadata_invalid"},
                {"issue": 2, "source": "same", "body_hash": "y", "classification": "metadata_invalid"}]
        with self.assertRaises(sqlite3.IntegrityError):
            self.store.record_scan(rows, "2026-11-11T14:00:00Z")
        self.assertEqual(self.store.db.execute("SELECT count(*) FROM scans").fetchone()[0], 0)
        self.assertEqual(self.store.db.execute("SELECT count(*) FROM observations").fetchone()[0], 0)

    def test_kernel_lock_excludes_another_process_and_is_released(self):
        package = str(Path(__file__).resolve().parents[1])
        lock = self.store.binding.with_suffix(".collector.lock")
        script = ("import sys;sys.path.insert(0,sys.argv[1]);"
                  "from rng_intake.store import FileLock;"
                  "from pathlib import Path;"
                  "with_lock=FileLock(Path(sys.argv[2]));"
                  "with_lock.__enter__();with_lock.__exit__()")
        with FileLock(lock):
            child = subprocess.run([sys.executable, "-B", "-c", script, package, str(lock)],
                                   capture_output=True, timeout=10)
            self.assertNotEqual(child.returncode, 0)
            self.assertIn(b"AlreadyRunning", child.stderr)
        child = subprocess.run([sys.executable, "-B", "-c", script, package, str(lock)],
                               capture_output=True, timeout=10)
        self.assertEqual(child.returncode, 0, child.stderr)
