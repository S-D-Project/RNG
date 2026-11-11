from dataclasses import replace
import unittest

from support import FENCE, PLAN, REPO, body, issue, raw_issue
from rng_intake.collector import evaluate
from rng_intake.errors import InvalidPlan
from rng_intake.metadata import NewEvidence, classify, parse


class MetadataTests(unittest.TestCase):
    def test_ready_candidate_does_not_depend_on_github_open_labels_assignees(self):
        raw = raw_issue()
        raw.update(state="closed", labels=[{"name": "completed"}], assignees=[{"login": "someone"}])
        result = evaluate([raw], REPO)
        self.assertEqual(result[0]["classification"], "ready_candidate")

    def test_other_statuses_never_become_new_intake(self):
        for status in ("proposed", "active", "on hold", "completed", "rejected"):
            with self.subTest(status=status):
                current = issue(status=status)
                self.assertEqual(classify(current, parse(current)), "not_ready")

    def test_invalid_metadata_is_not_ready(self):
        original = issue()
        bad_bodies = [
            "### 작업 내용\nno metadata",
            body().replace("status: ready\n", ""),
            body().replace("status: ready", "status: READY"),
            body().replace("status: ready", "status: true"),
            body().replace("current_step: assignment pending", "current_step: null"),
            body().replace("id: ", "id: 42\nunused: ", 1),
            body().replace("status: ready", "status: ready\nstatus: ready"),
            body().replace("status: ready", "status: ready\n'status': active"),
            body().replace("status: ready", "status: ready\nextra: value"),
            body().replace("status: ready", "status: &s ready\nother: *s"),
            body().replace("status: ready", "status: !!str ready"),
            body().replace("status: ready", "<<: {status: ready}"),
            body().replace(FENCE + "yaml", FENCE + "yml"),
            body().replace(FENCE + "yaml", FENCE + "json"),
            body().replace("status: ready", "status: [ready]"),
            body().replace("status: ready", "status: {nested: ready}"),
            body().replace("status: ready", "status: ready\n---\nstatus: active"),
            body().replace("\n\n### 작업 내용", "\n" + FENCE + "yaml\nstatus: ready\n" + FENCE + "\n\n### 작업 내용"),
            body() + "\n### PLAN metadata\n" + FENCE + "yaml\nid: x\n" + FENCE,
            body().replace("### 작업 내용", "unexpected text\n### 작업 내용"),
            body().replace(PLAN, "plan-20260231-1234abcd"),
            body().replace("/issues/1", "/issues/2"),
            body().replace("### 작업 내용", FENCE + "\n### 작업 내용"),
            body().replace("### 완료 조건\n\nfocused tests pass", "### 완료 조건"),
            body().replace("### 작업 내용", "### unknown section"),
        ]
        for text in bad_bodies:
            with self.subTest(text=text):
                with self.assertRaises(InvalidPlan):
                    parse(replace(original, body=text))

    def test_fake_metadata_inside_an_example_fence_is_ignored(self):
        current = issue()
        example = FENCE + "text\n### PLAN metadata\nexample only\n" + FENCE + "\n"
        self.assertEqual(parse(replace(current, body=current.body + example)).status, "ready")

    def test_metadata_progress_and_scope_versions_are_distinct(self):
        initial = parse(issue())
        self.assertEqual(initial.scope_hash, parse(issue(status="active")).scope_hash)
        self.assertEqual(initial.scope_hash, parse(issue(progress="verification done")).scope_hash)
        self.assertNotEqual(initial.scope_hash, parse(issue(task="changed approved task")).scope_hash)
        self.assertNotEqual(initial.scope_hash, parse(replace(issue(), title="different title")).scope_hash)

    def test_blank_new_fields_need_scoped_attestation_not_creator_or_empty_values(self):
        current = issue(plan_id="", source="")
        plan = parse(current)
        self.assertEqual(classify(current, plan), "new_evidence_required")
        proof = NewEvidence(current.url, plan.body_hash, "user", "2026-11-11T23:00:00+09:00",
                            "user request recording", True, True)
        self.assertEqual(classify(current, plan, evidence=proof), "initialization_required")
        for bad in (replace(proof, body_hash="old"), replace(proof, comparison_complete=False),
                    replace(proof, new_submission=False), replace(proof, reference=""),
                    replace(proof, confirmed_at="2026-11-11T23:00:00")):
            self.assertEqual(classify(current, plan, evidence=bad), "new_evidence_required")

    def test_same_id_across_issues_is_held_before_any_acceptance(self):
        result = evaluate([raw_issue(1), raw_issue(2)], REPO)
        self.assertEqual([r["classification"] for r in result], ["duplicate_plan_id"] * 2)

    def test_pull_requests_and_legacy_issues_never_become_candidates(self):
        raws = [dict(raw_issue(5), pull_request={})]
        raws.extend(dict(raw_issue(n), body="### 작업 내용\nlegacy issue") for n in (15, 27, 28))
        results = evaluate(raws, REPO)
        self.assertEqual([r["issue"] for r in results], [15, 27, 28])
        self.assertTrue(all(r["classification"] == "metadata_invalid" for r in results))

    def test_local_original_and_completed_records_are_preserved(self):
        current = issue()
        for record in ({"source": "Docs/Plans/original.md", "status": "completed"},
                       {"source": current.url, "status": "completed"},
                       {"source": current.url, "status": "active"}):
            observations = evaluate([raw_issue()], REPO, local={PLAN: [record]})
            self.assertNotEqual(observations[0]["classification"], "ready_candidate")

    def test_repeated_issue_in_pages_requires_rescan_not_partial_checkpoint(self):
        with self.assertRaisesRegex(Exception, "rescan"):
            evaluate([raw_issue(), raw_issue()], REPO)


if __name__ == "__main__":
    unittest.main()
