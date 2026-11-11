"""Shared policy core. Production CLI transports remain disabled, not opt-in."""

from dataclasses import dataclass, replace
from pathlib import Path
import re

from .errors import RecoveryRequired, SafetyBlock
from .metadata import Issue, Plan, ResumeEvidence, classify, parse


@dataclass(frozen=True)
class Capabilities:
    conditional_claim: bool = False
    durable_delivery: bool = False
    correlated_spawn: bool = False


@dataclass(frozen=True)
class ControlledIssue:
    issue: Issue
    plan: Plan
    token: str = ""
    owner: str = ""


@dataclass(frozen=True)
class WorkerReceipt:
    session_id: str
    project_id: str
    branch: str
    worktree: str
    head: str
    correlation: str
    verified_worktree: bool


class Gate:
    """All accept/handoff/spawn/reconciliation operations share the same ledger.

    Backend claims must be atomic compare-and-set, not read/PATCH/read. Receipts
    must refer to an inert worker until the backend verifies its worktree/HEAD.
    No installed AO/GitHub transport satisfies this contract in this release.
    """

    def __init__(self, store, backend, project):
        self.store, self.backend, self.project = store, backend, project

    def _capability(self, name):
        if not getattr(self.backend.capabilities, name):
            raise SafetyBlock(f"{name} is unproven/unsupported; external effect disabled")

    def _scan(self, issue, plan):
        latest = self.store.db.execute("SELECT id FROM scans ORDER BY rowid DESC LIMIT 1").fetchone()
        row = self.store.db.execute("SELECT * FROM observations WHERE issue=?", (issue.number,)).fetchone()
        if (not latest or not row or row["scan_id"] != latest["id"]
                or row["body_hash"] != plan.body_hash
                or row["classification"] not in ("ready_candidate", "initialization_required")):
            raise SafetyBlock("complete current scan and eligible observation required")
        if plan.plan_id and self.store.db.execute(
                "SELECT 1 FROM observations WHERE plan_id=? AND issue<>?",
                (plan.plan_id, issue.number)).fetchone():
            raise SafetyBlock("duplicate PLAN ID in the complete issue inventory")

    def accept(self, issue: Issue, owner: str, evidence=None):
        self._capability("conditional_claim")
        if not owner.strip():
            raise SafetyBlock("intake owner required")
        with self.store.lock("dispatch"):
            plan = parse(issue)
            self._scan(issue, plan)
            fresh = self.backend.current(issue.number)
            if fresh.plan.body_hash != plan.body_hash or fresh.issue.url != issue.url:
                raise SafetyBlock("source changed after scan; rescan instead of overwriting")
            if fresh.token or fresh.owner:
                raise SafetyBlock("existing intake marker requires reconciliation, not new acceptance")
            if classify(issue, plan, known=bool(self.store.intake(plan.plan_id)),
                        evidence=evidence) not in ("ready_candidate", "initialization_required"):
                raise SafetyBlock("PLAN is not eligible for new intake")
            if not plan.plan_id or not plan.source:
                if evidence is None or not evidence.valid_for(issue, plan):
                    raise SafetyBlock("D1 evidence required")
                plan = replace(plan, plan_id=plan.plan_id or self.store.initialized_id(issue.number, issue.url),
                               source=issue.url)
            record = self.store.intent(issue, plan, owner)
            try:
                claimed = self.backend.claim(issue, plan, record["token"], owner)
                self._match(record, claimed)
                self.store.intake_phase(plan.plan_id, "active")
            except Exception as exc:
                # A successful remote claim can precede a local acknowledgement
                # failure. Never turn it back to ready or repeat the mutation.
                try:
                    self.store.intake_phase(plan.plan_id, "uncertain", type(exc).__name__)
                except Exception:
                    pass
                raise RecoveryRequired("claim outcome needs source reconciliation; spawn forbidden") from exc
            return self.store.intake(plan.plan_id)

    @staticmethod
    def _match(record, current):
        p = current.plan
        if (p.status != "active" or p.plan_id != record["plan_id"]
                or p.source != record["source"] or current.issue.url != record["source"]
                or p.scope_hash != record["scope_hash"] or current.token != record["token"]
                or current.owner != record["owner"]):
            raise SafetyBlock("approval/state/identity/ownership changed; hold")

    def _current(self, plan_id):
        record = self.store.intake(plan_id)
        if not record or record["phase"] != "active":
            raise SafetyBlock("confirmed active intake required")
        self._match(record, self.backend.current(record["issue"]))
        return record

    def reconcile_intake(self, plan_id):
        with self.store.lock("dispatch"):
            record = self.store.intake(plan_id)
            if not record:
                raise SafetyBlock("existing intake required; active issues are never auto-adopted")
            self._match(record, self.backend.current(record["issue"]))
            self.store.intake_phase(plan_id, "active", "original intake reconciled")
            return self.store.intake(plan_id)

    def resume(self, plan_id, evidence):
        self._capability("conditional_claim")
        with self.store.lock("dispatch"):
            record = self.store.intake(plan_id)
            if not record:
                raise SafetyBlock("existing intake required for resume")
            current = self.backend.current(record["issue"])
            if (not isinstance(evidence, ResumeEvidence) or not evidence.valid_for(current.plan, record)
                    or current.plan.status != "ready" or current.plan.plan_id != plan_id
                    or current.plan.source != record["source"] or current.issue.url != record["source"]
                    or current.plan.scope_hash != record["scope_hash"]
                    or current.token != record["token"] or current.owner != record["owner"]):
                raise SafetyBlock("approved ready resume and settled previous owner required")
            self.store.resume_intent(record, evidence)
            try:
                claimed = self.backend.claim(current.issue, current.plan, record["token"], record["owner"])
                self._match(record, claimed)
                self.store.intake_phase(plan_id, "active", "approved existing intake resumed")
                return self.store.intake(plan_id)
            except Exception as exc:
                try:
                    self.store.intake_phase(plan_id, "uncertain", type(exc).__name__)
                except Exception:
                    pass
                raise RecoveryRequired("resume uncertain; reconcile existing intake without new assignment") from exc

    def handoff(self, plan_id, target):
        self._capability("durable_delivery")
        with self.store.lock("dispatch"):
            record = self._current(plan_id)
            self.backend.validate_orchestrator(target, self.project)
            delivery, created = self.store.reserve_delivery(record["token"], target)
            if not created:
                if delivery["phase"] == "delivered":
                    return delivery["receipt"]
                raise RecoveryRequired("existing delivery uncertain; D4 approval required for retry")
            try:
                receipt = self.backend.send(target, record, delivery["correlation"])
                if not isinstance(receipt, str) or not receipt:
                    raise SafetyBlock("durable delivery receipt missing")
                self.store.delivery_phase(record["token"], "delivered", receipt)
                return receipt
            except Exception as exc:
                try:
                    self.store.delivery_phase(record["token"], "uncertain")
                except Exception:
                    pass
                raise RecoveryRequired("send uncertain; do not resend or respawn") from exc

    def dispatch(self, plan_id, task_key, dependencies=()):
        self._capability("correlated_spawn")
        if not re.fullmatch(r"[a-z0-9]+(?:-[a-z0-9]+)*", task_key) or len(task_key) > 48:
            raise SafetyBlock("stable task key must be 1-48 lowercase letters/digits/hyphens")
        with self.store.lock("dispatch"):
            record = self._current(plan_id)
            for dependency in dependencies:
                row = self.store.db.execute("""SELECT 1 FROM assignments a JOIN attempts t
                    ON t.assignment_id=a.id WHERE a.plan_id=? AND a.task_key=? AND t.phase='completed'""",
                                             (plan_id, dependency)).fetchone()
                if not row:
                    raise SafetyBlock("dependency has not completed")
            base = self.backend.develop_sha()
            if not re.fullmatch(r"[0-9a-f]{40}", base):
                raise SafetyBlock("verified full develop SHA required")
            assignment = self.store.reserve_assignment(plan_id, task_key, base, record["scope_hash"])
            attempt, created = self.store.attempt(assignment)
            if not created:
                if attempt["phase"] == "running":
                    return attempt
                raise RecoveryRequired("existing spawn intent is unresolved; do not retry")
            # Check again after durable reservation and immediately before effect.
            self._current(plan_id)
            try:
                receipt = self.backend.spawn(record, assignment, attempt, self.project)
                self._worker_match(assignment, attempt, receipt)
                self.store.attempt_phase(attempt["id"], "running", session_id=receipt.session_id)
                return dict(self.store.db.execute("SELECT * FROM attempts WHERE id=?", (attempt["id"],)).fetchone())
            except Exception as exc:
                try:
                    self.store.attempt_phase(attempt["id"], "uncertain", detail=type(exc).__name__)
                except Exception:
                    pass
                raise RecoveryRequired("spawn uncertain; compare AO inventory before any retry") from exc

    def reconcile_delivery(self, plan_id):
        with self.store.lock("dispatch"):
            record = self._current(plan_id)
            delivery = self.store.db.execute("SELECT * FROM deliveries WHERE intake_token=?",
                                              (record["token"],)).fetchone()
            if not delivery:
                raise SafetyBlock("existing delivery required")
            self.backend.validate_orchestrator(delivery["target"], self.project)
            receipts = self.backend.delivery_receipts(delivery["target"], delivery["correlation"])
            if len(receipts) != 1 or not isinstance(receipts[0], str) or not receipts[0]:
                raise RecoveryRequired("delivery is unproven/ambiguous; D4 user approval required for retry")
            self.store.delivery_phase(record["token"], "delivered", receipts[0])
            return receipts[0]

    def _worker_match(self, assignment, attempt, receipt):
        if (not isinstance(receipt, WorkerReceipt) or not receipt.session_id
                or receipt.project_id != self.project or receipt.branch != attempt["branch"]
                or receipt.head != assignment["base_sha"] or receipt.correlation != attempt["correlation"]
                or not receipt.verified_worktree or not Path(receipt.worktree).is_absolute()):
            raise SafetyBlock("worker project/branch/worktree/HEAD/correlation not verified")

    def reconcile_spawn(self, attempt_id):
        with self.store.lock("dispatch"):
            row = self.store.db.execute("SELECT * FROM attempts WHERE id=?", (attempt_id,)).fetchone()
            if not row:
                raise SafetyBlock("existing attempt required")
            attempt = dict(row)
            assignment = dict(self.store.db.execute("SELECT * FROM assignments WHERE id=?",
                                                    (attempt["assignment_id"],)).fetchone())
            self._current(assignment["plan_id"])
            receipts = self.backend.lookup(attempt["correlation"], attempt["branch"], self.project)
            if len(receipts) != 1:
                raise RecoveryRequired("zero/ambiguous matches: investigate and hold for D4 user decision")
            self._worker_match(assignment, attempt, receipts[0])
            if attempt["session_id"] and attempt["session_id"] != receipts[0].session_id:
                raise RecoveryRequired("receipt contradicts recorded session; hold")
            self.store.attempt_phase(attempt_id, "running", session_id=receipts[0].session_id,
                                     detail="unique existing execution reconciled")
            return receipts[0].session_id
