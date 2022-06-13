namespace RefraSin.Core.ParticleModel.States
{
    /// <summary>
    /// Stellt den Zustand eines Oberflächenknotens dar.
    /// </summary>
    public class SurfaceNodeState : NodeState, ISurfaceNode
    {
        public SurfaceNodeState() : base() { }
        public SurfaceNodeState(INode template) : base(template) { }
    }
}