using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
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
    public static Vector<double> EvaluateAt(SolutionState currentState, StepVector stepVector)
    {
        var evaluation = YieldEquations(currentState, stepVector).ToArray();

        if (evaluation.Any(x => !double.IsFinite(x)))
        {
            throw new InvalidOperationException(
                "One ore more components of the gradient evaluated to an infinite value."
            );
        }

        return new DenseVector(evaluation);
    }

    public static IEnumerable<double> YieldEquations(
        SolutionState currentState,
        StepVector stepVector
    ) =>
        Join(
            currentState.Particles.SelectMany(p => YieldParticleBlockEquations(p, stepVector)),
            YieldBorderBlockEquations(currentState, stepVector)
        );

    public static IEnumerable<double> YieldParticleBlockEquations(
        Particle particle,
        StepVector stepVector
    ) => YieldNodeEquations(particle.Nodes, stepVector);

    public static IEnumerable<double> YieldBorderBlockEquations(
        SolutionState currentState,
        StepVector stepVector
    ) => YieldLinearBorderBlockEquations(currentState, stepVector).Append(DissipationEquality(currentState, stepVector));
    
    public static IEnumerable<double> YieldLinearBorderBlockEquations(
        SolutionState currentState,
        StepVector stepVector
    ) => YieldContactsEquations(currentState.ParticleContacts, stepVector);

    public static IEnumerable<double> YieldNodeEquations(
        IEnumerable<NodeBase> nodes,
        StepVector stepVector
    )
    {
        foreach (var node in nodes)
        {
            if (node is not ContactNodeBase)
            {
                yield return StateVelocityDerivativeNormal(stepVector, node);
                yield return FluxDerivative(stepVector, node);
                yield return RequiredConstraint(stepVector, node);
            }
        }
    }

    public static IEnumerable<double> YieldContactsEquations(
        IEnumerable<ParticleContact> contacts,
        StepVector stepVector
    ) =>
        contacts.SelectMany(contact =>
            Join(
                YieldContactNodesEquations(stepVector, contact),
                YieldContactAuxiliaryDerivatives(stepVector, contact)
            )
        );

    public static IEnumerable<double> YieldContactNodesEquations(
        StepVector stepVector,
        ParticleContact contact
    )
    {
        foreach (var contactNode in contact.FromNodes)
        {
            yield return ContactConstraintDistance(stepVector, contact, contactNode);
            yield return ContactConstraintDirection(stepVector, contact, contactNode);

            yield return StateVelocityDerivativeNormal(stepVector, contactNode);
            yield return FluxDerivative(stepVector, contactNode);
            yield return RequiredConstraint(stepVector, contactNode);
            
            yield return StateVelocityDerivativeNormal(stepVector, contactNode.ContactedNode);
            yield return FluxDerivative(stepVector, contactNode.ContactedNode);
            yield return RequiredConstraint(stepVector, contactNode.ContactedNode);

            if (contactNode is NeckNode neckNode)
            {
                yield return StateVelocityDerivativeTangential(stepVector, neckNode);
                yield return StateVelocityDerivativeTangential(stepVector, neckNode.ContactedNode);
            }
        }
    }

    public static IEnumerable<double> YieldContactAuxiliaryDerivatives(
        StepVector stepVector,
        ParticleContact contact
    )
    {
        yield return ParticleRadialDisplacementDerivative(stepVector, contact);
        yield return ParticleAngleDisplacementDerivative(stepVector, contact);
    }

    public static double ParticleRadialDisplacementDerivative(
        StepVector stepVector,
        ParticleContact contact
    ) => contact.FromNodes.Sum(stepVector.LambdaContactDistance);

    public static double ParticleAngleDisplacementDerivative(
        StepVector stepVector,
        ParticleContact contact
    ) => contact.FromNodes.Sum(stepVector.LambdaContactDirection);

    public static double StateVelocityDerivativeNormal(StepVector stepVector, NodeBase node)
    {
        var gibbsTerm =
            node.GibbsEnergyGradient.Normal * (1 + stepVector.LambdaDissipation());
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

    public static double StateVelocityDerivativeTangential(StepVector stepVector, NeckNode node)
    {
        var gibbsTerm =
            node.GibbsEnergyGradient.Tangential * (1 + stepVector.LambdaDissipation());
        var requiredConstraintsTerm =
            node.VolumeGradient.Tangential * stepVector.LambdaVolume(node);
        var contactTerm =
            node.ContactDistanceGradient.Tangential * stepVector.LambdaContactDistance(node)
          + node.ContactDirectionGradient.Tangential * stepVector.LambdaContactDirection(node);

        return -gibbsTerm + requiredConstraintsTerm - contactTerm;
    }

    public static double FluxDerivative(StepVector stepVector, NodeBase node)
    {
        var dissipationTerm =
            2
          * node.Particle.VacancyVolumeEnergy
          * node.SurfaceDistance.ToUpper
          / node.SurfaceDiffusionCoefficient.ToUpper
          * stepVector.FluxToUpper(node)
          * stepVector.LambdaDissipation();
        var thisRequiredConstraintsTerm = stepVector.LambdaVolume(node);
        var upperRequiredConstraintsTerm = stepVector.LambdaVolume(node.Upper);

        return -dissipationTerm - thisRequiredConstraintsTerm + upperRequiredConstraintsTerm;
    }

    public static double RequiredConstraint(StepVector stepVector, NodeBase node)
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

    public static double DissipationEquality(SolutionState currentState, StepVector stepVector)
    {
        var dissipationNormal = currentState
            .Nodes.Select(n => -n.GibbsEnergyGradient.Normal * stepVector.NormalDisplacement(n))
            .Sum();

        var dissipationTangential = currentState
            .Nodes.OfType<NeckNode>()
            .Select(n => -n.GibbsEnergyGradient.Tangential * stepVector.TangentialDisplacement(n))
            .Sum();

        var dissipationFunction = currentState
            .Nodes.Select(n =>
                n.Particle.VacancyVolumeEnergy
              * n.SurfaceDistance.ToUpper
              * Pow(stepVector.FluxToUpper(n), 2)
              / n.SurfaceDiffusionCoefficient.ToUpper
            )
            .Sum();

        return dissipationNormal + dissipationTangential - dissipationFunction;
    }

    public static double ContactConstraintDistance(
        StepVector stepVector,
        IParticleContact contact,
        ContactNodeBase node
    ) =>
        stepVector.RadialDisplacement(contact)
      - node.ContactDistanceGradient.Normal * stepVector.NormalDisplacement(node)
      - node.ContactedNode.ContactDistanceGradient.Normal
      * stepVector.NormalDisplacement(node.ContactedNode)
      - (
            node is NeckNode
                ? node.ContactDistanceGradient.Tangential * stepVector.TangentialDisplacement(node)
                + node.ContactedNode.ContactDistanceGradient.Tangential
                * stepVector.TangentialDisplacement(node.ContactedNode)
                : 0
        );

    public static double ContactConstraintDirection(
        StepVector stepVector,
        IParticleContact contact,
        ContactNodeBase node
    ) =>
        stepVector.AngleDisplacement(contact)
      - node.ContactDirectionGradient.Normal * stepVector.NormalDisplacement(node)
      - node.ContactedNode.ContactDirectionGradient.Normal
      * stepVector.NormalDisplacement(node.ContactedNode)
      - (
            node is NeckNode
                ? node.ContactDirectionGradient.Tangential * stepVector.TangentialDisplacement(node)
                + node.ContactedNode.ContactDirectionGradient.Tangential
                * stepVector.TangentialDisplacement(node.ContactedNode)
                : 0
        );
}