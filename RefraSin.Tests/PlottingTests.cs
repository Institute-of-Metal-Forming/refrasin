using System.Collections.Generic;
using System.IO;
using System.Linq;
using IMF.Coordinates;
using IMF.Coordinates.Absolute;
using IMF.Coordinates.Polar;
using IMF.Enumerables;
using MathNet.Numerics;
using NUnit.Framework;
using OxyPlot;
using RefraSin.Core.Materials;
using RefraSin.Core.ParticleModel;
using RefraSin.Core.ParticleModel.States;
using RefraSin.Core.ParticleSources;
using RefraSin.Core.ParticleTreeCompactors;
using RefraSin.Core.ParticleTreeSources;
using RefraSin.Core.SinteringProcesses;
using RefraSin.Core.Solver;
using RefraSin.Core.Solver.Solution;
using RefraSin.Plotting;
using static System.Double;
using static System.Math;

namespace RefraSin.Tests
{
    public class PlottingTests
    {
        [Test]
        public void ParticlePlotTest()
        {
            var material = new Material("", 1e-4, 1, 1, 1);
            var materialCollection = new MaterialCollection(new[] {material});
            var materialInterface = new MaterialInterface(material, material, 0.5, 1);
            var materialInterfaceCollection = new MaterialInterfaceCollection(new[] {materialInterface});

            var particles = new[]
            {
                new TrigonometricParticleSource()
                {
                    BaseRadius = 1000,
                    Ovality = 0.2,
                    PeakCount = 5,
                    PeakHeight = 0.2
                },
                new TrigonometricParticleSource()
                {
                    BaseRadius = 1000,
                    Ovality = 0.2,
                    PeakCount = 5,
                    PeakHeight = 0.2,
                    RotationAngle = PI
                }
            };

            var tree = new ExplicitTreeSource()
            {
                Root = new ExplicitTreeItem()
                {
                    CenterCoordinates = new PolarPoint(0, 0),
                    ParticleSource = particles[0],
                    Children =
                    {
                        new ExplicitTreeItem()
                        {
                            CenterCoordinates = new PolarPoint(0, 2500),
                            ParticleSource = particles[1],
                        }
                    }
                }
            }.GetParticleTree(new MaterialDatabase(materialCollection, materialInterfaceCollection), _ => 100);

            var treeCompactor = new OneDirectionalParticleTreeCompactor();
            var session = new DummySession(tree, new SinteringSolverOptions()
                { DiscretizationWidth = 100});
            treeCompactor.Compact(session);

            var states = tree.Select(p => new ParticleState(p)).ToArray();

            var plotModel = FlatSpacePlotModel.Create().AddLegend();

            foreach (var state in states)
            {
                plotModel
                    .DrawSurfaceLine(state)
                    .DrawNodeCenterConjunctions(state);
            }

            // plotModel.DrawCenterCenterConjunctions(states);

            var positions = new List<IPoint>() {states[0].SurfaceNodes.First(k => k is INeckNode).AbsoluteCoordinates};

            for (int i = 0; i < 5; i++)
            {
                var x = positions[i].Absolute.X + 50 * (i.IsOdd() ? 1 : -1);
                var y = positions[i].Absolute.Y + 50;
                positions.Add(new AbsolutePoint(x, y));
            }

            // plotModel.DrawPointTrace(positions, "neck trace");

            File.WriteAllText("particle.svg", SvgExporter.ExportToString(plotModel, 1000, 1000, true));
        }
        
        private class DummySession : ISinteringSolverSession
        {
            public DummySession(Tree<Particle> particles, SinteringSolverOptions solverOptions)
            {
                Particles = particles;
                SolverOptions = solverOptions;
            }
            public double CurrentTime => NaN;
            public double StartTime => NaN;
            public double EndTime => NaN;
            public double TimeStepWidth => NaN;
            public double? TimeStepWidthOfLastStep => NaN;
            public Tree<Particle> Particles { get; }
            public SinteringSolverOptions SolverOptions { get; }

            public IReadOnlyList<TimeSeriesItem> TimeSeries => null!;
        }
    }
}