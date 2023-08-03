using System.Collections.Generic;
using System.Linq;
using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Series;
using RefraSin.Core;
using RefraSin.Core.ParticleModel;
using RefraSin.Core.ParticleModel.Interfaces;

namespace RefraSin.Plotting
{
    /// <summary>
    ///     Provides <see cref="PlotModel" /> extension methods for drawing particles.
    /// </summary>
    public static class ParticlePlotting
    {
        /// <summary>
        ///     Draws the surface line of a particle to the plot.
        /// </summary>
        /// <param name="plotModel">the plot model to draw in</param>
        /// <param name="particle">the particle to draw</param>
        /// <param name="lineColor">color of the line, default: automatic</param>
        /// <param name="markerColor">color of the knot markers, default: black</param>
        /// <param name="markerType">type of the knot markers, default: cross</param>
        /// <returns>chainable plot model</returns>
        public static FlatSpacePlotModel DrawSurfaceLine(
            this FlatSpacePlotModel plotModel,
            IParticle particle,
            OxyColor? lineColor = null,
            OxyColor? markerColor = null,
            MarkerType markerType = MarkerType.Cross
        )
        {
            plotModel.Series.Add(new LineSeries
            {
                ItemsSource = particle.SurfaceNodes.Append(particle.SurfaceNodes[0]).Select(k =>
                    new DataPoint(k.AbsoluteCoordinates.X, k.AbsoluteCoordinates.Y)
                ),
                MarkerType = markerType,
                MarkerStroke = markerColor ?? OxyColors.Black,
                Color = lineColor ?? OxyColors.Automatic,
                Title = $"Particle {particle.Id.ToShortString()}"
            });

            return plotModel;
        }
        
        /// <summary>
        ///     Shades the area of a particle.
        /// </summary>
        /// <param name="plotModel">the plot model to draw in</param>
        /// <param name="particle">the particle to draw</param>
        /// <param name="lineColor">color of the line, default: automatic</param>
        /// <param name="markerColor">color of the knot markers, default: black</param>
        /// <param name="markerType">type of the knot markers, default: cross</param>
        /// <returns>chainable plot model</returns>
        public static FlatSpacePlotModel ShadeParticleArea(
            this FlatSpacePlotModel plotModel,
            IParticle particle,
            OxyColor? lineColor = null,
            OxyColor? markerColor = null,
            MarkerType markerType = MarkerType.Cross
        )
        {
            plotModel.Series.Add(new AreaSeries
            {
                ItemsSource = particle.SurfaceNodes.Append(particle.SurfaceNodes[0]).Select(k =>
                    new DataPoint(k.AbsoluteCoordinates.X, k.AbsoluteCoordinates.Y)
                ),
                MarkerType = markerType,
                MarkerStroke = markerColor ?? OxyColors.Black,
                Color = lineColor ?? OxyColors.Automatic,
                Title = $"Particle {particle.Id.ToShortString()}"
            });

            return plotModel;
        }

        /// <summary>
        ///     Draws lines between the centers of all particles.
        /// </summary>
        /// <param name="plotModel">the plot model to draw in</param>
        /// <param name="particles">list of particles</param>
        /// <param name="color">color of the line, default: red</param>
        /// <param name="lineStyle">style of the line, default: solid</param>
        /// <returns>chainable plot model</returns>
        public static FlatSpacePlotModel DrawCenterCenterConjunctions(
            this FlatSpacePlotModel plotModel,
            IReadOnlyList<IParticle> particles,
            OxyColor? color = null,
            LineStyle? lineStyle = null
        )
        {
            foreach (var p1 in particles)
            foreach (var p2 in particles)
                if (p1.Id != p2.Id)
                    plotModel.Annotations.Add(new PolylineAnnotation
                    {
                        Points =
                        {
                            new DataPoint(p1.AbsoluteCenterCoordinates.X, p1.AbsoluteCenterCoordinates.Y),
                            new DataPoint(p2.AbsoluteCenterCoordinates.X, p2.AbsoluteCenterCoordinates.Y)
                        },
                        Color = color ?? OxyColors.Red,
                        LineStyle = lineStyle ?? LineStyle.Solid
                    });

            return plotModel;
        }

        /// <summary>
        ///     Marks the knot types by drawing colored markers.
        /// </summary>
        /// <param name="plotModel">the plot model to draw in</param>
        /// <param name="particle">the particle</param>
        /// <param name="markerType">type of markers, default: circle</param>
        /// <param name="surfaceKnotColor">color for surface knots, default: transparent</param>
        /// <param name="grainBoundaryKnotColor">color for grain boundary knots, default: blue</param>
        /// <param name="neckKnotColor">color for neck knots, default: red</param>
        /// <returns></returns>
        public static FlatSpacePlotModel DrawNodeTypeMarkers(
            this FlatSpacePlotModel plotModel,
            IParticle particle,
            MarkerType markerType = MarkerType.Circle,
            OxyColor? surfaceNodeColor = null,
            OxyColor? grainBoundaryNodeColor = null,
            OxyColor? neckNodeColor = null
        )
        {
            foreach (var k in particle.SurfaceNodes)
                switch (k)
                {
                    case IGrainBoundaryNode _:
                        plotModel.Annotations.Add(new PointAnnotation
                        {
                            Shape = markerType,
                            Stroke = grainBoundaryNodeColor ?? OxyColors.Blue,
                            Fill = OxyColors.Transparent,
                            StrokeThickness = 1,
                            X = k.AbsoluteCoordinates.X,
                            Y = k.AbsoluteCoordinates.Y
                        });
                        break;

                    case INeckNode _:
                        plotModel.Annotations.Add(new PointAnnotation
                        {
                            Shape = markerType,
                            Stroke = neckNodeColor ?? OxyColors.Red,
                            Fill = OxyColors.Transparent,
                            StrokeThickness = 1,
                            X = k.AbsoluteCoordinates.X,
                            Y = k.AbsoluteCoordinates.Y
                        });
                        break;

                    default:
                        plotModel.Annotations.Add(new PointAnnotation
                        {
                            Shape = markerType,
                            Stroke = surfaceNodeColor ?? OxyColors.Transparent,
                            Fill = OxyColors.Transparent,
                            StrokeThickness = 1,
                            X = k.AbsoluteCoordinates.X,
                            Y = k.AbsoluteCoordinates.Y
                        });
                        break;
                }

            return plotModel;
        }

        /// <summary>
        ///     Draws lines between the surface knots and the particles center.
        /// </summary>
        /// <param name="plotModel">the plot model to draw in</param>
        /// <param name="particle">the particle</param>
        /// <param name="color">color of the line, default: black</param>
        /// <param name="lineStyle">style of the line, default: solid</param>
        /// <returns></returns>
        public static FlatSpacePlotModel DrawNodeCenterConjunctions(
            this FlatSpacePlotModel plotModel,
            IParticle particle,
            OxyColor? color = null,
            LineStyle? lineStyle = null)
        {
            var center = particle.AbsoluteCenterCoordinates;

            foreach (var k in particle.SurfaceNodes)
                plotModel.Annotations.Add(new PolylineAnnotation
                {
                    Points =
                    {
                        new DataPoint(center.X, center.Y),
                        new DataPoint(k.AbsoluteCoordinates.X, k.AbsoluteCoordinates.Y)
                    },
                    Color = color ?? OxyColors.Black,
                    LineStyle = lineStyle ?? LineStyle.Solid
                });

            return plotModel;
        }
    }
}