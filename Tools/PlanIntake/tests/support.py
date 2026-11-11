from dataclasses import replace
import json
from pathlib import Path
import sys
import tempfile
import unittest

sys.path.insert(0, str(Path(__file__).resolve().parents[1]))

from rng_intake.collector import evaluate
from rng_intake.gate import Capabilities, ControlledIssue, Gate, WorkerReceipt
from rng_intake.metadata import Issue, parse
from rng_intake.store import Store


REPO = "S-D-Project/RNG"
PLAN = "plan-20261111-1234abcd"
SHA = "a" * 40
FENCE = chr(96) * 3


def body(*, plan_id=PLAN, status="ready", source=None, number=1, task="implement collector", progress=""):
    if source is None:
        source = f"https://github.com/{REPO}/issues/{number}"
    text = ("### PLAN metadata\n\n" + FENCE + "yaml\n"
            + f"id: {json.dumps(plan_id)}\nstatus: {status}\ncurrent_step: assignment pending\n"
            + f"source: {json.dumps(source)}\n" + FENCE + "\n\n"
            + f"### 작업 내용\n\n{task}\n\n### 완료 조건\n\nfocused tests pass\n")
    if progress:
        text += "\n### PLAN progress\n\n" + progress + "\n"
    return text


def raw_issue(number=1, **kwargs):
    return {"number": number, "html_url": f"https://github.com/{REPO}/issues/{number}",
            "title": "collector task", "body": body(number=number, **kwargs),
            "updated_at": "2026-11-11T14:00:00Z", "state": "open", "labels": [], "assignees": []}


def issue(number=1, **kwargs):
    return Issue.from_api(raw_issue(number, **kwargs), REPO)


class FakeBackend:
    """A contract test double, not evidence that live APIs have these features."""

    capabilities = Capabilities(True, True, True)

    def __init__(self, current, worktree):
        self.controlled = ControlledIssue(current, parse(current))
        self.worktree = str(worktree.resolve())
        self.claims, self.sends, self.spawns = 0, 0, 0
        self.lose_claim = self.lose_send = self.lose_spawn = False
        self.fail_claim = False
        self.edit_during_claim = False
        self.bad_head = False
        self.receipts, self.deliveries = [], []
        self.spawn_entered = self.spawn_release = None

    def current(self, number):
        assert number == self.controlled.issue.number
        return self.controlled

    def edit(self, *, status=None, task=None, owner=None):
        current = self.controlled
        edited = replace(current.issue, body=body(plan_id=current.plan.plan_id,
                         status=status or current.plan.status, source=current.plan.source,
                         task=task or "implement collector"))
        self.controlled = ControlledIssue(edited, parse(edited), current.token,
                                          current.owner if owner is None else owner)

    def claim(self, original, plan, token, owner):
        self.claims += 1
        if self.edit_during_claim:
            self.edit(task="user concurrent edit")
        if self.fail_claim or self.controlled.plan.body_hash != parse(original).body_hash:
            raise TimeoutError("conditional claim refused or unconfirmed")
        content = original.body
        start = content.index(FENCE + "yaml")
        finish = content.index(FENCE, start + len(FENCE)) + len(FENCE)
        replacement = (FENCE + "yaml\n" + f"id: {json.dumps(plan.plan_id)}\nstatus: active\n"
                       + "current_step: assignment pending\n" + f"source: {json.dumps(plan.source)}\n" + FENCE)
        edited = replace(original, body=content[:start] + replacement + content[finish:])
        self.controlled = ControlledIssue(edited, parse(edited), token, owner)
        if self.lose_claim:
            raise TimeoutError("successful claim response lost")
        return self.controlled

    def validate_orchestrator(self, target, project):
        assert target == "existing-orchestrator" and project == "rng"

    def send(self, target, record, correlation):
        self.sends += 1
        self.deliveries = ["receipt-" + correlation]
        if self.lose_send:
            raise TimeoutError("successful send response lost")
        return self.deliveries[0]

    def delivery_receipts(self, target, correlation):
        return self.deliveries

    def develop_sha(self):
        return SHA

    def spawn(self, record, assignment, attempt, project):
        self.spawns += 1
        if self.spawn_entered:
            self.spawn_entered.set()
            assert self.spawn_release.wait(5), "concurrency test release timeout"
        receipt = WorkerReceipt("session-" + attempt["correlation"], project, attempt["branch"],
                                self.worktree, "b" * 40 if self.bad_head else assignment["base_sha"],
                                attempt["correlation"], True)
        self.receipts = [receipt]
        if self.lose_spawn:
            raise TimeoutError("successful spawn response lost")
        return receipt

    def lookup(self, correlation, branch, project):
        return self.receipts


class StateCase(unittest.TestCase):
    def setUp(self):
        self.temporary = tempfile.TemporaryDirectory(prefix="rng-intake-test-")
        self.root = Path(self.temporary.name)
        self.workspace = self.root / "workspace"
        self.workspace.mkdir()
        self.directory, self.bindings = self.root / "state", self.root / "bindings"
        Store.initialize(self.directory, self.bindings, REPO, [self.workspace])
        self.store = Store(self.directory, self.bindings, REPO, [self.workspace])
        self.remote = FakeBackend(issue(), self.root / "worker")
        self.gate = Gate(self.store, self.remote, "rng")

    def tearDown(self):
        self.store.close()
        self.temporary.cleanup()

    def scan(self, raws=None, evidence=None):
        raws = raws or [raw_issue()]
        observations = evaluate(raws, REPO, store=self.store, evidence=evidence)
        self.store.record_scan(observations, "2026-11-11T14:00:00Z")
        return observations

    def accepted(self):
        self.scan()
        return self.gate.accept(self.remote.controlled.issue, "existing-orchestrator")

    def reopen(self):
        self.store.close()
        self.store = Store(self.directory, self.bindings, REPO, [self.workspace])
        self.gate = Gate(self.store, self.remote, "rng")
