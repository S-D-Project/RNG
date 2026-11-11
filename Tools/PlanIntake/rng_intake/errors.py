class IntakeError(Exception):
    """A stable operator-visible failure; never an instruction to retry a write."""


class InvalidPlan(IntakeError):
    pass


class SafetyBlock(IntakeError):
    pass


class RecoveryRequired(SafetyBlock):
    """Investigation and hold only; D4 user approval is needed for recovery."""


class AlreadyRunning(SafetyBlock):
    pass
