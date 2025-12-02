using RefraSin.Coordinates.Absolute;
using RefraSin.Coordinates.Polar;
using RefraSin.MaterialData;
using RefraSin.ParticleModel;
using RefraSin.ParticleModel.Nodes;
using RefraSin.ParticleModel.Particles;
using RefraSin.ParticleModel.Pores;
using RefraSin.ParticleModel.Pores.Extensions;
using RefraSin.ParticleModel.System;
using RefraSin.ProcessModel;
using RefraSin.ProcessModel.Sintering;
using RefraSin.Vertex;

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

    public ISystemState NormalizeSystemState(ISystemState state)
    {
        var normalizedState = new SystemState(
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

        if (
            state
            is IParticleSystemWithPores<
                IParticle<IParticleNode>,
                IParticleNode,
                IPoreState<IParticleNode>
            > withPores
        )
        {
            return new SystemStateWithPores(
                normalizedState.Id,
                normalizedState.Time,
                normalizedState.Particles,
                withPores
                    .Pores.Select(p => new PoreState<IParticleNode>(
                        p.Id,
                        p.Nodes,
                        p.Porosity,
                        p.HydrostaticStress / Mass * Pow(Time, 2) * Length
                    ))
                    .ToReadOnlyVertexCollection()
            );
        }

        return normalizedState;
    }

    public IInterfaceProperties NormalizeInterfaceProperties(
        IInterfaceProperties interfaceProperties
    ) =>
        new InterfaceProperties(
            interfaceProperties.DiffusionCoefficient / Volume * Time,
            interfaceProperties.Energy / Energy * Area,
            interfaceProperties.TransferCoefficient / Length * Time
        );

    public ISubstanceProperties NormalizeSubstanceProperties(
        ISubstanceProperties substanceProperties
    ) =>
        SubstanceProperties.FromDensityAndMolarMass(
            substanceProperties.Density / Mass * Volume,
            substanceProperties.MolarMass / Mass * Substance
        );

    public IViscoElasticProperties NormalizeViscoElasticProperties(
        IViscoElasticProperties viscoElasticProperties
    ) =>
        new ViscoElasticProperties(
            viscoElasticProperties.ElasticModulus / Mass * Pow(Time, 2) * Length,
            viscoElasticProperties.ShearViscosity / Mass * Time * Length
        );

    public IParticleMaterial NormalizeMaterial(IParticleMaterial material) =>
        new ParticleMaterial(
            material.Id,
            material.Name,
            NormalizeSubstanceProperties(material.Substance),
            NormalizeInterfaceProperties(material.Surface),
            material.Interfaces.ToDictionary(
                kv => kv.Key,
                kv => NormalizeInterfaceProperties(kv.Value)
            )
        );

    public IPoreMaterial NormalizePoreMaterial(IPoreMaterial material) =>
        new PoreMaterial(
            material.Id,
            NormalizeSubstanceProperties(material.Substance),
            material.AverageParticleRadius / Length,
            material.InterfaceEnergy / Energy * Area,
            NormalizeViscoElasticProperties(material.ViscoElastic)
        );

    public ISinteringConditions NormalizeConditions(ISinteringConditions conditions) =>
        new SinteringConditions(
            conditions.Temperature / Temperature,
            conditions.Duration / Time,
            conditions.GasConstant / Energy * Substance * Temperature
        );
}
