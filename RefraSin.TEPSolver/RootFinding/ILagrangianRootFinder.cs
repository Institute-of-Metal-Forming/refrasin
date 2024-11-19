using Microsoft.Extensions.Logging;
using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver.RootFinding;

public interface ILagrangianRootFinder : ISolverRoutine
{
    public StepVector FindRoot(SolutionState currentState, StepVector initialGuess, ILogger logger);
}
