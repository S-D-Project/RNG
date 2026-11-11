from dataclasses import replace
import threading
from unittest.mock import patch

from support import FakeBackend, PLAN, REPO, SHA, StateCase, issue, raw_issue
from rng_intake.collector import evaluate
from rng_intake.errors import AlreadyRunning, InvalidPlan, RecoveryRequired, SafetyBlock
from rng_intake.gate import Capabilities, Gate
from rng_intake.metadata import NewEvidence, ResumeEvidence, parse
from rng_intake.store import Store


class GateTests(StateCase):
    def test_ready_accepted_once_and_same_logical_task_spawns_once(self):
        record = self.accepted()
        self.assertEqual(record["phase"], "active")
        self.gate.handoff(PLAN, "existing-orchestrator")
        self.gate.handoff(PLAN, "existing-orchestrator")
        first = self.gate.dispatch(PLAN, "parser")
        second = self.gate.dispatch(PLAN, "parser")
        self.assertEqual(first["id"], second["id"])
        self.assertEqual((self.remote.claims, self.remote.sends, self.remote.spawns), (1, 1, 1))
        self.assertEqual(first["phase"], "running")
        self.assertTrue(first["branch"].startswith(f"feature/{PLAN}-parser-"))
        self.assertEqual(self.store.db.execute("SELECT base_sha FROM assignments").fetchone()[0], SHA)

    def test_other_states_never_call_claim_or_spawn(self):
        for status in ("proposed", "active", "on hold", "completed", "rejected"):
            current = issue(status=status)
            self.remote = FakeBackend(current, self.root / "worker")
            self.gate = Gate(self.store, self.remote, "rng")
            self.scan([raw_issue(status=status)])
            with self.assertRaises(SafetyBlock):
                self.gate.accept(current, "owner")
            self.assertEqual((self.remote.claims, self.remote.spawns), (0, 0))

    def test_unproven_live_capability_is_blocked_before_any_intent(self):
        self.remote.capabilities = Capabilities()
        self.scan()
        with self.assertRaisesRegex(SafetyBlock, "conditional_claim"):
            self.gate.accept(issue(), "owner")
        self.assertIsNone(self.store.intake(PLAN))
        self.assertEqual(self.remote.claims, 0)

    def test_full_scan_required_before_accept(self):
        with self.assertRaisesRegex(SafetyBlock, "complete current scan"):
            self.gate.accept(issue(), "owner")
        self.assertEqual(self.remote.claims, 0)

    def test_duplicate_ids_in_different_issues_never_call_claim(self):
        self.scan([raw_issue(1), raw_issue(2)])
        with self.assertRaises(SafetyBlock):
            self.gate.accept(issue(), "owner")
        self.assertEqual(self.remote.claims, 0)

    def test_invalid_duplicate_identity_is_indexed_and_blocks_acceptance(self):
        completed = raw_issue(2, status="completed")
        variants = {
            "unmigrated_source": raw_issue(2, status="completed", source="Docs/Plans/original.md"),
            "missing_task": dict(completed, body=completed["body"].replace("### 작업 내용", "### 설명")),
            "missing_step": dict(completed, body=completed["body"].replace("current_step: assignment pending\n", "")),
            "nonstring_source": dict(completed, body=completed["body"].replace(f'source: "{completed["html_url"]}"',
                                                                                 "source: false")),
        }
        for name, duplicate in variants.items():
            with self.subTest(invalid_field=name):
                observations = self.scan([raw_issue(), duplicate])
                self.assertEqual(observations[1]["classification"], "metadata_invalid")
                self.assertEqual(observations[1].get("plan_id"), PLAN)
                self.assertEqual(observations[0]["classification"], "duplicate_plan_id")
                count = self.store.db.execute("SELECT count(*) FROM observations WHERE plan_id=?", (PLAN,)).fetchone()[0]
                self.assertEqual(count, 2)
                with self.assertRaises(SafetyBlock):
                    self.gate.accept(issue(), "owner")
                self.assertEqual(self.remote.claims, 0)

    def test_nested_completion_edit_cannot_reach_dispatch(self):
        current = issue()
        current = replace(current, body=current.body.replace("### 완료 조건",
                                                            "### PLAN progress\n\n#### 완료 조건"))
        backend = None
        with self.assertRaises((InvalidPlan, SafetyBlock)):
            backend = FakeBackend(current, self.root / "worker")
            gate = Gate(self.store, backend, "rng")
            self.scan([dict(raw_issue(), body=current.body)])
            gate.accept(current, "owner")
            changed = replace(backend.controlled.issue,
                              body=backend.controlled.issue.body.replace("focused tests pass",
                                                                         "changed completion criteria"))
            backend.controlled = replace(backend.controlled, issue=changed, plan=parse(changed))
            gate.dispatch(PLAN, "parser")
        self.assertEqual(self.store.db.execute("SELECT count(*) FROM attempts").fetchone()[0], 0)
        if backend is not None:
            self.assertEqual(backend.spawns, 0)

    def test_changed_source_between_scan_and_claim_preserves_user_edit(self):
        self.scan()
        original = issue()
        self.remote.edit(task="user edit")
        with self.assertRaisesRegex(SafetyBlock, "source changed"):
            self.gate.accept(original, "owner")
        self.assertEqual(self.remote.claims, 0)
        self.assertIn("user edit", self.remote.controlled.issue.body)

    def test_concurrent_edit_at_conditional_claim_is_not_overwritten(self):
        self.scan()
        self.remote.edit_during_claim = True
        with self.assertRaises(RecoveryRequired):
            self.gate.accept(issue(), "owner")
        self.assertIn("user concurrent edit", self.remote.controlled.issue.body)
        self.assertEqual(self.store.intake(PLAN)["phase"], "uncertain")
        self.assertEqual(self.remote.spawns, 0)

    def test_claim_failure_never_allows_spawn(self):
        self.scan()
        self.remote.fail_claim = True
        with self.assertRaises(RecoveryRequired):
            self.gate.accept(issue(), "owner")
        with self.assertRaises(SafetyBlock):
            self.gate.dispatch(PLAN, "parser")
        self.assertEqual(self.remote.spawns, 0)

    def test_active_transition_then_interruption_recovers_same_intake(self):
        self.scan()
        self.remote.lose_claim = True
        with self.assertRaises(RecoveryRequired):
            self.gate.accept(issue(), "owner")
        token = self.store.intake(PLAN)["token"]
        self.reopen()
        self.gate.reconcile_intake(PLAN)
        attempt = self.gate.dispatch(PLAN, "parser")
        self.assertEqual(self.store.intake(PLAN)["token"], token)
        self.assertEqual(self.remote.claims, 1)
        self.assertEqual(attempt["phase"], "running")

    def test_active_unknown_or_existing_plan_is_not_auto_adopted(self):
        self.remote = FakeBackend(issue(status="active"), self.root / "worker")
        self.gate = Gate(self.store, self.remote, "rng")
        with self.assertRaises(SafetyBlock):
            self.gate.reconcile_intake(PLAN)
        self.assertIsNone(self.store.intake(PLAN))

    def test_current_state_scope_and_owner_are_checked_before_spawn(self):
        self.accepted()
        for change in ({"status": "on hold"}, {"task": "changed scope"}, {"owner": "different owner"}):
            old = self.remote.controlled
            self.remote.edit(**change)
            with self.assertRaises(SafetyBlock):
                self.gate.dispatch(PLAN, "parser")
            self.assertEqual(self.remote.spawns, 0)
            self.remote.controlled = old

    def test_unapproved_ready_resume_does_not_create_new_intake_or_assignment(self):
        record = self.accepted()
        self.remote.edit(status="ready")
        with self.assertRaises(SafetyBlock):
            self.gate.dispatch(PLAN, "parser")
        with self.assertRaises(SafetyBlock):
            self.gate.reconcile_intake(PLAN)
        self.assertEqual(self.store.intake(PLAN)["token"], record["token"])
        self.assertEqual(self.remote.spawns, 0)

    def test_approved_ready_resume_reuses_existing_intake_and_assignment(self):
        original = self.accepted()
        first = self.gate.dispatch(PLAN, "parser")
        self.remote.edit(status="ready")
        proof = ResumeEvidence(self.remote.controlled.issue.url, self.remote.controlled.plan.body_hash,
                               original["owner"], "user", "2026-11-11T14:00:00Z",
                               "user-resume-event-1", True)
        with self.assertRaises(SafetyBlock):
            self.gate.resume(PLAN, replace(proof, previous_execution_settled=False))
        resumed = self.gate.resume(PLAN, proof)
        recovered = self.gate.dispatch(PLAN, "parser")
        self.assertEqual(resumed["token"], original["token"])
        self.assertEqual(recovered["id"], first["id"])
        self.assertEqual(self.remote.spawns, 1)
        self.remote.edit(status="ready")
        with self.assertRaisesRegex(SafetyBlock, "already consumed"):
            self.gate.resume(PLAN, proof)

    def test_cancellation_after_reservation_is_checked_immediately_before_spawn(self):
        self.accepted()
        current = self.remote.current
        calls = 0
        def edit_second(number):
            nonlocal calls
            calls += 1
            if calls == 2:
                self.remote.edit(status="on hold")
            return current(number)
        with patch.object(self.remote, "current", side_effect=edit_second):
            with self.assertRaises(SafetyBlock):
                self.gate.dispatch(PLAN, "parser")
        self.assertEqual(self.remote.spawns, 0)

    def test_independent_subtasks_have_separate_assignments(self):
        self.accepted()
        a = self.gate.dispatch(PLAN, "parser")
        b = self.gate.dispatch(PLAN, "storage")
        self.assertNotEqual(a["id"], b["id"])
        self.assertEqual(self.remote.spawns, 2)
        self.assertEqual(self.store.db.execute("SELECT count(*) FROM intakes").fetchone()[0], 1)

    def test_dependencies_do_not_run_until_recorded_complete(self):
        self.accepted()
        with self.assertRaisesRegex(SafetyBlock, "dependency"):
            self.gate.dispatch(PLAN, "storage", dependencies=["parser"])
        first = self.gate.dispatch(PLAN, "parser")
        self.store.attempt_phase(first["id"], "completed", session_id=first["session_id"])
        self.gate.dispatch(PLAN, "storage", dependencies=["parser"])
        self.assertEqual(self.remote.spawns, 2)

    def test_send_response_loss_never_blindly_resends(self):
        self.accepted()
        self.remote.lose_send = True
        with self.assertRaises(RecoveryRequired):
            self.gate.handoff(PLAN, "existing-orchestrator")
        with self.assertRaises(RecoveryRequired):
            self.gate.handoff(PLAN, "existing-orchestrator")
        self.gate.reconcile_delivery(PLAN)
        self.gate.handoff(PLAN, "existing-orchestrator")
        self.assertEqual(self.remote.sends, 1)

    def test_spawn_response_loss_reconciles_same_attempt_after_restart(self):
        self.accepted()
        self.remote.lose_spawn = True
        with self.assertRaises(RecoveryRequired):
            self.gate.dispatch(PLAN, "parser")
        row = self.store.db.execute("SELECT * FROM attempts").fetchone()
        self.reopen()
        with self.assertRaises(RecoveryRequired):
            self.gate.dispatch(PLAN, "parser")
        self.gate.reconcile_spawn(row["id"])
        recovered = self.gate.dispatch(PLAN, "parser")
        self.assertEqual(recovered["id"], row["id"])
        self.assertEqual(self.remote.spawns, 1)

    def test_zero_and_multiple_matches_remain_held_without_retry(self):
        self.accepted()
        self.remote.lose_spawn = True
        with self.assertRaises(RecoveryRequired):
            self.gate.dispatch(PLAN, "parser")
        row = self.store.db.execute("SELECT * FROM attempts").fetchone()
        receipt = self.remote.receipts[0]
        for matches in ([], [receipt, receipt]):
            self.remote.receipts = matches
            with self.assertRaises(RecoveryRequired):
                self.gate.reconcile_spawn(row["id"])
            with self.assertRaises(RecoveryRequired):
                self.gate.dispatch(PLAN, "parser")
        self.assertEqual(self.remote.spawns, 1)
        self.assertEqual(self.store.intake(PLAN)["phase"], "active")

    def test_wrong_worker_head_is_not_reported_running(self):
        self.accepted()
        self.remote.bad_head = True
        with self.assertRaises(RecoveryRequired):
            self.gate.dispatch(PLAN, "parser")
        self.assertEqual(self.store.db.execute("SELECT phase FROM attempts").fetchone()[0], "uncertain")

    def test_external_success_local_ack_failure_is_reconciled_not_reexecuted(self):
        self.accepted()
        original = self.store.attempt_phase
        def fail_first(attempt_id, phase, **kwargs):
            if phase == "running":
                raise OSError("disk write failed")
            return original(attempt_id, phase, **kwargs)
        with patch.object(self.store, "attempt_phase", side_effect=fail_first):
            with self.assertRaises(RecoveryRequired):
                self.gate.dispatch(PLAN, "parser")
        row = self.store.db.execute("SELECT * FROM attempts").fetchone()
        self.reopen()
        self.gate.reconcile_spawn(row["id"])
        self.assertEqual(self.remote.spawns, 1)

    def test_new_id_initialization_is_persisted_before_claim_and_reused(self):
        current = issue(plan_id="", source="")
        self.remote = FakeBackend(current, self.root / "worker")
        self.gate = Gate(self.store, self.remote, "rng")
        proof = NewEvidence(current.url, parse(current).body_hash, "user",
                            "2026-11-11T14:00:00Z", "user declaration", True, True)
        self.scan([raw_issue(plan_id="", source="")], {current.url: proof})
        record = self.gate.accept(current, "owner", proof)
        generated = record["plan_id"]
        self.assertRegex(generated, r"^plan-\d{8}-[0-9a-f]{8}$")
        self.reopen()
        self.assertEqual(self.store.initialized_id(1, current.url), generated)
        self.assertEqual(self.remote.claims, 1)

    def test_concurrent_dispatchers_share_one_gate(self):
        self.accepted()
        entered, release = threading.Event(), threading.Event()
        self.remote.spawn_entered, self.remote.spawn_release = entered, release
        outcomes = []
        def dispatch():
            local = None
            try:
                local = Store(self.directory, self.bindings, REPO, [self.workspace])
                outcomes.append(Gate(local, self.remote, "rng").dispatch(PLAN, "parser"))
            except Exception as exc:
                outcomes.append(exc)
            finally:
                if local:
                    local.close()
        first = threading.Thread(target=dispatch)
        first.start()
        self.assertTrue(entered.wait(5))
        second = threading.Thread(target=dispatch)
        second.start()
        second.join(5)
        release.set()
        first.join(5)
        self.assertFalse(first.is_alive() or second.is_alive())
        self.assertEqual(self.remote.spawns, 1)
        self.assertEqual(sum(isinstance(o, AlreadyRunning) for o in outcomes), 1)
