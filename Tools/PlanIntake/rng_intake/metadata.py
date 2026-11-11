from dataclasses import dataclass
from datetime import datetime, timedelta, timezone
import hashlib
import json
import re
import secrets

import yaml
from yaml.nodes import MappingNode, ScalarNode
from yaml.tokens import AliasToken, AnchorToken, TagToken

from .errors import InvalidPlan


STATUSES = frozenset(("proposed", "ready", "active", "on hold", "completed", "rejected"))
FIELDS = frozenset(("id", "status", "current_step", "source"))
PLAN_ID = re.compile(r"plan-(\d{8})-[0-9a-f]{8}\Z")
FENCE = re.compile(r"^ {0,3}(" + chr(96) + r"{3,}|~{3,})(.*)$")
HEADING = re.compile(r"^ {0,3}(#{1,6})[ \t]+(.+?)[ \t]*#*[ \t]*$")
MAX_BODY = 65536
MAX_METADATA = 8192


def digest(value: str) -> str:
    return hashlib.sha256(value.encode("utf-8")).hexdigest()


def new_plan_id() -> str:
    day = datetime.now(timezone(timedelta(hours=9))).strftime("%Y%m%d")
    return f"plan-{day}-{secrets.token_hex(4)}"


def valid_plan_id(value: str) -> bool:
    match = PLAN_ID.fullmatch(value)
    if not match:
        return False
    try:
        datetime.strptime(match[1], "%Y%m%d")
        return True
    except ValueError:
        return False


@dataclass(frozen=True)
class Plan:
    plan_id: str
    status: str
    current_step: str
    source: str
    body_hash: str
    scope_hash: str


@dataclass(frozen=True)
class Issue:
    number: int
    url: str
    title: str
    body: str
    updated_at: str

    @classmethod
    def from_api(cls, raw: dict, repo: str):
        if "pull_request" in raw:
            return None
        number = raw.get("number")
        if type(number) is not int or number < 1:
            raise InvalidPlan("invalid issue identity")
        url = f"https://github.com/{repo}/issues/{number}"
        if raw.get("html_url") != url:
            raise InvalidPlan("issue URL/repository mismatch")
        body = raw.get("body")
        title = raw.get("title")
        updated = raw.get("updated_at")
        if body is None:
            body = ""
        if not all(isinstance(v, str) for v in (body, title, updated)):
            raise InvalidPlan("invalid issue field types")
        return cls(number, url, title, body, updated)


def _sections(body: str):
    lines = body.splitlines(keepends=True)
    headings, fences = [], []
    opened = None
    offset = 0
    for line in lines:
        text = line.rstrip("\r\n")
        marker = FENCE.match(text)
        if opened:
            start, char, length, language, content_start = opened
            if marker and marker[1][0] == char and len(marker[1]) >= length and not marker[2].strip():
                fences.append((start, offset + len(line), language, body[content_start:offset]))
                opened = None
        elif marker:
            opened = (offset, marker[1][0], len(marker[1]), marker[2].strip(), offset + len(line))
        else:
            heading = HEADING.match(text)
            if heading:
                headings.append((offset, offset + len(line), len(heading[1]), heading[2]))
        offset += len(line)
    if opened:
        raise InvalidPlan("unclosed Markdown fence")
    return headings, fences


def _region(headings, heading, end):
    return next((h[0] for h in headings if h[0] > heading[0] and h[2] <= heading[2]), end)


