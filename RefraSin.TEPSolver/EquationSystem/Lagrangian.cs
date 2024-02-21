using RefraSin.Coordinates;
using RefraSin.ParticleModel;
using RefraSin.ProcessModel;
using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.StepVectors;
using static System.Math;
using static RefraSin.Coordinates.Constants;
using static RefraSin.TEPSolver.EquationSystem.Helper;
using Particle = RefraSin.TEPSolver.ParticleModel.Particle;
using ParticleContact = RefraSin.TEPSolver.ParticleModel.ParticleContact;

namespace RefraSin.TEPSolver.EquationSystem;

public static class Lagrangian
{
    public static StepVector EvaluateAt(
        IProcessConditions conditions,
        SolutionState currentState,
        StepVector stepVector
    )
    {
        var evaluation = YieldEquations(conditions, currentState, stepVector).ToArray();

        if (evaluation.Any(x => !double.IsFinite(x)))
        {
            throw new InvalidOperationException(
                "One ore more components of the gradient evaluated to an infinite value."
            );
        }

        return new StepVector(evaluation, stepVector.StepVectorMap);
    }

    public static IEnumerable<double> YieldEquations(
        IProcessConditions conditions,
        SolutionState currentState,
        StepVector stepVector
    ) =>
        Join(
            YieldNodeEquations(conditions, currentState.Nodes, stepVector),
            YieldContactsEquations(conditions, currentState.Contacts, stepVector),
            YieldGlobalEquations(conditions, currentState, stepVector)
        );

    public static IEnumerable<double> YieldParticleBlockEquations(
        IProcessConditions conditions,
        Particle particle,
        StepVector stepVector
    ) => YieldNodeEquations(conditions, particle.Nodes, stepVector);

    public static IEnumerable<double> YieldFunctionalBlockEquations(
        IProcessConditions conditions,
        SolutionState currentState,
        StepVector stepVector
    ) =>
        Join(
            YieldContactsEquations(conditions, currentState.Contacts, stepVector),
            YieldGlobalEquations(conditions, currentState, stepVector)
        );

    public static IEnumerable<double> YieldNodeEquations(
        IProcessConditions conditions,
        IEnumerable<NodeBase> nodes,
        StepVector stepVector
    )
    {
        foreach (var node in nodes)
        {
            yield return StateVelocityDerivativeNormal(conditions, stepVector, node);
            yield return FluxDerivative(conditions, stepVector, node);
            yield return RequiredConstraint(conditions, stepVector, node);
        }
    }

    public static IEnumerable<double> YieldContactsEquations(
        IProcessConditions conditions,
        IEnumerable<ParticleContact> contacts,
        StepVector stepVector
    ) =>
        contacts.SelectMany(contact =>
        {
            var involvedNodes = contact
                .From.Nodes.OfType<ContactNodeBase>()
                .Where(n => n.ContactedParticleId == contact.To.Id)
                .ToArray();

            return Join(
                YieldContactNodesEquations(conditions, stepVector, involvedNodes, contact),
                YieldContactAuxiliaryDerivatives(stepVector, involvedNodes, contact)
            );
        });

    private static IEnumerable<double> YieldContactNodesEquations(
        IProcessConditions conditions,
        StepVector stepVector,
        IEnumerable<ContactNodeBase> involvedNodes,
        IParticleContact contact
    )
    {
        foreach (var contactNode in involvedNodes)
        {
            yield return StateVelocityDerivativeTangential(conditions, stepVector, contactNode);
            yield return StateVelocityDerivativeTangential(
                conditions,
                stepVector,
                contactNode.ContactedNode
            );

            var constraints = ContactConstraints(conditions, stepVector, contact, contactNode);
            yield return constraints.distance;
            yield return constraints.direction;

            // lambdas of contact must be equal for both connected nodes
            yield return stepVector.LambdaContactDistance(contactNode)
                - stepVector.LambdaContactDistance(contactNode.ContactedNode);
            yield return stepVector.LambdaContactDirection(contactNode)
                - stepVector.LambdaContactDirection(contactNode.ContactedNode);
        }
    }

    private static IEnumerable<double> YieldContactAuxiliaryDerivatives(
        StepVector stepVector,
        IList<ContactNodeBase> involvedNodes,
        IParticleContact contact
    )
    {
        yield return ParticleRadialDisplacementDerivative(stepVector, involvedNodes);
        yield return ParticleAngleDisplacementDerivative(stepVector, involvedNodes);
        yield return ParticleRotationDerivative(stepVector, involvedNodes, contact);
    }

    private static double ParticleRadialDisplacementDerivative(
        StepVector stepVector,
        IEnumerable<ContactNodeBase> involvedNodes
    ) => involvedNodes.Sum(stepVector.LambdaContactDistance);

    private static double ParticleAngleDisplacementDerivative(
        StepVector stepVector,
        IEnumerable<ContactNodeBase> involvedNodes
    ) => involvedNodes.Sum(stepVector.LambdaContactDirection);

    private static double ParticleRotationDerivative(
        StepVector stepVector,
        IEnumerable<ContactNodeBase> involvedNodes,
        IParticleContact contact
    ) =>
        involvedNodes.Sum(n =>
        {
            var angleDifference = (
                n.ContactedNode.Coordinates.Phi - n.ContactedNode.ContactDirection
            ).Reduce(Angle.ReductionDomain.WithNegative);
            return -n.ContactedNode.Coordinates.R
                    * Sin(stepVector.RotationDisplacement(contact) + angleDifference)
                    * stepVector.LambdaContactDistance(n)
                + n.ContactedNode.Coordinates.R
                    / contact.Distance
                    * Cos(stepVector.RotationDisplacement(contact) + angleDifference)
                    * stepVector.LambdaContactDirection(n);
        });

