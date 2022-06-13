using System;
using System.Collections.Generic;
using System.Linq;
using IMF.Coordinates;
using IMF.Coordinates.Absolute;
using IMF.Coordinates.Polar;
using MathNet.Numerics;
using NUnit.Framework;
using RefraSin.Analysis;
using RefraSin.Core.Materials;
using RefraSin.Core.ParticleModel;
using RefraSin.Core.ParticleModel.States;
using RefraSin.Core.Solver.Solution;

namespace Tests;

public class AnalysisTest
{
    public class ParticleDummy : IParticle
    {
        public ParticleDummy(PolarPoint centerCoordinates, Guid id)
        {
            CenterCoordinates = centerCoordinates;
            Id = id;
        }

        /// <inheritdoc />
        public Guid Id { get; }

        /// <inheritdoc />
        public Angle RotationAngle => 0;

        /// <inheritdoc />
        public AbsolutePoint AbsoluteCenterCoordinates => CenterCoordinates.Absolute;

        /// <inheritdoc />
        public PolarPoint CenterCoordinates { get; }

        /// <inheritdoc />
        public Material Material { get; }

        /// <inheritdoc />
        public IReadOnlyList<INode> SurfaceNodes { get; } = new List<INode>();

        /// <inheritdoc />
        public IReadOnlyList<INeck> Necks { get; } = new List<INeck>();
    }

    [Test]
    public void ShrinkageTest()
    {
        var ids = new[] { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };
        var timeSeries = new[]
        {
            new TimeSeriesItem(0, new[]
            {
                new ParticleDummy(new PolarPoint(0, 0), ids[0]),
                new ParticleDummy(new PolarPoint(3.14, 1), ids[1]),
                new ParticleDummy(new PolarPoint(2, 3), ids[2])
            }),
            new TimeSeriesItem(1, new[]
            {
                new ParticleDummy(new PolarPoint(0, 0), ids[0]),
                new ParticleDummy(new PolarPoint(3.14, 1.2), ids[1]),
                new ParticleDummy(new PolarPoint(2, 2.5), ids[2])
            }),
            new TimeSeriesItem(2, new[]
            {
                new ParticleDummy(new PolarPoint(0, 0), ids[0]),
                new ParticleDummy(new PolarPoint(3.14, 1.4), ids[1]),
                new ParticleDummy(new PolarPoint(2, 2), ids[2])
            })
        };

        {
            var splitted = timeSeries.SplitPairs();
            var shrinkages = splitted.Values.Select(ts => ts
                .ShrinkageSeries()
                .ToArray()
            ).ToArray();

            var raster = shrinkages.InnerRaster(10).ToArray();

            var interpolations = shrinkages.InterpolateAll().ToArray();
            var means = interpolations.MeanSeries(raster);
            var medians = interpolations.MedianSeries(raster);
            var mins = interpolations.MinSeries(raster);
            var maxs = interpolations.MaxSeries(raster);
            var skews = interpolations.SkewnessSeries(raster);
        }
    }
}