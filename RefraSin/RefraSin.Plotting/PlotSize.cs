namespace RefraSin.Plotting
{
    /// <summary>
    /// Struct describing the size of an output image.
    /// </summary>
    public readonly struct PlotSize
    {
        /// <summary>
        /// Create from string with format width,height
        /// </summary>
        /// <param name="s"></param>
        public PlotSize(string s)
        {
            var split = s.Split(',');
            Width = int.Parse(split[0]);
            Height = int.Parse(split[1]);
        }
        
        /// <summary>
        /// Create with specified values.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public PlotSize(int width, int height)
        {
            Width = width;
            Height = height;
        }
        /// <summary>
        /// Width of the plot.
        /// </summary>
        public int Width { get; }
        
        /// <summary>
        /// Height of the plot.
        /// </summary>
        public int Height { get; }

        /// <inheritdoc />
        public override string ToString() => $"{Width},{Height}";
    }
}