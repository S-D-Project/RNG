import argparse
from datetime import datetime, timedelta, timezone
import json
import os
from pathlib import Path
import signal
import sqlite3
import threading

from .adapters import AoRead, GitHubRead, ReadFailure, worktree_roots
from .collector import evaluate, local_records, report
from .errors import IntakeError
from .metadata import NewEvidence
from .store import Store


WORKSPACE = Path(__file__).resolve().parents[3]


def emit(value):
    # ASCII JSON also works in Windows scheduled tasks with a legacy code page.
    print(json.dumps(value, ensure_ascii=True, sort_keys=True), flush=True)


def binding_root():
    home = Path(os.environ.get("LOCALAPPDATA", str(Path.home() / ".local" / "state")))
    return home / "RNG" / "PlanIntake" / "bindings"


def default_state(repo):
    return binding_root().parent / "state" / repo.replace("/", "-")


def parser():
    p = argparse.ArgumentParser(description="Read-only PLAN collector; automatic claim/send/spawn are disabled.")
    p.add_argument("--repo", default="S-D-Project/RNG")
    p.add_argument("--workspace", type=Path, default=WORKSPACE)
    p.add_argument("--project", default=os.environ.get("AO_PROJECT_ID"))
    p.add_argument("--orchestrator", help="Expected existing session ID; always verified, never hard-coded")
    p.add_argument("--state-dir", type=Path)
    sub = p.add_subparsers(dest="command", required=True)
    inspect = sub.add_parser("inspect", help="Stateless dry-run; no state or external writes")
    inspect.add_argument("--input", type=Path, help="Complete gh API JSON issue array instead of live GET")
    inspect.add_argument("--evidence", type=Path, help="Trusted local D1 attestation array; never inferred from author")
    inspect.add_argument("--skip-ao", action="store_true", help="Offline inspection only; leaves AO identity unverified")
    sub.add_parser("init-state", help="Explicit NEW installation only; existing/partial binding is refused")
    poll = sub.add_parser("poll", help="Persist observations only; no intake/handoff/spawn")
    poll.add_argument("--cycles", type=int, default=1, help="0 means continuous; default 1")
    poll.add_argument("--interval", type=int, default=60)
    sub.add_parser("status", help="Read existing ledger status")
    return p


def main(argv=None):
    args = parser().parse_args(argv)
    state = None
    stop = threading.Event()
    try:
        github = GitHubRead(args.repo)
        directory = args.state_dir or default_state(args.repo)
        workspace = args.workspace.resolve()
        if args.command == "inspect":
            inventory = None
            if args.input:
                raw = json.loads(args.input.read_text(encoding="utf-8-sig"))
            else:
                inventory = github.list_issues()
                raw = inventory.issues
            if not isinstance(raw, list) or not all(isinstance(x, dict) for x in raw):
                raise IntakeError("input must be a complete array of issues")
            evidence = {}
            if args.evidence:
                values = json.loads(args.evidence.read_text(encoding="utf-8-sig"))
                if not isinstance(values, list):
                    raise IntakeError("D1 evidence must be an array")
                for value in values:
                    item = NewEvidence(**value)
                    if item.issue_url in evidence:
                        raise IntakeError("duplicate D1 evidence")
                    evidence[item.issue_url] = item
            observations = evaluate(raw, args.repo, local=local_records(workspace), evidence=evidence)
            target = AoRead(args.project).orchestrator(args.orchestrator) if args.project and not args.skip_ao else None
            emit(report(observations, pages=inventory.pages if inventory else 0, orchestrator=target))
            return 0
        roots = worktree_roots(workspace)
        if args.command == "init-state":
            Store.initialize(directory, binding_root(), args.repo, roots)
            emit({"initialized": str(directory.resolve()), "automatic_intake": False})
            return 0
        state = Store(directory, binding_root(), args.repo, roots)
        if args.command == "status":
            emit({"automatic_intake": False,
                  "scans": state.db.execute("SELECT count(*) FROM scans").fetchone()[0],
                  "intakes": state.db.execute("SELECT count(*) FROM intakes").fetchone()[0],
                  "attempts": state.db.execute("SELECT count(*) FROM attempts").fetchone()[0]})
            return 0
        if args.interval < 60 or args.cycles < 0:
            raise IntakeError("interval must be at least 60 seconds; cycles must be nonnegative")
        signal.signal(signal.SIGINT, lambda *_: stop.set())
        signal.signal(signal.SIGTERM, lambda *_: stop.set())
        # The collector lock covers waits and all cycles. Dispatch has its own
        # common lock and ledger transaction for callers outside the collector.
        with state.lock("collector"):
            completed, failures = 0, 0
            while not stop.is_set():
                last = state.db.execute("SELECT payload FROM events ORDER BY seq DESC LIMIT 1").fetchone()
                if last:
                    event = json.loads(last[0])
                    if event["action"] == "read_failure":
                        retry_at = datetime.fromisoformat(event["detail"]["retry_at"])
                        remaining = (retry_at - datetime.now(timezone.utc)).total_seconds()
                        if remaining > 0:
                            if args.cycles:
                                emit({"error": "durable read backoff active", "retry_at": retry_at.isoformat(),
                                      "automatic_intake": False})
                                return 2
                            stop.wait(min(remaining, 1))
                            continue
                delay = args.interval
                try:
                    inventory = github.list_issues()
                    target = AoRead(args.project).orchestrator(args.orchestrator) if args.project else None
                    observations = evaluate(inventory.issues, args.repo, local=local_records(workspace), store=state)
                    state.record_scan(observations, inventory.started)
                    emit(report(observations, pages=inventory.pages, orchestrator=target, mode="observation-only"))
                    delay = max(delay, inventory.poll_interval)
                    completed += 1
                    failures = 0
                except ReadFailure as exc:
                    failures += 1
                    delay = max(delay, exc.delay, min(3600, 60 * 2 ** min(failures - 1, 6)))
                    retry_at = (datetime.now(timezone.utc) + timedelta(seconds=delay)).isoformat()
                    with state.write("read_failure", {"error": str(exc), "retry_after_seconds": delay,
                                                      "retry_at": retry_at}):
                        pass
                    emit({"error": str(exc), "automatic_intake": False, "retry_after_seconds": delay})
                    if failures >= 5 or args.cycles:
                        return 2
                if args.cycles and completed >= args.cycles:
                    break
                # Short interruptible waits also keep long Retry-After delays stoppable.
                remaining = delay
                while remaining > 0 and not stop.is_set():
                    step = min(remaining, 1)
                    stop.wait(step)
                    remaining -= step
        return 0
    except (IntakeError, OSError, ValueError, TypeError, sqlite3.Error) as exc:
        emit({"error": str(exc), "automatic_intake": False, "external_writes": False})
        return 2
    finally:
        if state:
            state.close()
