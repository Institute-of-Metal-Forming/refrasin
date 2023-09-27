using RefraSin.MaterialData;
using RefraSin.ParticleModel;

namespace RefraSin.ProcessModel;

public interface ISinteringProcess
{
    public double StartTime { get; }

    public double EndTime { get; }

    public IReadOnlyList<IParticleSpec> ParticleSpecs { get; }

    public IReadOnlyList<IMaterial> Materials { get; }

    public IReadOnlyList<IMaterialInterface> MaterialInterfaces { get; }
}