def parse(issue: Issue) -> Plan:
    body = issue.body
    if len(body.encode("utf-8")) > MAX_BODY:
        raise InvalidPlan("issue body exceeds parser limit")
    headings, fences = _sections(body)
    metadata = [h for h in headings if h[3] == "PLAN metadata"]
    if len(metadata) != 1:
        raise InvalidPlan("exactly one PLAN metadata heading required")
    heading = metadata[0]
    end = _region(headings, heading, len(body))
    blocks = [b for b in fences if heading[1] <= b[0] < end]
    if len(blocks) != 1 or blocks[0][2] != "yaml":
        raise InvalidPlan("exactly one yaml block required in PLAN metadata")
    start, finish, _, content = blocks[0]
    if finish > end or body[heading[1]:start].strip() or body[finish:end].strip():
        raise InvalidPlan("unexpected text in PLAN metadata")
    if len(content) > MAX_METADATA:
        raise InvalidPlan("metadata exceeds parser limit")
    try:
        tokens = list(yaml.scan(content))
        if len(tokens) > 80 or any(isinstance(t, (AliasToken, AnchorToken, TagToken)) for t in tokens):
            raise InvalidPlan("YAML anchors, aliases, tags or excessive tokens are unsupported")
        node = yaml.compose(content, Loader=yaml.SafeLoader)
        if not isinstance(node, MappingNode):
            raise InvalidPlan("metadata must be a mapping")
        # Keep a unique, valid identity even if another field needs repair.
        # Duplicate ID keys are ambiguous and must not choose an identity.
        ids = [value for key, value in node.value
               if isinstance(key, ScalarNode) and key.tag == "tag:yaml.org,2002:str" and key.value == "id"]
        identified = (ids[0].value if len(ids) == 1 and isinstance(ids[0], ScalarNode)
                      and ids[0].tag == "tag:yaml.org,2002:str" and valid_plan_id(ids[0].value) else None)

        def invalid(message):
            return InvalidPlan(message, plan_id=identified)

        values = {}
        for key, value in node.value:
            if not isinstance(key, ScalarNode) or key.tag != "tag:yaml.org,2002:str":
                raise invalid("metadata keys must be strings")
            if key.value in values:
                raise invalid("duplicate YAML key")
            if not isinstance(value, ScalarNode) or value.tag != "tag:yaml.org,2002:str":
                raise invalid("metadata values must be strings")
            values[key.value] = value.value
    except (yaml.YAMLError, RecursionError) as exc:
        raise InvalidPlan("invalid YAML") from exc
    if set(values) != FIELDS:
        raise invalid("metadata fields must be id/status/current_step/source")
    if values["status"] not in STATUSES or not values["current_step"].strip():
        raise invalid("invalid status or empty current_step")
    if values["id"] and not valid_plan_id(values["id"]):
        raise invalid("invalid PLAN ID")
    if values["source"] and values["source"] != issue.url:
        raise invalid("source must equal this issue URL; migration needs repair")
    goals = [h for h in headings if h[3] in ("작업 내용", "문제 내용", "리팩토링 내용", "목표와 범위")]
    completions = [h for h in headings if h[3] == "완료 조건"]
    if len(goals) != 1 or len(completions) != 1:
        raise invalid("one task/scope and one completion section required")
    for section in goals + completions:
        content_text = body[section[1]:_region(headings, section, len(body))].strip()
        if not content_text or content_text == "_No response_":
            raise invalid("empty task/scope or completion section")

    # Progress and intake receipts have explicit sections; all other text is
    # approved scope. Unknown sections are included, never silently ignored.
    excluded = [(heading[0], end)]
    for name in ("PLAN progress", "PLAN intake"):
        matches = [h for h in headings if h[3] == name]
        if len(matches) > 1:
            raise invalid("duplicate managed section")
        for h in matches:
            excluded.append((h[0], _region(headings, h, len(body))))
    for left, right in zip(sorted(excluded), sorted(excluded)[1:]):
        if left[1] > right[0]:
            raise invalid("overlapping managed sections")
    for section in goals + completions:
        section_end = _region(headings, section, len(body))
        if any(a < section_end and section[0] < b for a, b in excluded):
            raise invalid("required task/completion section overlaps managed region")
    scope = body
    for a, b in sorted(excluded, reverse=True):
        scope = scope[:a] + scope[b:]
    # Appending a managed section adds a separator newline, not approved text.
    scope = scope.rstrip("\r\n") + "\n"
    if not scope.strip() or not issue.title.strip():
        raise invalid("PLAN scope and issue title required")
    return Plan(values["id"], values["status"], values["current_step"], values["source"],
                digest(body), digest(issue.title + "\n" + scope))


@dataclass(frozen=True)
class NewEvidence:
    issue_url: str
    body_hash: str
    submitted_by: str
    confirmed_at: str
    reference: str
    comparison_complete: bool
    new_submission: bool

    def valid_for(self, issue: Issue, plan: Plan):
        # Supplied by the trusted local operator from a user/delegate record.
        # GitHub's issue creator is not evidence of who edited today's body.
        if not all(isinstance(v, str) for v in (self.issue_url, self.body_hash, self.submitted_by,
                                               self.confirmed_at, self.reference)):
            return False
        try:
            timestamp = datetime.fromisoformat(self.confirmed_at.replace("Z", "+00:00"))
            if timestamp.utcoffset() is None:
                return False
        except (ValueError, TypeError):
            return False
        return (self.issue_url == issue.url and self.body_hash == plan.body_hash
                and bool(self.submitted_by.strip()) and bool(self.reference.strip())
                and self.comparison_complete is True and self.new_submission is True)


@dataclass(frozen=True)
class ResumeEvidence:
    issue_url: str
    body_hash: str
    previous_owner: str
    approved_by: str
    approved_at: str
    reference: str
    previous_execution_settled: bool

    def valid_for(self, current: Plan, record) -> bool:
        strings = (self.issue_url, self.body_hash, self.previous_owner,
                   self.approved_by, self.approved_at, self.reference)
        if not all(isinstance(v, str) and v.strip() for v in strings):
            return False
        try:
            stamp = datetime.fromisoformat(self.approved_at.replace("Z", "+00:00"))
            if stamp.utcoffset() is None:
                return False
        except ValueError:
            return False
        return (self.issue_url == record["source"] and self.body_hash == current.body_hash
                and self.previous_owner == record["owner"] and self.previous_execution_settled is True)


def classify(issue: Issue, plan: Plan, *, duplicate=False, known=False, evidence=None) -> str:
    if duplicate:
        return "duplicate_plan_id"
    if plan.status != "ready":
        return "existing_active" if plan.status == "active" and known else "not_ready"
    if known:
        return "existing_assignment_requires_reconciliation"
    if not plan.plan_id or not plan.source:
        if evidence is not None and evidence.valid_for(issue, plan):
            return "initialization_required"
        return "new_evidence_required"
    return "ready_candidate"


def stable_json(value) -> str:
    return json.dumps(value, ensure_ascii=False, sort_keys=True, separators=(",", ":"))
