using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Legends;
using static System.Double;

namespace RefraSin.Plotting
{
    /// <summary>
    /// Represents a specialized plot model showing points and shape in 2D linear scaled space.
    /// </summary>
    public class FlatSpacePlotModel : PlotModel
    {
        private FlatSpacePlotModel() : base() { }

        /// <summary>
        ///    Creates a new <see cref="FlatSpacePlotModel"/>.
        /// </summary>
        /// <param name="minX">lower boundary of plot in x-direction</param>
        /// <param name="maxX">upper boundary of plot in x-direction</param>
        /// <param name="minY">lower boundary of plot in y-direction</param>
        /// <param name="maxY">upper boundary of plot in y-direction</param>
        /// <param name="lengthUnit">unit of the coordinate axes</param>
        /// <param name="backgroundColor">color of background, default: white</param>
        /// <returns>OxyPlot <see cref="PlotModel" /></returns>
        /// <remarks>
        ///     If the plot boundary parameters (<paramref name="minX" />, <paramref name="maxX" />,<paramref name="minY" />,
        ///     <paramref name="maxY" />) are null, the boundaries are determined automatically.
        /// </remarks>
        public static FlatSpacePlotModel Create(
            double? minX = null,
            double? maxX = null,
            double? minY = null,
            double? maxY = null,
            string lengthUnit = "μm",
            OxyColor? backgroundColor = null
        )
        {
            var xAxis = new LinearAxis {Position = AxisPosition.Bottom, Title = $"x in {lengthUnit}", Minimum = minX ?? NaN, Maximum = maxX ?? NaN};
            var yAxis = new LinearAxis {Position = AxisPosition.Left, Title = $"y in {lengthUnit}", Minimum = minY ?? NaN, Maximum = maxY ?? NaN};

            var plotModel = new FlatSpacePlotModel()
            {
                PlotType = PlotType.Cartesian,
                Background = backgroundColor ?? OxyColors.White,
                Axes =
                {
                    xAxis, yAxis
                }
            };

            return plotModel;
        }

        /// <summary>
        /// Adds a legend to this PlotModel.
        /// </summary>
        public FlatSpacePlotModel AddLegend()
        {
            Legends.Add(new Legend()
            {
                
            });
            return this;
        }
    }
}