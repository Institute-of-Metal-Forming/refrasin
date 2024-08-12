using Plotly.NET;
using RefraSin.Compaction.ProcessModel;
using RefraSin.Coordinates;
using RefraSin.Coordinates.Absolute;
using RefraSin.ParticleModel.Nodes;
using RefraSin.ParticleModel.ParticleFactories;
using RefraSin.ParticleModel.Particles;
using RefraSin.ParticleModel.System;
using RefraSin.ProcessModel;

namespace RefraSin.Plotting.Tests;

[TestFixture]
public class ProcessPlotTests
{
    private readonly ISystemState<IParticle<IParticleNode>, IParticleNode>[] _states;
    private readonly Guid _id1;
    private readonly Guid _id2;

    public ProcessPlotTests()
    {
        var compactor = new FocalCompactionStep(new AbsolutePoint(0, 0), 0.05);

        _id1 = Guid.NewGuid();
        _id2 = Guid.NewGuid();

        _states =
        [
            compactor.Solve(new SystemState(Guid.NewGuid(), 0, new ParticleSystem<IParticle<IParticleNode>, IParticleNode>([
                new ShapeFunctionParticleFactory(1, 0.2, 5, 0.2, Guid.NewGuid())
                        { NodeCount = 200, RotationAngle = 0, CenterCoordinates = (0, 0) }
                    .GetParticle(_id1),
                new ShapeFunctionParticleFactory(1, 0.2, 5, 0.2, Guid.NewGuid())
                        { NodeCount = 200, RotationAngle = Angle.Straight, CenterCoordinates = (2.35, 0) }
                    .GetParticle(_id2),
            ]))),
            compactor.Solve(new SystemState(Guid.NewGuid(), 1, new ParticleSystem<IParticle<IParticleNode>, IParticleNode>([
                new ShapeFunctionParticleFactory(1, 0.2, 5, 0.2, Guid.NewGuid())
                        { NodeCount = 200, RotationAngle = 0, CenterCoordinates = (0, 0) }
                    .GetParticle(_id1),
                new ShapeFunctionParticleFactory(1, 0.2, 5, 0.2, Guid.NewGuid())
                        { NodeCount = 200, RotationAngle = Angle.Straight + 0.1, CenterCoordinates = (2.3, 0) }
                    .GetParticle(_id2),
            ]))),
            compactor.Solve(new SystemState(Guid.NewGuid(), 3, new ParticleSystem<IParticle<IParticleNode>, IParticleNode>([
                new ShapeFunctionParticleFactory(1, 0.2, 5, 0.2, Guid.NewGuid())
                        { NodeCount = 200, RotationAngle = 0, CenterCoordinates = (0, 0) }
                    .GetParticle(_id1),
                new ShapeFunctionParticleFactory(1, 0.2, 5, 0.2, Guid.NewGuid())
                        { NodeCount = 200, RotationAngle = Angle.Straight + 0.2, CenterCoordinates = (2.2, 0) }
                    .GetParticle(_id2),
            ]))),
            compactor.Solve(new SystemState(Guid.NewGuid(), 3.5, new ParticleSystem<IParticle<IParticleNode>, IParticleNode>([
                new ShapeFunctionParticleFactory(1, 0.2, 5, 0.2, Guid.NewGuid())
                        { NodeCount = 200, RotationAngle = 0, CenterCoordinates = (0, 0) }
                    .GetParticle(_id1),
                new ShapeFunctionParticleFactory(1, 0.2, 5, 0.2, Guid.NewGuid())
                        { NodeCount = 200, RotationAngle = Angle.Straight + 0.3, CenterCoordinates = (2.1, 0) }
                    .GetParticle(_id2),
            ])))
        ];
    }

    [Test]
    public void TestPlotParticles()
    {
        foreach (var state in _states)
        {
            var plot = ParticlePlot.PlotParticles(state.Particles);
            plot.Show();
        }
    }

    [Test]
    public void TestPlotTimeSteps()
    {
        var plot = ProcessPlot.PlotTimeSteps(_states);
        plot.Show();
    }

    [Test]
    public void TestPlotParticleCenter()
    {
        var states = _states.Select(s => s.Particles[_id2]).ToArray();
        var plot = ProcessPlot.PlotParticleCenter(states);
        var start = ParticlePlot.PlotParticle(states[0]);
        var end = ParticlePlot.PlotParticle(states[^1]);
        Chart.Combine([start, plot, end]).Show();
    }

    [Test]
    public void TestPlotParticleRotationAngle()
    {
        var states = _states.Select(s => s.Particles[_id2]).ToArray();
        var center = ProcessPlot.PlotParticleCenter(states);
        var rots = ProcessPlot.PlotParticleRotation(states);
        var start = ParticlePlot.PlotParticle(states[0]);
        var end = ParticlePlot.PlotParticle(states[^1]);
        Chart.Combine([start, center, rots, end]).Show();
    }
    [Test]
    public void TestPlotParticleCenters()
    {
        var plot = ProcessPlot.PlotParticleCenters(_states);
        plot.Show();
    }

    [Test]
    public void TestPlotParticleRotationAngles()
    {
        var center = ProcessPlot.PlotParticleCenters(_states);
        var rots = ProcessPlot.PlotParticleRotations(_states);
        Chart.Combine([center, rots]).Show();
    }
}