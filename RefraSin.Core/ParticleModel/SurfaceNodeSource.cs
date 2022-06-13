using System.Collections.Generic;

namespace RefraSin.Core.ParticleModel
{
    /// <summary>
    /// Delegate that serves a sequence of surface knots for the specified particle <paramref name="parent"/>.
    /// </summary>
    /// <param name="parent">particle to which the knots should belong to</param>
    public delegate IEnumerable<SurfaceNode> SurfaceNodeSource(Particle parent);
}