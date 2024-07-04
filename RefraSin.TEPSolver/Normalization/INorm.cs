using RefraSin.Coordinates.Absolute;
using RefraSin.Coordinates.Polar;
using RefraSin.MaterialData;
using RefraSin.ParticleModel;
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
                var center = new AbsolutePoint(p.CenterCoordinates.X / Length, p.CenterCoordinates.Y / Length);
                var coordinateSystem = new PolarCoordinateSystem(center, p.RotationAngle);

                return new Particle(
                    p.Id,
                    center,
                    p.RotationAngle,
                    p.MaterialId,
                    p.Nodes.Select(n => n switch
                    {
                        _ => new Node(
                            n.Id,
                            n.ParticleId,
                            new PolarPoint(n.Coordinates.Phi, n.Coordinates.R / Length, coordinateSystem),
                            n.Type
                        )
                    }).ToArray()
                );
            })
        );

    public ISystemState DenormalizeSystemState(ISystemState state) =>
        new SystemState(
            state.Id,
            state.Time,
            state.Particles.Select(p =>
            {
                var center = new AbsolutePoint(p.CenterCoordinates.X * Length, p.CenterCoordinates.Y * Length);
                var coordinateSystem = new PolarCoordinateSystem(center, p.RotationAngle);

                return new Particle(
                    p.Id,
                    center,
                    p.RotationAngle,
                    p.MaterialId,
                    p.Nodes.Select(n => n switch
                    {
                        _ => new Node(
                            n.Id,
                            n.ParticleId,
                            new PolarPoint(n.Coordinates.Phi, n.Coordinates.R * Length, coordinateSystem),
                            n.Type
                        )
                    }).ToArray()
                );
            })
        );

    public IInterfaceProperties NormalizeInterfaceProperties(IInterfaceProperties interfaceProperties)
        => new InterfaceProperties(interfaceProperties.DiffusionCoefficient / DiffusionCoefficient, interfaceProperties.Energy / InterfaceEnergy);

    public IBulkProperties NormalizeBulkProperties(IBulkProperties bulkProperties) =>
        new BulkProperties(bulkProperties.VolumeDiffusionCoefficient / DiffusionCoefficient, bulkProperties.EquilibriumVacancyConcentration);

    public ISubstanceProperties NormalizeSubstanceProperties(ISubstanceProperties substanceProperties) =>
        new SubstanceProperties(substanceProperties.Density / Mass * Volume, substanceProperties.MolarMass / Mass * Substance);

    public IMaterial NormalizeMaterial(IMaterial material) => new Material(
        material.Id,
        material.Name,
        NormalizeBulkProperties(material.Bulk),
        NormalizeSubstanceProperties(material.Substance),
        NormalizeInterfaceProperties(material.Surface),
        material.Interfaces.ToDictionary(kv => kv.Key, kv => NormalizeInterfaceProperties(kv.Value))
    );
}