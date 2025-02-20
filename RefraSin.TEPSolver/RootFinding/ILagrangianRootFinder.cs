using Microsoft.Extensions.Logging;
using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver.RootFinding;

public interface ILagrangianRootFinder : ISolverRoutine
{
    public StepVector FindRoot(
        EquationSystem equationSystem,
        StepVector initialGuess,
        ILogger logger
    );
}
