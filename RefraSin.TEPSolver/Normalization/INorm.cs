using RefraSin.Coordinates.Absolute;
using RefraSin.Coordinates.Polar;
using RefraSin.MaterialData;
using RefraSin.ParticleModel;
using RefraSin.ParticleModel.Nodes;
using RefraSin.ParticleModel.Particles;
using RefraSin.ProcessModel;
using RefraSin.ProcessModel.Sintering;
using static System.Double;

namespace RefraSin.TEPSolver.Normalization;

public interface INorm
{
    public double Time { get; }
    public double Mass { get; }
    public double Length { get; }
    public double Temperature { get; }
    public double Substance { get; }

    public double Area { get; }
    public double Volume { get; }
    public double Energy { get; }
    public double DiffusionCoefficient { get; }
    public double InterfaceEnergy { get; }

    public ISystemState NormalizeSystemState(ISystemState state) =>
        new SystemState(
            state.Id,
            state.Time,
            state.Particles.Select(p =>
            {
                IEnumerable<ParticleNode> NodeFactory(IParticle<IParticleNode> particle) =>
                    p.Nodes.Select(n => new ParticleNode(
                        n.Id,
                        particle,
                        new PolarPoint(n.Coordinates.Phi, n.Coordinates.R / Length, particle),
                        n.Type
                    ));

                return new Particle<ParticleNode>(
                    p.Id,
                    new AbsolutePoint(p.Coordinates.X / Length, p.Coordinates.Y / Length),
                    p.RotationAngle,
                    p.MaterialId,
                    NodeFactory
                );
            })
        );

    public IInterfaceProperties NormalizeInterfaceProperties(
        IInterfaceProperties interfaceProperties
    ) =>
        new InterfaceProperties(
            interfaceProperties.DiffusionCoefficient / DiffusionCoefficient,
            interfaceProperties.Energy / InterfaceEnergy
        );

    public IBulkProperties NormalizeBulkProperties(IBulkProperties bulkProperties) =>
        new BulkProperties(
            bulkProperties.VolumeDiffusionCoefficient / DiffusionCoefficient,
            bulkProperties.EquilibriumVacancyConcentration
        );

    public ISubstanceProperties NormalizeSubstanceProperties(
        ISubstanceProperties substanceProperties
    ) =>
        new SubstanceProperties(
            substanceProperties.Density / Mass * Volume,
            substanceProperties.MolarMass / Mass * Substance
        );

    public IMaterial NormalizeMaterial(IMaterial material) =>
        new Material(
            material.Id,
            material.Name,
            NormalizeBulkProperties(material.Bulk),
            NormalizeSubstanceProperties(material.Substance),
            NormalizeInterfaceProperties(material.Surface),
            material.Interfaces.ToDictionary(
                kv => kv.Key,
                kv => NormalizeInterfaceProperties(kv.Value)
            )
        );

    public ISinteringConditions NormalizeConditions(ISinteringConditions conditions) =>
        new SinteringConditions(
            conditions.Temperature / Temperature,
            conditions.Duration / Time,
            conditions.GasConstant / Energy * Substance * Temperature
        );
}
