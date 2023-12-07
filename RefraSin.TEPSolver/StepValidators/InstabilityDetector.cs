using RefraSin.TEPSolver.Exceptions;
using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver.StepValidators;

public class InstabilityDetector : IStepValidator
{
    /// <inheritdoc />
    public void Validate(StepVector stepVector, ISolverOptions options)
    {
        foreach (var particle in stepVector.StepVectorMap.Particles)
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