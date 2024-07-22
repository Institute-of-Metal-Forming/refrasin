using Microsoft.Extensions.Logging;
using RefraSin.MaterialData;
using RefraSin.ProcessModel.Sintering;
using RefraSin.TEPSolver.Normalization;
using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver;

/// <summary>
/// Interface for objects holding session data of a solution procedure.
/// </summary>
public interface ISolverSession : ISinteringConditions
{
    /// <summary>
    /// Unique ID of this session.
    /// </summary>
    Guid Id { get; }
    
    public IReadOnlyDictionary<Guid, IMaterial> Materials { get; }
    
    public IReadOnlyDictionary<Guid, IReadOnlyList<IMaterialInterface>> MaterialInterfaces { get; }

    public SolutionState CurrentState { get; }
    
    public StepVector? LastStep { get; }

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