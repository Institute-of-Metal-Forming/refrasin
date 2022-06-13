namespace RefraSin.Core.ParticleSources
{
    /// <summary>
    /// Delegate for functions that determine the surface knot count from the particle radius.
    /// </summary>
    public delegate int SurfaceNodeCountFunction (double particleRadius);
}