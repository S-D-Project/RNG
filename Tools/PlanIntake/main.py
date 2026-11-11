"""Run from any directory: python <workspace>/Tools/PlanIntake/main.py --help."""

import sys

sys.dont_write_bytecode = True

from rng_intake.cli import main

if __name__ == "__main__":
    raise SystemExit(main())
