from dataclasses import dataclass
from datetime import datetime, timezone
from email.utils import parsedate_to_datetime
import json
from pathlib import Path
import re
import subprocess
import time
from urllib.parse import urlparse

from .errors import IntakeError, SafetyBlock
from .gate import Capabilities


GITHUB_VERSION = "2026-03-10"
CAS_REASON = "github_conditional_issue_patch_unsupported"
AO_REASON = "ao_effect_contract_unproven"


class ReadFailure(IntakeError):
    def __init__(self, message, delay=60):
        super().__init__(message)
        self.delay = max(60, delay)


def run(args, timeout=45):
    try:
        return subprocess.run(args, capture_output=True, encoding="utf-8", errors="strict",
                              timeout=timeout, check=False, shell=False)
    except (OSError, subprocess.TimeoutExpired, UnicodeError) as exc:
        raise ReadFailure("external read unavailable: " + args[0]) from exc


def retry_delay(headers):
    try:
        delay = float(headers.get("retry-after", "60"))
    except ValueError:
        try:
            delay = parsedate_to_datetime(headers["retry-after"]).timestamp() - time.time()
        except (ValueError, TypeError, KeyError):
            delay = 60
    if headers.get("x-ratelimit-remaining") == "0":
        try:
            delay = max(delay, float(headers["x-ratelimit-reset"]) - time.time())
        except (KeyError, ValueError):
            pass
    return max(60, delay)


def parse_response(output):
    head, separator, body = output.replace("\r\n", "\n").partition("\n\n")
    lines = head.splitlines()
    match = re.match(r"HTTP/\S+\s+(\d{3})", lines[0] if lines else "")
    if not separator or not match:
        raise ReadFailure("gh response has no HTTP status/headers")
    headers = {}
    for line in lines[1:]:
        if ":" in line:
            key, value = line.split(":", 1)
            headers[key.lower()] = value.strip()
    status = int(match[1])
    if status != 200:
        raise ReadFailure(f"GitHub read HTTP {status}", retry_delay(headers))
    try:
        return json.loads(body), headers
    except ValueError as exc:
        raise ReadFailure("GitHub read returned invalid JSON") from exc


@dataclass(frozen=True)
class Inventory:
    issues: list
    started: str
    pages: int
    poll_interval: float


class GitHubRead:
    """Only GET is implemented; there is intentionally no issue write method."""

    capabilities = Capabilities()

    def __init__(self, repo, runner=run):
        if not re.fullmatch(r"[A-Za-z0-9_.-]+/[A-Za-z0-9_.-]+", repo):
            raise SafetyBlock("repository must be owner/name")
        self.repo, self.runner = repo, runner

    def list_issues(self):
        started = datetime.now(timezone.utc).isoformat(timespec="microseconds")
        endpoint = f"repos/{self.repo}/issues?state=all&sort=created&direction=asc&per_page=100"
        seen, raw, pages, interval = set(), [], 0, 60.0
        while endpoint:
            if endpoint in seen or pages >= 10000:
                raise ReadFailure("pagination cycle/limit; inventory is incomplete")
            seen.add(endpoint)
            result = self.runner(["gh", "api", "--method", "GET", "--include",
                                  "-H", "Accept: application/vnd.github+json",
                                  "-H", f"X-GitHub-Api-Version: {GITHUB_VERSION}", endpoint])
            values, headers = parse_response(result.stdout)
            if result.returncode or not isinstance(values, list) or not all(isinstance(x, dict) for x in values):
                raise ReadFailure("invalid issue page; inventory is incomplete")
            try:
                interval = max(interval, float(headers.get("x-poll-interval", "60")))
            except ValueError as exc:
                raise ReadFailure("invalid x-poll-interval header") from exc
            if not 0 < interval < 86401:
                raise ReadFailure("invalid server poll interval")
            raw.extend(values)
            pages += 1
            links = re.findall(r'<([^>]+)>;\s*rel="([^"]+)"', headers.get("link", ""))
            next_links = [url for url, rel in links if rel == "next"]
            if len(next_links) > 1:
                raise ReadFailure("ambiguous next page")
            endpoint = ""
            if next_links:
                target = urlparse(next_links[0])
                if (target.scheme != "https" or target.netloc != "api.github.com"
                        or target.path != f"/repos/{self.repo}/issues" or target.fragment):
                    raise ReadFailure("unexpected pagination target")
                endpoint = target.path.lstrip("/") + ("?" + target.query if target.query else "")
        return Inventory(raw, started, pages, interval)


class AoRead:
    """Inspect installed contracts; never send/spawn through an unproven adapter."""

    capabilities = Capabilities()

    def __init__(self, project, runner=run):
        if not project or project.startswith("-"):
            raise SafetyBlock("AO project ID required")
        self.project, self.runner = project, runner

    def sessions(self):
        result = self.runner(["ao", "session", "ls", "--all", "--include-terminated",
                              "--project", self.project, "--json"])
        try:
            data = json.loads(result.stdout)["data"]
            if result.returncode or not isinstance(data, list) or not all(isinstance(s, dict) for s in data):
                raise ValueError("unexpected envelope")
            return data
        except (ValueError, KeyError, TypeError) as exc:
            raise ReadFailure("AO session inventory unavailable/incomplete") from exc

    def orchestrator(self, preferred=None):
        sessions = self.sessions()
        active = [s for s in sessions if s.get("projectId") == self.project
                  and s.get("role", s.get("kind")) == "orchestrator"
                  and not s.get("isTerminated", False)
                  and s.get("status") in ("working", "idle", "ready", "waiting", "needs_input")
                  and isinstance(s.get("id"), str)]
        if preferred:
            active = [s for s in active if s["id"] == preferred]
        if len(active) != 1:
            raise SafetyBlock("exactly one verified existing orchestrator required")
        return active[0]["id"]

    def validate_orchestrator(self, target, project):
        if project != self.project or self.orchestrator(target) != target:
            raise SafetyBlock("orchestrator identity changed")

    def current(self, *args):
        raise SafetyBlock("live controlled-issue proof is unavailable")

    def claim(self, *args):
        raise SafetyBlock(CAS_REASON)

    def send(self, *args):
        raise SafetyBlock(AO_REASON)

    def spawn(self, *args):
        raise SafetyBlock(AO_REASON)

    def lookup(self, *args):
        raise SafetyBlock("live worker worktree/HEAD/correlation proof is unverified")

    def delivery_receipts(self, *args):
        raise SafetyBlock("ordinary send receipt recovery is unverified")


def worktree_roots(workspace, runner=run):
    result = runner(["git", "-C", str(workspace), "worktree", "list", "--porcelain"])
    roots = [Path(line[len("worktree "):]).resolve() for line in result.stdout.splitlines()
             if line.startswith("worktree ")]
    if result.returncode or not roots:
        raise ReadFailure("cannot determine registered worktree roots")
    return roots
