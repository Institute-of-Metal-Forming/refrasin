using MathNet.Numerics.LinearAlgebra;
using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver.EquationSystem;

public class EquationSystem
{
    private readonly IReadOnlyDictionary<Guid, IEquation[]> _particleBlocks;
    private readonly IReadOnlyDictionary<(Guid, Guid), IEquation[]> _contactBlocks;
    private readonly IReadOnlyList<IEquation> _globalBlock;

    public EquationSystem(SolutionState solutionState, StepVector stepVector)
    {
        State = solutionState;
        StepVector = stepVector;

        _particleBlocks = solutionState.Particles.ToDictionary(
            p => p.Id,
            p => YieldParticleEquations(p).ToArray()
        );
        _contactBlocks = solutionState.ParticleContacts.ToDictionary(
            c => (c.From.Id, c.To.Id),
            c => YieldContactEquations(c).ToArray()
        );
        _globalBlock = YieldGlobalEquations().ToArray();
    }

    public SolutionState State { get; }

    public StepVector StepVector { get; }

    private IEnumerable<IEquation> YieldParticleEquations(Particle p)
    {
        foreach (var n in p.Nodes)
        {
            yield return new NormalDisplacementDerivative(State, n, StepVector);
            if (n is NeckNode)
                yield return new TangentialDisplacementDerivative(State, n, StepVector);
            yield return new FluxDerivative(State, n, StepVector);
            yield return new VolumeBalanceConstraint(State, n, StepVector);
        }
    }

    private IEnumerable<IEquation> YieldContactEquations(ParticleContact contact)
    {
        foreach (var n in contact.FromNodes)
        {
            yield return new ContactDistanceConstraint(State, n, StepVector);
            yield return new ContactDirectionConstraint(State, n, StepVector);
        }

        yield return new ContactDistanceDerivative(State, contact, StepVector);
        yield return new ContactDirectionDerivative(State, contact, StepVector);
    }

    private IEnumerable<IEquation> YieldGlobalEquations()
    {
        foreach (var cycle in State.ParticleCycles)
        {
            yield return new RingContactConstraintX(State, cycle, StepVector);
            yield return new RingContactConstraintY(State, cycle, StepVector);
        }
        yield return new DissipationEqualityConstraint(State, StepVector);
    }

    public IReadOnlyList<IEquation> Equations =>
        _particleBlocks
            .SelectMany(kvp => kvp.Value)
            .Concat(_contactBlocks.SelectMany(kvp => kvp.Value))
            .Concat(_globalBlock)
            .ToArray();

    public IReadOnlyList<IEquation> GlobalBlockEquations => _globalBlock;

    public IReadOnlyList<IEquation> ParticleBlockEquations(IParticle particle) =>
        _particleBlocks[particle.Id];

    public IReadOnlyList<IEquation> ContactBlockEquations(IParticleContactEdge contact) =>
        _contactBlocks[(contact.From, contact.To)];

    public Vector<double> Lagrangian() =>
        Vector<double>.Build.DenseOfEnumerable(Equations.Select(e => e.Value()));

    public Matrix<double> Jacobian() =>
        Matrix<double>.Build.SparseOfIndexed(
            StepVector.Count,
            StepVector.Count,
            Equations.SelectMany((e, i) => e.Derivative().Select(c => (i, c.Item1, c.Item2)))
        );

    public Vector<double> ParticleBlockLagrangian(IParticle particle) =>
        Vector<double>.Build.DenseOfEnumerable(
            ParticleBlockEquations(particle).Select(e => e.Value())
        );

    public Matrix<double> ParticleBlockJacobian(IParticle particle)
    {
        var equations = ParticleBlockEquations(particle);
        var block = StepVector.StepVectorMap[particle];
        var endIndex = block.start + block.length;
        return Matrix<double>.Build.SparseOfIndexed(
            equations.Count,
            block.length,
            equations.SelectMany(
                (e, i) =>
                    e.Derivative()
                        .Where(c => c.colIndex >= block.start && c.colIndex < endIndex)
                        .Select(c => (i, c.Item1 - block.start, c.Item2))
            )
        );
    }

    public Vector<double> ContactBlockLagrangian(IParticleContactEdge contact) =>
        Vector<double>.Build.DenseOfEnumerable(
            ContactBlockEquations(contact).Select(e => e.Value())
        );

    public Matrix<double> ContactBlockJacobian(IParticleContactEdge contact)
    {
        var equations = ContactBlockEquations(contact);
        var block = StepVector.StepVectorMap[contact];
        var endIndex = block.start + block.length;
        return Matrix<double>.Build.SparseOfIndexed(
            equations.Count,
            block.length,
            equations.SelectMany(
                (e, i) =>
                    e.Derivative()
                        .Where(c => c.colIndex >= block.start && c.colIndex < endIndex)
                        .Select(c => (i, c.colIndex - block.start, c.value))
            )
        );
    }

    public Vector<double> GlobalBlockLagrangian() =>
        Vector<double>.Build.DenseOfEnumerable(GlobalBlockEquations.Select(e => e.Value()));

    public Matrix<double> GlobalBlockJacobian()
    {
        var endIndex = StepVector.StepVectorMap.GlobalStart + StepVector.StepVectorMap.GlobalLength;
        return Matrix<double>.Build.SparseOfIndexed(
            GlobalBlockEquations.Count,
            GlobalBlockEquations.Count,
            GlobalBlockEquations.SelectMany(
                (e, i) =>
                    e.Derivative()
                        .Where(c =>
                            c.colIndex >= StepVector.StepVectorMap.GlobalStart
                            && c.colIndex < endIndex
                        )
                        .Select(c =>
                            (i, c.colIndex - StepVector.StepVectorMap.GlobalLength, c.value)
                        )
            )
        );
    }
}
