using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver.StepValidators;

public class InstabilityDetector : IStepValidator
{
    /// <inheritdoc />
    public void Validate(SolutionState currentState, StepVector stepVector)
    {
        foreach (var particle in currentState.Particles)
        {
            var displacements = particle.Nodes.Select(stepVector.NormalDisplacement).ToArray();
            var differences = displacements.Zip(displacements.Skip(1).Append(displacements[0]), (current, next) => next - current).ToArray();

            for (int i = 0; i < differences.Length; i++)
            {
                if (
                    differences[i] * differences[(i + 1) % differences.Length] < 0 &&
                    differences[(i + 1) % differences.Length] * differences[(i + 2) % differences.Length] < 0 &&
                    differences[(i + 2) % differences.Length] * differences[(i + 3) % differences.Length] < 0 &&
                    differences[(i + 3) % differences.Length] * differences[(i + 4) % differences.Length] < 0
                )
                    throw new InstabilityException(currentState, stepVector, particle.Id, particle.Nodes[i].Id, i);
            }
        }
    }

    /// <inheritdoc />
    public void RegisterWithSolver(SinteringSolver solver) { }
}