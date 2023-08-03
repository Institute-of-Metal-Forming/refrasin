using System;
using System.Collections.Generic;

namespace RefraSin.Core.ParticleModel
{
    public interface INeck
    {
        public INeckNode LowerNeckNode { get; }
        
        public INeckNode UpperNeckNode { get; }
        
        public IReadOnlyList<IGrainBoundaryNode> GrainBoundaryNodes { get; }
        
        public Guid ParticleId { get; }
        
        public Guid ContactedParticleId { get; }
        
        public double Radius { get; }
        
        public double GrainBoundaryLength { get; }
        
        public double MeanCurvature { get; }
        
        public int Id { get; }
    }
}