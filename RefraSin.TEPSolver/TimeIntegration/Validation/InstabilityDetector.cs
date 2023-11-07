using RefraSin.TEPSolver.Exceptions;
using RefraSin.TEPSolver.TimeIntegration.StepVectors;

namespace RefraSin.TEPSolver.TimeIntegration.Validation;

public class InstabilityDetector : IStepValidator
{
    /// <inheritdoc />
    public void Validate(ISolverSession solverSession, StepVector stepVector)
    {
        foreach (var particle in solverSession.Particles.Values)
        {
            var displacements = particle.Nodes.Select(n => stepVector[n].NormalDisplacement).ToArray();
            var differences = displacements.Zip(displacements.Skip(1).Append(displacements[0]), (current, next) => next - current).ToArray();

            for (int i = 0; i < differences.Length; i++)
            {
                if (
                    differences[i] * differences[(i + 1) % differences.Length] < 0 &&
                    differences[(i + 1) % differences.Length] * differences[(i + 2) % differences.Length] < 0 &&
                    differences[(i + 2) % differences.Length] * differences[(i + 3) % differences.Length] < 0
                )
                    throw new InstabilityException(particle.Id, particle.Nodes[i].Id, i);
            }
        }
    }
}