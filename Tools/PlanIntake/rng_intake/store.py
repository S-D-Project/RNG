from contextlib import contextmanager
from datetime import datetime, timezone
import hashlib
import json
import os
from pathlib import Path
import sqlite3
import uuid

from .errors import AlreadyRunning, RecoveryRequired, SafetyBlock
from .metadata import new_plan_id, stable_json


def now() -> str:
    return datetime.now(timezone.utc).isoformat(timespec="microseconds")


def outside(path: Path, workspaces):
    resolved = path.resolve()
    if not path.is_absolute() or any(resolved.is_relative_to(Path(w).resolve()) for w in workspaces):
        raise SafetyBlock("state and bindings must be absolute paths outside every worktree")
    return resolved


class FileLock:
    """Kernel-owned lock, never deleted or recovered by a PID/TTL heuristic."""

    def __init__(self, path: Path):
        self.path, self.handle = path, None

    def __enter__(self):
        self.handle = self.path.open("a+b")
        try:
            self.handle.seek(0, os.SEEK_END)
            if not self.handle.tell():
                self.handle.write(b"0")
                self.handle.flush()
            self.handle.seek(0)
            if os.name == "nt":
                import msvcrt
                msvcrt.locking(self.handle.fileno(), msvcrt.LK_NBLCK, 1)
            else:
                import fcntl
                fcntl.flock(self.handle.fileno(), fcntl.LOCK_EX | fcntl.LOCK_NB)
            return self
        except OSError as exc:
            self.handle.close()
            self.handle = None
            raise AlreadyRunning("another process holds " + self.path.name) from exc

    def __exit__(self, *args):
        if self.handle:
            self.handle.seek(0)
            if os.name == "nt":
                import msvcrt
                msvcrt.locking(self.handle.fileno(), msvcrt.LK_UNLCK, 1)
            else:
                import fcntl
                fcntl.flock(self.handle.fileno(), fcntl.LOCK_UN)
            self.handle.close()
            self.handle = None


def atomic_json(path: Path, data):
    temporary = path.with_name(path.name + "." + uuid.uuid4().hex + ".tmp")
    try:
        with temporary.open("x", encoding="utf-8", newline="\n") as stream:
            stream.write(stable_json(data) + "\n")
            stream.flush()
            os.fsync(stream.fileno())
        os.replace(temporary, path)
    finally:
        if temporary.exists():
            temporary.unlink()


SCHEMA = """
CREATE TABLE identity(repo TEXT PRIMARY KEY, generation TEXT NOT NULL, schema_version INTEGER NOT NULL);
CREATE TABLE events(seq INTEGER PRIMARY KEY, payload TEXT NOT NULL, hash TEXT NOT NULL);
CREATE TABLE scans(id TEXT PRIMARY KEY, started TEXT NOT NULL, finished TEXT NOT NULL, count INTEGER NOT NULL);
CREATE TABLE observations(
  issue INTEGER PRIMARY KEY, url TEXT NOT NULL UNIQUE, first_plan_id TEXT,
  plan_id TEXT, body_hash TEXT NOT NULL, scope_hash TEXT, status TEXT,
  classification TEXT NOT NULL, scan_id TEXT NOT NULL REFERENCES scans(id));
CREATE TABLE initializations(issue INTEGER PRIMARY KEY, plan_id TEXT NOT NULL UNIQUE, source TEXT NOT NULL);
CREATE TABLE intakes(
  plan_id TEXT PRIMARY KEY, issue INTEGER NOT NULL UNIQUE, source TEXT NOT NULL UNIQUE,
  token TEXT NOT NULL UNIQUE, owner TEXT NOT NULL, scope_hash TEXT NOT NULL,
  body_hash TEXT NOT NULL, phase TEXT NOT NULL, detail TEXT NOT NULL DEFAULT '');
CREATE TABLE assignments(
  id TEXT PRIMARY KEY, plan_id TEXT NOT NULL REFERENCES intakes(plan_id),
  task_key TEXT NOT NULL, base_sha TEXT NOT NULL, scope_hash TEXT NOT NULL,
  UNIQUE(plan_id, task_key));
CREATE TABLE attempts(
  id TEXT PRIMARY KEY, assignment_id TEXT NOT NULL REFERENCES assignments(id),
  correlation TEXT NOT NULL UNIQUE, branch TEXT NOT NULL UNIQUE,
  phase TEXT NOT NULL, session_id TEXT, detail TEXT NOT NULL DEFAULT '');
CREATE UNIQUE INDEX one_unresolved_attempt ON attempts(assignment_id)
  WHERE phase IN ('reserved','uncertain','running');
CREATE TABLE deliveries(
  intake_token TEXT PRIMARY KEY REFERENCES intakes(token), target TEXT NOT NULL,
  correlation TEXT NOT NULL UNIQUE, phase TEXT NOT NULL, receipt TEXT);
CREATE TABLE resumptions(
  reference TEXT PRIMARY KEY, plan_id TEXT NOT NULL REFERENCES intakes(plan_id),
  body_hash TEXT NOT NULL, approved_by TEXT NOT NULL);
"""


