using System.Xml.Schema;
using RefraSin.ProcessModel;
using RefraSin.ProcessModel.Sintering;

namespace RefraSin.TEPSolver.Normalization;

public interface INormalizer
{
    INorm GetNorm(ISystemState referenceState, ISinteringStep sinteringStep);
}