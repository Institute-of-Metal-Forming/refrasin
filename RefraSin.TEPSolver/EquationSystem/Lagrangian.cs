using RefraSin.ParticleModel;
using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.StepVectors;
using static System.Math;
using static RefraSin.TEPSolver.EquationSystem.Helper;
using NeckNode = RefraSin.TEPSolver.ParticleModel.NeckNode;
using Particle = RefraSin.TEPSolver.ParticleModel.Particle;
using ParticleContact = RefraSin.TEPSolver.ParticleModel.ParticleContact;

namespace RefraSin.TEPSolver.EquationSystem;

public static class Lagrangian
{
    public static StepVector EvaluateAt(
        SolutionState currentState,
        StepVector stepVector
    )
    {
        var evaluation = YieldEquations(currentState, stepVector).ToArray();

        if (evaluation.Any(x => !double.IsFinite(x)))
        {
            throw new InvalidOperationException(
                "One ore more components of the gradient evaluated to an infinite value."
            );
        }

        return new StepVector(evaluation, stepVector.StepVectorMap);
    }

    public static IEnumerable<double> YieldEquations(
        SolutionState currentState,
        StepVector stepVector
    ) =>
        Join(
            currentState.Particles.SelectMany(p => YieldParticleBlockEquations(p, stepVector)),
            YieldFunctionalBlockEquations(currentState, stepVector)
        );

    public static IEnumerable<double> YieldParticleBlockEquations(
        Particle particle,
        StepVector stepVector
    ) => Join(
        YieldNodeEquations(particle.Nodes, stepVector),
        YieldParticleEquations(particle, stepVector)
    );

    public static IEnumerable<double> YieldFunctionalBlockEquations(
        SolutionState currentState,
        StepVector stepVector
    ) => YieldContactsEquations(currentState.Contacts, stepVector);

    public static IEnumerable<double> YieldNodeEquations(
        IEnumerable<NodeBase> nodes,
        StepVector stepVector
    )
    {
        foreach (var node in nodes)
        {
            yield return StateVelocityDerivativeNormal(stepVector, node);
            yield return FluxDerivative(stepVector, node);
            yield return RequiredConstraint(stepVector, node);
        }
    }

    public static IEnumerable<double> YieldContactsEquations(
        IEnumerable<ParticleContact> contacts,
        StepVector stepVector
    ) =>
        contacts.SelectMany(contact => Join(
            YieldContactNodesEquations(stepVector, contact),
            YieldContactAuxiliaryDerivatives(stepVector, contact)
        ));

    private static IEnumerable<double> YieldContactNodesEquations(
        StepVector stepVector,
        ParticleContact contact
    )
    {
        foreach (var contactNode in contact.FromNodes)
        {
            var constraints = ContactConstraints(stepVector, contact, contactNode);
            yield return constraints.distance;
            yield return constraints.direction;

            if (contactNode is NeckNode neckNode)
            {
                yield return StateVelocityDerivativeTangential(stepVector, neckNode);
                yield return StateVelocityDerivativeTangential(stepVector, neckNode.ContactedNode);
            }
        }
    }

    private static IEnumerable<double> YieldContactAuxiliaryDerivatives(
        StepVector stepVector,
        ParticleContact contact
    )
    {
        yield return ParticleRadialDisplacementDerivative(stepVector, contact);
        yield return ParticleAngleDisplacementDerivative(stepVector, contact);
        yield return ParticleRotationDerivative(stepVector, contact);
    }

    private static double ParticleRadialDisplacementDerivative(
        StepVector stepVector,
        ParticleContact contact
    ) => contact.FromNodes.Sum(stepVector.LambdaContactDistance);

    private static double ParticleAngleDisplacementDerivative(
        StepVector stepVector,
        ParticleContact contact
    ) => contact.FromNodes.Sum(stepVector.LambdaContactDirection);

    private static double ParticleRotationDerivative(
        StepVector stepVector,
        ParticleContact contact
    ) =>
        contact.FromNodes.Sum(n =>
            n.ContactedNode.Coordinates.R
          * Sin(n.ContactedNode.AngleDistanceFromContactDirection)
          * stepVector.LambdaContactDistance(n)
          - n.ContactedNode.Coordinates.R
          / contact.Distance
          * Cos(n.ContactedNode.AngleDistanceFromContactDirection)
          * stepVector.LambdaContactDirection(n)
        );

    public static IEnumerable<double> YieldParticleEquations(
        Particle particle,
        StepVector stepVector
    )
    {
        yield return DissipationEquality(particle, stepVector);
    }

