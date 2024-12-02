using RefraSin.ParticleModel.Nodes;
using RefraSin.ParticleModel.Particles;

namespace RefraSin.ProcessModel;

public interface IProcessStepSolver<in TProcessStep>
    where TProcessStep : IProcessStep
{
    ISystemState<IParticle<IParticleNode>, IParticleNode> Solve(
        TProcessStep processStep,
        ISystemState<IParticle<IParticleNode>, IParticleNode> inputState
    );
}