    public static IEnumerable<double> YieldGlobalEquations(
        IProcessConditions conditions,
        SolutionState currentState,
        StepVector stepVector
    )
    {
        yield return DissipationEquality(conditions, currentState, stepVector);
    }

    private static double StateVelocityDerivativeNormal(
        IProcessConditions conditions,
        StepVector stepVector,
        NodeBase node
    )
    {
        var gibbsTerm = -node.GibbsEnergyGradient.Normal * (1 + stepVector.Lambda1);
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

        return gibbsTerm + requiredConstraintsTerm - contactTerm;
    }

    private static double StateVelocityDerivativeTangential(
        IProcessConditions conditions,
        StepVector stepVector,
        ContactNodeBase node
    )
    {
        var gibbsTerm = -node.GibbsEnergyGradient.Tangential * (1 + stepVector.Lambda1);
        var requiredConstraintsTerm =
            node.VolumeGradient.Tangential * stepVector.LambdaVolume(node);
        var contactTerm =
            node.ContactDistanceGradient.Tangential * stepVector.LambdaContactDistance(node)
            + node.ContactDirectionGradient.Tangential * stepVector.LambdaContactDirection(node);

        return gibbsTerm + requiredConstraintsTerm - contactTerm;
    }

    private static double FluxDerivative(
        IProcessConditions conditions,
        StepVector stepVector,
        NodeBase node
    )
    {
        var dissipationTerm =
            2
            * conditions.GasConstant
            * conditions.Temperature
            / (
                node.Particle.Material.MolarVolume
                * node.Particle.Material.EquilibriumVacancyConcentration
            )
            * node.SurfaceDistance.ToUpper
            * stepVector.FluxToUpper(node)
            / node.SurfaceDiffusionCoefficient.ToUpper
            * stepVector.Lambda1;
        var thisRequiredConstraintsTerm = stepVector.LambdaVolume(node);
        var upperRequiredConstraintsTerm = stepVector.LambdaVolume(node.Upper);

        return -dissipationTerm - thisRequiredConstraintsTerm + upperRequiredConstraintsTerm;
    }

    private static double RequiredConstraint(
        IProcessConditions conditions,
        StepVector stepVector,
        NodeBase node
    )
    {
        var normalVolumeTerm = node.VolumeGradient.Normal * stepVector.NormalDisplacement(node);
        var tangentialVolumeTerm = 0.0;

        if (node is ContactNodeBase contactNode)
        {
            tangentialVolumeTerm =
                node.VolumeGradient.Tangential * stepVector.TangentialDisplacement(contactNode);
        }

        var fluxTerm = stepVector.FluxToUpper(node) - stepVector.FluxToUpper(node.Lower);

        return normalVolumeTerm + tangentialVolumeTerm - fluxTerm;
    }

    private static double DissipationEquality(
        IProcessConditions conditions,
        SolutionState currentState,
        StepVector stepVector
    )
    {
        var dissipationNormal = currentState
            .Nodes.Select(n => -n.GibbsEnergyGradient.Normal * stepVector.NormalDisplacement(n))
            .Sum();

        var dissipationTangential = currentState
            .Nodes.OfType<ContactNodeBase>()
            .Select(n => -n.GibbsEnergyGradient.Tangential * stepVector.TangentialDisplacement(n))
            .Sum();

        var dissipationFunction =
            conditions.GasConstant
            * conditions.Temperature
            * currentState
                .Nodes.Select(n =>
                    n.SurfaceDistance.ToUpper
                    * Pow(stepVector.FluxToUpper(n), 2)
                    / n.SurfaceDiffusionCoefficient.ToUpper
                    / (
                        n.Particle.Material.MolarVolume
                        * n.Particle.Material.EquilibriumVacancyConcentration
                    )
                )
                .Sum();

        return dissipationNormal + dissipationTangential - dissipationFunction;
    }

    private static (double distance, double direction) ContactConstraints(
        IProcessConditions conditions,
        StepVector stepVector,
        IParticleContact contact,
        ContactNodeBase node
    )
    {
        var normalShift =
            stepVector.NormalDisplacement(node) + stepVector.NormalDisplacement(node.ContactedNode);

        var tangentialShift =
            stepVector.TangentialDisplacement(node)
            + stepVector.TangentialDisplacement(node.ContactedNode);

        var rotationShift =
            2
            * node.ContactedNode.Coordinates.R
            * Sin(stepVector.RotationDisplacement(contact) / 2);

        var rotationDirection =
            -(node.ContactedNode.Coordinates.Phi - node.ContactedNode.ContactDirection)
            + (Pi - stepVector.RotationDisplacement(contact)) / 2;

        var distance =
            stepVector.RadialDisplacement(contact)
            - node.ContactDistanceGradient.Normal * normalShift
            - node.ContactDistanceGradient.Tangential * tangentialShift
            + Cos(rotationDirection) * rotationShift;

        var direction =
            stepVector.AngleDisplacement(contact)
            - node.ContactDirectionGradient.Normal * normalShift
            - node.ContactDirectionGradient.Tangential * tangentialShift
            - Sin(rotationDirection) / contact.Distance * rotationShift;

        return (distance, direction);
    }

}