class Store:
    """One repository per installation binding; no migration/restore/unlock command."""

    @staticmethod
    def binding_path(root, repo):
        return Path(root) / (hashlib.sha256(repo.casefold().encode()).hexdigest()[:24] + ".json")

    @classmethod
    def initialize(cls, directory, binding_root, repo, workspaces):
        directory = outside(Path(directory), workspaces)
        binding_root = outside(Path(binding_root), workspaces)
        if directory == binding_root or directory.is_relative_to(binding_root) or binding_root.is_relative_to(directory):
            raise SafetyBlock("bindings must be separate from the state backup directory")
        binding_root.mkdir(parents=True, exist_ok=True)
        binding = cls.binding_path(binding_root, repo)
        with FileLock(binding.with_suffix(".ledger.lock")):
            if binding.exists() or (directory.exists() and any(directory.iterdir())):
                raise RecoveryRequired("existing installation/partial state: D4 investigation and user approval required")
            directory.mkdir(parents=True, exist_ok=True)
            generation = uuid.uuid4().hex
            db = sqlite3.connect(directory / "state.sqlite3")
            try:
                db.executescript(SCHEMA)
                db.execute("INSERT INTO identity VALUES(?,?,1)", (repo, generation))
                db.commit()
            finally:
                db.close()
            atomic_json(directory / "identity.json", {"repo": repo, "generation": generation})
            atomic_json(binding, {"repo": repo, "generation": generation, "directory": str(directory),
                                  "revision": 0, "hash": ""})

    def __init__(self, directory, binding_root, repo, workspaces):
        self.directory = outside(Path(directory), workspaces)
        self.binding_root = outside(Path(binding_root), workspaces)
        self.binding = self.binding_path(self.binding_root, repo)
        self.repo, self.db = repo, None
        try:
            if not self.binding.is_file() or not (self.directory / "identity.json").is_file():
                raise RecoveryRequired("installation binding or state identity missing; do not initialize over it")
            for path in (self.binding, self.directory / "identity.json", self.directory / "state.sqlite3"):
                if path.is_symlink():
                    raise RecoveryRequired("state file symlinks are unsupported")
            # mode=rw is essential: losing a DB must never create an empty one.
            uri = (self.directory / "state.sqlite3").as_uri() + "?mode=rw"
            self.db = sqlite3.connect(uri, uri=True, timeout=5, isolation_level=None)
            self.db.row_factory = sqlite3.Row
            self.db.execute("PRAGMA foreign_keys=ON")
            self.db.execute("PRAGMA synchronous=FULL")
            with self.lock("ledger"):
                if self.db.execute("PRAGMA quick_check").fetchone()[0] != "ok":
                    raise RecoveryRequired("SQLite integrity failure")
                self._verify()
        except (OSError, ValueError, sqlite3.Error) as exc:
            self.close()
            raise RecoveryRequired("state cannot be opened; investigate and hold, never auto-restore") from exc
        except Exception:
            self.close()
            raise

    def close(self):
        if self.db is not None:
            self.db.close()
            self.db = None

    def lock(self, purpose):
        return FileLock(self.binding.with_suffix("." + purpose + ".lock"))

    def _verify(self):
        anchor = json.loads(self.binding.read_text(encoding="utf-8"))
        identity = json.loads((self.directory / "identity.json").read_text(encoding="utf-8"))
        if not isinstance(anchor, dict) or not isinstance(identity, dict):
            raise RecoveryRequired("invalid state identity/anchor")
        row = self.db.execute("SELECT * FROM identity").fetchall()
        if (len(row) != 1 or row[0]["schema_version"] != 1 or row[0]["repo"] != self.repo
                or identity != {"repo": self.repo, "generation": row[0]["generation"]}
                or anchor.get("repo") != self.repo or anchor.get("generation") != row[0]["generation"]
                or anchor.get("directory") != str(self.directory)):
            raise RecoveryRequired("installation/repository/path mismatch")
        revision = anchor.get("revision")
        if type(revision) is not int or revision < 0:
            raise RecoveryRequired("invalid durable anchor")
        previous = ""
        hashes = {0: ""}
        for event in self.db.execute("SELECT * FROM events ORDER BY seq"):
            if event["seq"] != len(hashes):
                raise RecoveryRequired("audit sequence gap")
            current = hashlib.sha256((previous + event["payload"]).encode()).hexdigest()
            if current != event["hash"]:
                raise RecoveryRequired("audit chain mismatch")
            hashes[event["seq"]] = current
            previous = current
        if hashes.get(revision) != anchor.get("hash"):
            raise RecoveryRequired("DB rollback or audit divergence: D4 user-approved recovery required")
        if revision != len(hashes) - 1:
            # SQL committed but the sidecar acknowledgement was interrupted.
            # Only an intact matching chain prefix can be advanced automatically.
            self._anchor(len(hashes) - 1, previous)

    def _anchor(self, revision, event_hash):
        anchor = json.loads(self.binding.read_text(encoding="utf-8"))
        anchor.update(revision=revision, hash=event_hash)
        atomic_json(self.binding, anchor)

    @contextmanager
    def write(self, action, detail):
        with self.lock("ledger"):
            self._verify()
            try:
                self.db.execute("BEGIN IMMEDIATE")
                yield self.db
                row = self.db.execute("SELECT seq, hash FROM events ORDER BY seq DESC LIMIT 1").fetchone()
                revision, previous = (row["seq"], row["hash"]) if row else (0, "")
                payload = stable_json({"action": action, "at": now(), "detail": detail})
                event_hash = hashlib.sha256((previous + payload).encode()).hexdigest()
                self.db.execute("INSERT INTO events VALUES(?,?,?)", (revision + 1, payload, event_hash))
                self.db.execute("COMMIT")
                self._anchor(revision + 1, event_hash)
            except BaseException:
                if self.db.in_transaction:
                    self.db.execute("ROLLBACK")
                # A failed acknowledgement may follow a committed transaction.
                # Let the caller stop; the next open verifies the chain prefix.
                raise

    def record_scan(self, observations, started):
        scan_id = uuid.uuid4().hex
        with self.write("scan_complete", {"scan_id": scan_id, "count": len(observations)}) as db:
            db.execute("INSERT INTO scans VALUES(?,?,?,?)", (scan_id, started, now(), len(observations)))
            for o in observations:
                old = db.execute("SELECT first_plan_id FROM observations WHERE issue=?", (o["issue"],)).fetchone()
                first = old["first_plan_id"] if old else None
                if first and o.get("plan_id") and first != o["plan_id"]:
                    o = dict(o, classification="identity_changed")
                first = first or o.get("plan_id") or None
                db.execute("""INSERT INTO observations VALUES(?,?,?,?,?,?,?,?,?)
                  ON CONFLICT(issue) DO UPDATE SET
                    first_plan_id=excluded.first_plan_id, plan_id=excluded.plan_id,
                    body_hash=excluded.body_hash, scope_hash=excluded.scope_hash,
                    status=excluded.status, classification=excluded.classification, scan_id=excluded.scan_id""",
                           (o["issue"], o["source"], first, o.get("plan_id"), o["body_hash"],
                            o.get("scope_hash"), o.get("status"), o["classification"], scan_id))
        return scan_id

    def initialized_id(self, issue, source):
        with self.write("initialization_intent", {"issue": issue, "source": source}) as db:
            existing = db.execute("SELECT * FROM initializations WHERE issue=?", (issue,)).fetchone()
            if existing:
                if existing["source"] != source:
                    raise SafetyBlock("initialization source changed")
                return existing["plan_id"]
            while True:
                candidate = new_plan_id()
                if not db.execute("""SELECT plan_id FROM intakes WHERE plan_id=?
                     UNION SELECT plan_id FROM observations WHERE plan_id=?
                     UNION SELECT plan_id FROM initializations WHERE plan_id=?""", (candidate,) * 3).fetchone():
                    break
            db.execute("INSERT INTO initializations VALUES(?,?,?)", (issue, candidate, source))
            return candidate

    def intake(self, plan_id):
        row = self.db.execute("SELECT * FROM intakes WHERE plan_id=?", (plan_id,)).fetchone()
        return dict(row) if row else None

    def intent(self, issue, plan, owner):
        token = uuid.uuid4().hex
        with self.write("intake_intent", {"plan_id": plan.plan_id, "token": token}) as db:
            if db.execute("SELECT 1 FROM intakes WHERE plan_id=? OR issue=?",
                          (plan.plan_id, issue.number)).fetchone():
                raise SafetyBlock("existing intake; reconcile it instead of accepting again")
            db.execute("INSERT INTO intakes VALUES(?,?,?,?,?,?,?,?,?)",
                       (plan.plan_id, issue.number, issue.url, token, owner, plan.scope_hash,
                        plan.body_hash, "intent", ""))
        return self.intake(plan.plan_id)

    def intake_phase(self, plan_id, phase, detail=""):
        with self.write("intake_phase", {"plan_id": plan_id, "phase": phase, "detail": detail}) as db:
            if db.execute("UPDATE intakes SET phase=?,detail=? WHERE plan_id=?",
                          (phase, detail, plan_id)).rowcount != 1:
                raise SafetyBlock("unknown intake")

    def reserve_assignment(self, plan_id, task_key, base_sha, scope_hash):
        with self.write("assignment_reserve", {"plan_id": plan_id, "task_key": task_key}) as db:
            old = db.execute("SELECT * FROM assignments WHERE plan_id=? AND task_key=?",
                             (plan_id, task_key)).fetchone()
            if old:
                if old["scope_hash"] != scope_hash:
                    raise SafetyBlock("approved scope changed; no new assignment ID")
                return dict(old)
            assignment_id = uuid.uuid4().hex
            db.execute("INSERT INTO assignments VALUES(?,?,?,?,?)",
                       (assignment_id, plan_id, task_key, base_sha, scope_hash))
            return dict(db.execute("SELECT * FROM assignments WHERE id=?", (assignment_id,)).fetchone())

    def attempt(self, assignment):
        # No replacement/retry API in this release. Uncertain attempts need D4
        # user approval and a proven external recovery contract.
        with self.write("spawn_intent", {"assignment": assignment["id"]}) as db:
            old = db.execute("SELECT * FROM attempts WHERE assignment_id=?", (assignment["id"],)).fetchone()
            if old:
                return dict(old), False
            correlation = uuid.uuid4().hex
            branch = f"feature/{assignment['plan_id']}-{assignment['task_key']}-{correlation[:12]}"
            attempt_id = uuid.uuid4().hex
            db.execute("INSERT INTO attempts VALUES(?,?,?,?,?,?,?)",
                       (attempt_id, assignment["id"], correlation, branch, "reserved", None, ""))
            return dict(db.execute("SELECT * FROM attempts WHERE id=?", (attempt_id,)).fetchone()), True

    def attempt_phase(self, attempt_id, phase, *, session_id=None, detail=""):
        with self.write("attempt_phase", {"attempt": attempt_id, "phase": phase, "session": session_id}) as db:
            if db.execute("UPDATE attempts SET phase=?,session_id=?,detail=? WHERE id=?",
                          (phase, session_id, detail, attempt_id)).rowcount != 1:
                raise SafetyBlock("unknown attempt")

    def reserve_delivery(self, token, target):
        with self.write("delivery_intent", {"token": token, "target": target}) as db:
            old = db.execute("SELECT * FROM deliveries WHERE intake_token=?", (token,)).fetchone()
            if old:
                if old["target"] != target:
                    raise RecoveryRequired("handoff target changed; investigate existing delivery")
                return dict(old), False
            correlation = uuid.uuid4().hex
            db.execute("INSERT INTO deliveries VALUES(?,?,?,?,?)",
                       (token, target, correlation, "intent", None))
            return dict(db.execute("SELECT * FROM deliveries WHERE intake_token=?", (token,)).fetchone()), True

    def delivery_phase(self, token, phase, receipt=None):
        with self.write("delivery_phase", {"token": token, "phase": phase}) as db:
            db.execute("UPDATE deliveries SET phase=?,receipt=? WHERE intake_token=?", (phase, receipt, token))

    def resume_intent(self, record, evidence):
        with self.write("resume_intent", {"plan_id": record["plan_id"], "reference": evidence.reference}) as db:
            if db.execute("SELECT 1 FROM resumptions WHERE reference=?", (evidence.reference,)).fetchone():
                raise SafetyBlock("resume evidence was already consumed; no replay")
            db.execute("INSERT INTO resumptions VALUES(?,?,?,?)",
                       (evidence.reference, record["plan_id"], evidence.body_hash, evidence.approved_by))
            db.execute("UPDATE intakes SET phase='resume_intent',body_hash=? WHERE plan_id=?",
                       (evidence.body_hash, record["plan_id"]))
