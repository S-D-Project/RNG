from collections import Counter
from pathlib import Path

import yaml
from yaml.nodes import MappingNode, ScalarNode

from .adapters import AO_REASON, CAS_REASON
from .errors import IntakeError, InvalidPlan
from .metadata import Issue, classify, digest, parse, valid_plan_id


def local_records(workspace):
    records = {}
    for path in sorted((Path(workspace) / "Docs" / "Plans").glob("plan-*.md")):
        try:
            text = path.read_text(encoding="utf-8-sig")
            parts = text.split("---", 2)
            if len(parts) != 3 or parts[0]:
                raise ValueError("front matter missing")
            node = yaml.compose(parts[1], Loader=yaml.SafeLoader)
            if not isinstance(node, MappingNode):
                raise ValueError("mapping required")
            values = {}
            for key, value in node.value:
                if (not isinstance(key, ScalarNode) or not isinstance(value, ScalarNode)
                        or key.tag != "tag:yaml.org,2002:str" or value.tag != "tag:yaml.org,2002:str"
                        or key.value in values):
                    raise ValueError("invalid local metadata")
                values[key.value] = value.value
            if not valid_plan_id(values.get("id", "")) or not values.get("source"):
                raise ValueError("local identity missing")
            records.setdefault(values["id"], []).append(values)
        except (OSError, ValueError, yaml.YAMLError, RecursionError) as exc:
            raise IntakeError("local PLAN inventory incomplete: " + path.name) from exc
    return records


def evaluate(raw_issues, repo, *, local=None, store=None, evidence=None):
    """Classify only after all pages were fetched; no GitHub/AO writes."""
    local, evidence = local or {}, evidence or {}
    parsed, observations, numbers = [], [], set()
    for raw in raw_issues:
        issue = Issue.from_api(raw, repo)
        if issue is None:
            continue
        if issue.number in numbers:
            raise IntakeError("issue moved/repeated across pages; rescan before checkpoint")
        numbers.add(issue.number)
        try:
            plan = parse(issue)
            parsed.append((issue, plan))
        except InvalidPlan as exc:
            observations.append({"issue": issue.number, "source": issue.url, "body_hash": digest(issue.body),
                                 "plan_id": exc.plan_id, "classification": "metadata_invalid", "detail": str(exc)})
    counts = Counter(p.plan_id for _, p in parsed if p.plan_id)
    counts.update(o["plan_id"] for o in observations if o.get("plan_id"))
    for issue, plan in parsed:
        known = store.intake(plan.plan_id) if store and plan.plan_id else None
        category = classify(issue, plan, duplicate=bool(plan.plan_id and counts[plan.plan_id] > 1),
                            known=bool(known), evidence=evidence.get(issue.url))
        previous = (store.db.execute("SELECT first_plan_id FROM observations WHERE issue=?", (issue.number,)).fetchone()
                    if store else None)
        if previous and previous["first_plan_id"] and plan.plan_id != previous["first_plan_id"]:
            category = "identity_changed"
        entries = local.get(plan.plan_id, [])
        if entries:
            if len(entries) != 1 or entries[0]["source"] != issue.url:
                category = "local_source_not_migrated"
            elif entries[0].get("status") in ("completed", "active", "on hold", "rejected") and not known:
                category = "existing_local_plan_requires_handoff"
        observations.append({"issue": issue.number, "source": issue.url, "plan_id": plan.plan_id,
                             "status": plan.status, "body_hash": plan.body_hash, "scope_hash": plan.scope_hash,
                             "classification": category})
    return sorted(observations, key=lambda o: o["issue"])


def report(observations, *, pages=0, orchestrator=None, mode="dry-run"):
    return {"mode": mode, "automatic_intake": False, "external_writes": False,
            "blocked_by": [CAS_REASON, AO_REASON], "orchestrator": orchestrator, "pages": pages,
            "counts": dict(Counter(o["classification"] for o in observations)), "observations": observations}
