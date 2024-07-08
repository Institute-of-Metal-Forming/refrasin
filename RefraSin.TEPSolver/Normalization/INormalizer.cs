using System.Xml.Schema;
using RefraSin.ProcessModel;
using RefraSin.ProcessModel.Sintering;

namespace RefraSin.TEPSolver.Normalization;

public interface INormalizer : ISolverRoutine
{
    INorm GetNorm(ISystemState referenceState, ISinteringStep sinteringStep);
}