    private static double StateVelocityDerivativeNormal(
        StepVector stepVector,
        NodeBase node
    )
    {
        var gibbsTerm = node.GibbsEnergyGradient.Normal * (1 + stepVector.LambdaDissipation(node.Particle));
        var requiredConstraintsTerm = node.VolumeGradient.Normal * stepVector.LambdaVolume(node);

        double contactTerm = 0;

        if (node is ContactNodeBase contactNode)
        {
            contactTerm =
                contactNode.ContactDistanceGradient.Normal
              * stepVector.LambdaContactDistance(contactNode)
              + contactNode.ContactDirectionGradient.Normal
              * stepVector.LambdaContactDirection(contactNode);
        }

        return -gibbsTerm + requiredConstraintsTerm - contactTerm;
    }

    private static double StateVelocityDerivativeTangential(
        StepVector stepVector,
        NeckNode node
    )
    {
        var gibbsTerm = node.GibbsEnergyGradient.Tangential * (1 + stepVector.LambdaDissipation(node.Particle));
        var requiredConstraintsTerm =
            node.VolumeGradient.Tangential * stepVector.LambdaVolume(node);
        var contactTerm =
            node.ContactDistanceGradient.Tangential * stepVector.LambdaContactDistance(node)
          + node.ContactDirectionGradient.Tangential * stepVector.LambdaContactDirection(node);

        return -gibbsTerm + requiredConstraintsTerm - contactTerm;
    }

    private static double FluxDerivative(
        StepVector stepVector,
        NodeBase node
    )
    {
        var dissipationTerm =
            2
          * node.Particle.VacancyVolumeEnergy
          * node.SurfaceDistance.ToUpper
          / node.SurfaceDiffusionCoefficient.ToUpper
          * stepVector.FluxToUpper(node)
          * stepVector.LambdaDissipation(node.Particle);
        var thisRequiredConstraintsTerm = stepVector.LambdaVolume(node);
        var upperRequiredConstraintsTerm = stepVector.LambdaVolume(node.Upper);

        return -dissipationTerm - thisRequiredConstraintsTerm + upperRequiredConstraintsTerm;
    }

    private static double RequiredConstraint(
        StepVector stepVector,
        NodeBase node
    )
    {
        var normalVolumeTerm = node.VolumeGradient.Normal * stepVector.NormalDisplacement(node);
        var tangentialVolumeTerm = 0.0;

        if (node is NeckNode neckNode)
        {
            tangentialVolumeTerm =
                node.VolumeGradient.Tangential * stepVector.TangentialDisplacement(neckNode);
        }

        var fluxTerm = stepVector.FluxToUpper(node) - stepVector.FluxToUpper(node.Lower);

        return normalVolumeTerm + tangentialVolumeTerm - fluxTerm;
    }

    private static double DissipationEquality(
        Particle particle,
        StepVector stepVector
    )
    {
        var dissipationNormal = particle
            .Nodes.Select(n => -n.GibbsEnergyGradient.Normal * stepVector.NormalDisplacement(n))
            .Sum();

        var dissipationTangential = particle
            .Nodes.OfType<NeckNode>()
            .Select(n => -n.GibbsEnergyGradient.Tangential * stepVector.TangentialDisplacement(n))
            .Sum();

        var dissipationFunction =
            particle
                .Nodes.Select(n =>
                    n.Particle.VacancyVolumeEnergy
                  * n.SurfaceDistance.ToUpper
                  * Pow(stepVector.FluxToUpper(n), 2)
                  / n.SurfaceDiffusionCoefficient.ToUpper
                )
                .Sum();

        return dissipationNormal + dissipationTangential - dissipationFunction;
    }

    private static (double distance, double direction) ContactConstraints(
        StepVector stepVector,
        IParticleContact contact,
        ContactNodeBase node
    )
    {
        var normalShift =
            stepVector.NormalDisplacement(node) + stepVector.NormalDisplacement(node.ContactedNode);

        var tangentialShift = 0.0;

        if (node is NeckNode)
            tangentialShift =
                stepVector.TangentialDisplacement(node)
              + stepVector.TangentialDisplacement(node.ContactedNode);

        var distance =
            stepVector.RadialDisplacement(contact)
          - node.ContactDistanceGradient.Normal * normalShift
          - node.ContactDistanceGradient.Tangential * tangentialShift
          + node.ContactedNode.Coordinates.R
          * Sin(node.ContactedNode.AngleDistanceFromContactDirection)
          * stepVector.RotationDisplacement(contact);

        var direction =
            stepVector.AngleDisplacement(contact)
          - node.ContactDirectionGradient.Normal * normalShift
          - node.ContactDirectionGradient.Tangential * tangentialShift
          - node.ContactedNode.Coordinates.R
          / contact.Distance
          * Cos(node.ContactedNode.AngleDistanceFromContactDirection)
          * stepVector.RotationDisplacement(contact);

        return (distance, direction);
    }
}
