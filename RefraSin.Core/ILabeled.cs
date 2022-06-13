namespace RefraSin.Core
{
    /// <summary>
    /// Interface for types containing a label for human identification.
    /// </summary>
    public interface ILabeled
    {
        /// <summary>
        /// Label for human identification.
        /// </summary>
        public string Label { get; }
    }
}