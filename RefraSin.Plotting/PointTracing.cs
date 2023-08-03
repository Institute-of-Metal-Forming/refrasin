using System.Collections.Generic;
using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Series;
using RefraSin.Coordinates;

namespace RefraSin.Plotting
{
    /// <summary>
    /// Provides <see cref="PlotModel"/> extension methods for drawing traces of points.
    /// </summary>
    public static class PointTracing
    {
        /// <summary>
        /// Draws the trace of a point in the plot model.
        /// </summary>
        /// <param name="plotModel">the plot model to draw in</param>
        /// <param name="positions">list of positions</param>
        /// <param name="title">title to show in the legend, if null, it is not shown</param>
        /// <param name="lineColor">color of the line, default: automatic</param>
        /// <returns></returns>
        public static FlatSpacePlotModel DrawPointTrace(this FlatSpacePlotModel plotModel, IReadOnlyList<IPoint> positions, string? title = null,
            OxyColor? lineColor = null)
        {
            plotModel.Series.Add(new LineSeries()
            {
                ItemsSource = positions.ToDataPoints(),
                MarkerType = MarkerType.Plus,
                MarkerStroke = OxyColors.Black,
                MarkerFill = OxyColors.Transparent,
                MarkerStrokeThickness = 1,
                Color = lineColor ?? OxyColors.Automatic,
                Title = title
            });

            var entry = positions[0].Absolute;
            plotModel.Annotations.Add(new PointAnnotation()
            {
                Shape = MarkerType.Circle,
                Stroke = OxyColors.Black,
                Fill = OxyColors.Transparent,
                StrokeThickness = 1,
                X = entry.X,
                Y = entry.Y
            });

            return plotModel;
        }
    }
}