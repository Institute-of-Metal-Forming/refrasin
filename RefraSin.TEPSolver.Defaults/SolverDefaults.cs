using RefraSin.TEPSolver.RootFinding;
using RefraSin.TEPSolver.StepValidators;
using RefraSin.TEPSolver.TimeSteppers;

namespace RefraSin.TEPSolver;

public static class SolverDefaults
{
    public static ISolverRoutines Routines = new SolverRoutines(
        null,
        null,
        new AdamsMoultonTimeStepper(),
        new[]
        {
            new InstabilityDetector()
        },
        new BroydenRootFinder()
    );

    public static ISolverOptions Options = new SolverOptions();
}