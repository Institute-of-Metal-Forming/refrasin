using System;
using System.Collections.Generic;
using System.Linq;

namespace RefraSin.Core.ParticleModel.States
{
    public class NeckState : INeck
    {
        public NeckState(INeck template)
        {
            LowerNeckNode = new NeckNodeState(template.LowerNeckNode);
            UpperNeckNode = new NeckNodeState(template.UpperNeckNode);
            GrainBoundaryNodes = template.GrainBoundaryNodes.Select(n => new GrainBoundaryNodeState(n)).ToArray();
            ParticleId = template.ParticleId;
            ContactedParticleId = template.ContactedParticleId;
            Radius = template.Radius;
            GrainBoundaryLength = template.GrainBoundaryLength;
            Id = template.Id;
            MeanCurvature = template.MeanCurvature;
        }
        
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