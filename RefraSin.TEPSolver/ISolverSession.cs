using Microsoft.Extensions.Logging;
using RefraSin.MaterialData;
using RefraSin.ProcessModel;
using RefraSin.ProcessModel.Sintering;
using RefraSin.Storage;
using RefraSin.TEPSolver.Normalization;
using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.RootFinding;
using RefraSin.TEPSolver.StepValidators;
using RefraSin.TEPSolver.StepVectors;
using RefraSin.TEPSolver.TimeSteppers;
using Particle = RefraSin.TEPSolver.ParticleModel.Particle;

namespace RefraSin.TEPSolver;

/// <summary>
/// Interface for objects holding session data of a solution procedure.
/// </summary>
public interface ISolverSession : ISinteringConditions
{
    public int TimeStepIndex { get; }

    /// <summary>
    /// Current step width of time integration.
    /// </summary>
    public double TimeStepWidth { get; }

    public StepVector? LastStep { get; }

    /// <summary>
    /// Options for the solver.
    /// </summary>
    public ISolverOptions Options { get; }

    public IReadOnlyDictionary<Guid, IMaterial> Materials { get; }
    
    public IReadOnlyDictionary<Guid, IReadOnlyList<IMaterialInterface>> MaterialInterfaces { get; }

    public SolutionState CurrentState { get; }

    /// <summary>
    /// Factory for loggers used in the session.
    /// </summary>
    public ILogger<SinteringSolver> Logger { get; }

    /// <summary>
    /// COllection of solver routines to use.
    /// </summary>
    public ISolverRoutines Routines { get; }
    
    INorm Norm { get; }
}