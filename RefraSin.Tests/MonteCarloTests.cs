using System.Collections.Generic;
using System.IO;
using System.Linq;
using IMF.Enumerables;
using IMF.Statistics.DistributedProperties;
using IMF.Statistics.Distributions;
using MathNet.Numerics.Random;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using OxyPlot;
using RefraSin.Core;
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
using static MathNet.Numerics.Constants;

namespace RefraSin.Tests
{
    public class MonteCarloTests
    {
        private Material _material1;
        private Material _material2;
        private MaterialCollection _materialCollection;
        private MaterialInterfaceCollection _materialInterfaceCollection;
        private MaterialDatabase _materialDatabase;

        [SetUp]
        public void Setup()
        {
            Configuration.UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole()));

            _material1 = new Material("1", 1e-4, 1, 1, 1);
            _material2 = new Material("2", 1e-4, 0.8, 1.2, 1);

            _materialCollection = new MaterialCollection(new[] {_material1, _material2});
            _materialInterfaceCollection = new MaterialInterfaceCollection(new[]
            {
                new MaterialInterface(_material1, _material1, 0.5, 1.1),
                new MaterialInterface(_material1, _material2, 0.8, 1.2),
                new MaterialInterface(_material2, _material1, 0.8, 1.3),
                new MaterialInterface(_material2, _material2, 0.75, 1.4)
            });
            _materialDatabase = new MaterialDatabase(_materialCollection, _materialInterfaceCollection);
        }

        [Test]
        public void ParticleSamplingTest()
        {
            Configuration.RandomSource = new MersenneTwister(42);

            var powderMixture = new PowderMixture()
            {
                Label = "TestMix",
                Fractions = new CategoricalProperty<IPowder>(
                    new List<Category<IPowder>>()
                    {
                        new(0.4, new TrigonometricPowder()
                        {
                            Label = "Powder1",
                            MaterialKey = _material1.Key,
                            BaseRadius = new NormalDistributedProperty(500, 200, 50),
                            Ovality = new UniformlyDistributedProperty(0.1, 0.3),
                            PeakCount = new CategoricalProperty<double>(new List<Category<double>>
                            {
                                new(0.5, 5),
                                new(0.3, 7),
                                new(0.2, 9)
                            }),
                            PeakHeight = new NormalDistributedProperty(0.2, 0.1, 0, 0.5)
                        }),
                        new(0.4, new TrigonometricPowder()
                        {
                            Label = "Powder2",
                            MaterialKey = _material2.Key,
                            BaseRadius = new NormalDistributedProperty(200, 100, 50),
                            Ovality = new UniformlyDistributedProperty(0.05, 0.1),
                            PeakCount = new CategoricalProperty<double>(new List<Category<double>>
                            {
                                new(0.5, 4),
                                new(0.3, 6),
                                new(0.2, 8)
                            }),
                            PeakHeight = new NormalDistributedProperty(0.1, 0.05, 0, 0.5)
                        })
                    }
                )
            };

            var treeSource = new RandomizedNecklaceTreeSource() {RingParticleCount = 10, RandomizedParticleSource = powderMixture};

            var tree = treeSource.GetParticleTree(_materialDatabase, r => (int) (0.7 * r));

            var states = tree.Select(x => new ParticleState(x)).ToArray();

            if (!Directory.Exists("out"))
                Directory.CreateDirectory("out");

            var plotModel = FlatSpacePlotModel.Create().AddLegend();

            foreach (var state in states)
            {
                plotModel
                    .DrawSurfaceLine(state)
                    .DrawNodeCenterConjunctions(state);
            }

            plotModel.DrawCenterCenterConjunctions(states);

            if (File.Exists("out/monte-carlo-test.svg")) File.Delete("out/monte-carlo-test.svg");
            using var fs = File.OpenWrite("out/monte-carlo-test.svg");
            SvgExporter.Export(plotModel, fs, 2000, 2000, true);
        }

        [Test]
        public void RealParticleSizeDistributionTest()
        {
            Configuration.RandomSource = new MersenneTwister(42);

            var points = new List<SplinePoint>()
            {
                new(0.113, 0.0),
                new(0.128, 0.01),
                new(0.145, 0.09999999999999999),
                new(0.165, 0.26999999999999996),
                new(0.188, 0.4388888888888889),
                new(0.214, 0.4566666666666667),
                new(0.243, 0.3711111111111111),
                new(0.276, 0.30000000000000004),
                new(0.313, 0.20888888888888887),
                new(0.356, 0.1388888888888889),
                new(0.405, 0.08999999999999998),
                new(0.46, 0.04333333333333333),
                new(0.523, 0.031111111111111114),
                new(0.594, 0.03666666666666666),
                new(0.675, 0.041111111111111105),
                new(0.767, 0.060000000000000005),
                new(0.871, 0.04666666666666666),
                new(0.99, 0.04),
                new(1.125, 0.02),
                new(1.278, 0.006666666666666667),
                new(1.452, 0.003333333333333333),
                new(1.65, 0.006666666666666667),
                new(1.875, 0.0),
                new(2.13, 0.0),
                new(2.42, 0.0),
                new(2.75, 0.0),
                new(3.124, 0.003333333333333333),
                new(3.55, 0.006666666666666667),
                new(4.033, 0.013333333333333334),
                new(4.583, 0.02222222222222222),
                new(5.207, 0.03444444444444445),
                new(5.916, 0.04444444444444444),
                new(6.721, 0.06222222222222223),
                new(7.636, 0.0688888888888889),
                new(8.676, 0.09777777777777777),
                new(9.858, 0.10999999999999999),
                new(11.2, 0.1377777777777778),
                new(12.72, 0.17666666666666664),
                new(14.45, 0.2411111111111111),
                new(16.42, 0.3222222222222222),
                new(18.66, 0.42333333333333334),
                new(21.2, 0.55),
                new(24.09, 0.7100000000000001),
                new(27.37, 0.8933333333333334),
                new(31.1, 1.1322222222222222),
                new(35.33, 1.4455555555555557),
                new(40.14, 1.8988888888888888),
                new(45.61, 2.58),
                new(51.82, 3.5411111111111113),
                new(58.87, 4.795555555555556),
                new(66.89, 6.248888888888889),
                new(76.0, 7.72111111111111),
                new(86.35, 9.009999999999998),
                new(98.11, 9.902222222222223),
                new(111.4, 10.217777777777776),
                new(126.6, 9.815555555555555),
                new(143.8, 8.59),
                new(163.4, 6.621111111111111),
                new(185.7, 4.73),
                new(211.0, 3.0544444444444445),
                new(239.7, 1.528888888888889),
                new(272.4, 0.46444444444444444),
                new(309.5, 0.06444444444444446),
                new(351.6, 0.0),
            };

            var powder = new TrigonometricPowder()
            {
                Label = "RealPowder",
                MaterialKey = _material1.Key,
                BaseRadius = new PdfSplineDistributedProperty(points),
                Ovality = new UniformlyDistributedProperty(0.05, 0.1),
                PeakCount = new CategoricalProperty<double>(new List<Category<double>>
                {
                    new(1, 4),
                    new(1, 5),
                    new(1, 6),
                    new(1, 7),
                    new(1, 8),
                    new(1, 9)
                }),
                PeakHeight = new NormalDistributedProperty(0.1, 0.05, 0, 0.5)
            };

            var treeSource = new RandomizedNecklaceTreeSource() {RingParticleCount = 8, RandomizedParticleSource = powder};

            var tree = treeSource.GetParticleTree(_materialDatabase, r => (int) (0.8 * r));

            var compactor = new OneDirectionalParticleTreeCompactor();
            // compactor.Compact(tree, 10);

            var states = tree.Select(x => new ParticleState(x)).ToArray();

            if (!Directory.Exists("out"))
                Directory.CreateDirectory("out");

            var plotModel = FlatSpacePlotModel.Create().AddLegend();

            foreach (var state in states)
            {
                plotModel
                    .DrawSurfaceLine(state)
                    .DrawNodeCenterConjunctions(state);
            }

            plotModel.DrawCenterCenterConjunctions(states);

            if (File.Exists("out/monte-carlo-test.svg")) File.Delete("out/monte-carlo-test-real.svg");
            using var fs = File.OpenWrite("out/monte-carlo-test-real.svg");
            SvgExporter.Export(plotModel, fs, 1000, 1000, true);
        }

        [Test]
        public void ParticlePairSamplingTest()
        {
            Configuration.RandomSource = new MersenneTwister(42);
            const double discretizationWidth = 20;

            var powderMixture = new PowderMixture()
            {
                Label = "TestMix",
                Fractions = new CategoricalProperty<IPowder>(
                    new List<Category<IPowder>>()
                    {
                        new(0.4, new TrigonometricPowder()
                        {
                            Label = "Powder1",
                            MaterialKey = _material1.Key,
                            BaseRadius = new NormalDistributedProperty(500, 200, 50),
                            Ovality = new UniformlyDistributedProperty(0.1, 0.3),
                            PeakCount = new CategoricalProperty<double>(new List<Category<double>>
                            {
                                new(0.5, 5),
                                new(0.3, 7),
                                new(0.2, 9)
                            }),
                            PeakHeight = new NormalDistributedProperty(0.2, 0.1, 0, 0.5)
                        }),
                        new(0.4, new TrigonometricPowder()
                        {
                            Label = "Powder2",
                            MaterialKey = _material2.Key,
                            BaseRadius = new NormalDistributedProperty(200, 100, 50),
                            Ovality = new UniformlyDistributedProperty(0.05, 0.1),
                            PeakCount = new CategoricalProperty<double>(new List<Category<double>>
                            {
                                new(0.5, 4),
                                new(0.3, 6),
                                new(0.2, 8)
                            }),
                            PeakHeight = new NormalDistributedProperty(0.1, 0.05, 0, 0.5)
                        })
                    }
                )
            };

            var treeSource = new RandomizedPairTreeSource() {RandomizedParticleSource = powderMixture};
            var treeCompactor = new OneDirectionalParticleTreeCompactor();

            for (int i = 0; i < 10; i++)
            {
                var tree = treeSource.GetParticleTree(_materialDatabase, r => (int) (Pi2 * r / discretizationWidth));

                var session = new DummySession(tree, new SinteringSolverOptions()
                    { DiscretizationWidth = discretizationWidth});
                treeCompactor.Compact(session);

                var states = tree.Select(x => new ParticleState(x)).ToArray();

                if (!Directory.Exists("out"))
                    Directory.CreateDirectory("out");

                var plotModel = FlatSpacePlotModel.Create().DrawNodeTypeMarkers(tree.Root);

                foreach (var state in states)
                {
                    plotModel
                        .DrawSurfaceLine(state)
                        .DrawNodeCenterConjunctions(state);
                }

                plotModel.DrawCenterCenterConjunctions(states);

                var fileName = $"out/monte-carlo-pair{i}.svg";
                if (File.Exists(fileName)) File.Delete(fileName);
                using var fs = File.OpenWrite(fileName);
                SvgExporter.Export(plotModel, fs, 500, 500, true);
            }
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