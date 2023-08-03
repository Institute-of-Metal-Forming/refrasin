using System.Collections.Generic;
using System.Linq;
using OxyPlot;
using OxyPlot.Series;
using RefraSin.Coordinates;

namespace RefraSin.Plotting
{
    /// <summary>
    /// Provides common extensions methods for plotting.
    /// </summary>
    public static class CommonExtensions
    {
        /// <summary>
        /// Converts a IMF.Coordinates <see cref="IPoint"/> to a OxyPlot <see cref="DataPoint"/>.
        /// </summary>
        /// <param name="self"></param>
        public static DataPoint ToDataPoint(this IPoint self)
        {
            var absolute = self.Absolute;
            return new DataPoint(absolute.X, absolute.Y);
        }

        /// <summary>
        /// Converts IMF.Coordinates <see cref="IPoint"/> sequences to OxyPlot <see cref="DataPoint"/> sequences.
        /// </summary>
        /// <param name="self"></param>
        public static IEnumerable<DataPoint> ToDataPoints(this IEnumerable<IPoint> self) => self.Select(ToDataPoint);
        
        /// <summary>
        /// Converts a IMF.Coordinates <see cref="IPoint"/> to a OxyPlot <see cref="ScatterPoint"/>.
        /// </summary>
        /// <param name="self"></param>
        public static ScatterPoint ToScatterPoint(this IPoint self)
        {
            var absolute = self.Absolute;
            return new ScatterPoint(absolute.X, absolute.Y);
        }
        
        /// <summary>
        /// Converts IMF.Coordinates <see cref="IPoint"/> sequences to OxyPlot <see cref="ScatterPoint"/> sequences.
        /// </summary>
        /// <param name="self"></param>
        public static IEnumerable<ScatterPoint> ToScatterPoints(this IEnumerable<IPoint> self) => self.Select(ToScatterPoint);
    }
}