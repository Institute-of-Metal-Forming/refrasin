using MathNet.Numerics.LinearAlgebra;
using RefraSin.ParticleModel;
using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.StepVectors;
using static System.Math;
using static RefraSin.TEPSolver.EquationSystem.Helper;
using NeckNode = RefraSin.TEPSolver.ParticleModel.NeckNode;
using Particle = RefraSin.TEPSolver.ParticleModel.Particle;
using ParticleContact = RefraSin.TEPSolver.ParticleModel.ParticleContact;
using JacobianRow = System.Collections.Generic.IEnumerable<(int colIndex, double value)>;
using JacobianRows = System.Collections.Generic.IEnumerable<System.Collections.Generic.IEnumerable<(int colIndex, double value)>>;

namespace RefraSin.TEPSolver.EquationSystem;

public static class Jacobian
{
    public static Matrix<double> BorderBlock(
        SolutionState currentState,
        StepVector stepVector
    )
    {
        var rows = YieldBorderBlockRows(currentState, stepVector).ToArray();
        var startIndex = stepVector.StepVectorMap.BorderStart;
        var size = stepVector.StepVectorMap.BorderLength;
        return Matrix<double>.Build.SparseOfIndexed(
            size,
            size,
            rows.SelectMany((r, i) => r.Select(c => (i, c.colIndex - startIndex, c.value)))
        );
    }

    public static JacobianRows YieldBorderBlockRows(
        SolutionState currentState,
        StepVector stepVector
    ) => YieldContactsEquations(currentState.Contacts, stepVector).Append(DissipationEquality(currentState, stepVector));

    public static JacobianRows YieldContactsEquations(
        IEnumerable<ParticleContact> contacts,
        StepVector stepVector
    ) =>
        contacts.SelectMany(contact =>
            Join(
                YieldContactNodesEquations(stepVector, contact),
                YieldContactAuxiliaryDerivatives(stepVector, contact)
            )
        );

    public static JacobianRow ParticleRadialDisplacementDerivative(
        StepVector stepVector,
        ParticleContact contact
    ) =>
        contact.FromNodes.Select(node =>
            (stepVector.StepVectorMap.LambdaContactDistance(node), 1.0)
        );

    public static JacobianRow ParticleAngleDisplacementDerivative(
        StepVector stepVector,
        ParticleContact contact
    ) =>
        contact.FromNodes.Select(node =>
            (stepVector.StepVectorMap.LambdaContactDirection(node), 1.0)
        );

    public static JacobianRow ParticleRotationDerivative(
        StepVector stepVector,
        ParticleContact contact
    )
    {
        foreach (var node in contact.FromNodes)
        {
            yield return (
                stepVector.StepVectorMap.LambdaContactDistance(node),
                node.ContactedNode.Coordinates.R
              * Sin(node.ContactedNode.AngleDistanceFromContactDirection)
            );
            yield return (
                stepVector.StepVectorMap.LambdaContactDirection(node),
                -node.ContactedNode.Coordinates.R
              / contact.Distance
              * Cos(node.ContactedNode.AngleDistanceFromContactDirection)
            );
        }
    }

    public static JacobianRow StateVelocityDerivativeTangential(
        StepVector stepVector,
        ContactNodeBase node
    )
    {
        yield return (
            stepVector.StepVectorMap.LambdaContactDistance(node),
            -node.ContactDistanceGradient.Tangential
        );
        yield return (
            stepVector.StepVectorMap.LambdaContactDirection(node),
            -node.ContactDirectionGradient.Tangential
        );
        yield return (
            stepVector.StepVectorMap.LambdaDissipation(),
            -node.GibbsEnergyGradient.Tangential
        );
    }

    public static JacobianRow DissipationEquality(
        SolutionState currentState,
        StepVector stepVector
    )
    {
        foreach (var node in currentState.Nodes)
        {
            yield return (
                stepVector.StepVectorMap.NormalDisplacement(node),
                -node.GibbsEnergyGradient.Normal
            );
            yield return (
                stepVector.StepVectorMap.FluxToUpper(node),
                -2
              * node.Particle.VacancyVolumeEnergy
              * node.SurfaceDistance.ToUpper
              / node.SurfaceDiffusionCoefficient.ToUpper
              * stepVector.FluxToUpper(node)
            );

            if (node is NeckNode neckNode)
                yield return (
                    stepVector.StepVectorMap.TangentialDisplacement(neckNode),
                    -node.GibbsEnergyGradient.Tangential
                );
        }
    }

    public static JacobianRow ContactConstraintDistance(
        StepVector stepVector,
        IParticleContact contact,
        ContactNodeBase node
    )
    {
        yield return (stepVector.StepVectorMap.RadialDisplacement(contact), 1.0);
        yield return (
            stepVector.StepVectorMap.RotationDisplacement(contact),
            node.ContactedNode.Coordinates.R
          * Sin(node.ContactedNode.AngleDistanceFromContactDirection)
        );

        if (node is NeckNode)
        {
            yield return (
                stepVector.StepVectorMap.TangentialDisplacement(node),
                -node.ContactDistanceGradient.Tangential
            );
            yield return (
                stepVector.StepVectorMap.TangentialDisplacement(node.ContactedNode),
                -node.ContactedNode.ContactDistanceGradient.Tangential
            );
        }
    }

    public static JacobianRow ContactConstraintDirection(
        StepVector stepVector,
        IParticleContact contact,
        ContactNodeBase node
    )
    {
        yield return (stepVector.StepVectorMap.AngleDisplacement(contact), 1.0);
        yield return (
            stepVector.StepVectorMap.RotationDisplacement(contact),
            -node.ContactedNode.Coordinates.R
          / contact.Distance
          * Cos(node.ContactedNode.AngleDistanceFromContactDirection)
        );

        if (node is NeckNode)
        {
            yield return (
                stepVector.StepVectorMap.TangentialDisplacement(node),
                -node.ContactDirectionGradient.Tangential
            );
            yield return (
                stepVector.StepVectorMap.TangentialDisplacement(node.ContactedNode),
                -node.ContactedNode.ContactDirectionGradient.Tangential
            );
        }
    }

    public static JacobianRows YieldContactNodesEquations(
        StepVector stepVector,
        ParticleContact contact
    )
    {
        foreach (var contactNode in contact.FromNodes)
        {
            yield return ContactConstraintDistance(stepVector, contact, contactNode);
            yield return ContactConstraintDirection(stepVector, contact, contactNode);

            if (contactNode is NeckNode neckNode)
            {
                yield return StateVelocityDerivativeTangential(stepVector, neckNode);
                yield return StateVelocityDerivativeTangential(stepVector, neckNode.ContactedNode);
            }
        }
    }

    public static JacobianRows YieldContactAuxiliaryDerivatives(StepVector stepVector, ParticleContact contact)
    {
        yield return ParticleRadialDisplacementDerivative(stepVector, contact);
        yield return ParticleAngleDisplacementDerivative(stepVector, contact);
        yield return ParticleRotationDerivative(stepVector, contact);
    }

    public static Matrix<double> ParticleBlock(
        Particle particle,
        StepVector stepVector
    )
    {
        var rows = YieldParticleBlockEquations(particle, stepVector)
            .Select(r => r.ToArray())
            .ToArray();
        var (startIndex, size) = stepVector.StepVectorMap[particle];
        return Matrix<double>.Build.SparseOfIndexed(
            size,
            size,
            rows.SelectMany((r, i) => r.Select(c => (i, c.colIndex - startIndex, c.value)))
        );
    }

    public static JacobianRows YieldParticleBlockEquations(
        Particle particle,
        StepVector stepVector
    ) => Join(
        YieldNodeEquations(particle.Nodes, stepVector)
    );

    public static JacobianRows YieldNodeEquations(
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

    public static JacobianRow StateVelocityDerivativeNormal(
        StepVector stepVector,
        NodeBase node
    )
    {
        yield return (
            stepVector.StepVectorMap.LambdaVolume(node),
            node.VolumeGradient.Normal
        );
        yield return (
            stepVector.StepVectorMap.LambdaDissipation(),
            -node.GibbsEnergyGradient.Normal
        );
    }

    public static JacobianRow FluxDerivative(
        StepVector stepVector,
        NodeBase node
    )
    {
        var bilinearPreFactor =
            -2
          * node.Particle.VacancyVolumeEnergy
          * node.SurfaceDistance.ToUpper
          / node.SurfaceDiffusionCoefficient.ToUpper;

        yield return (stepVector.StepVectorMap.FluxToUpper(node), bilinearPreFactor * stepVector.LambdaDissipation());
        yield return (stepVector.StepVectorMap.LambdaDissipation(), bilinearPreFactor * stepVector.FluxToUpper(node));
        yield return (stepVector.StepVectorMap.LambdaVolume(node), -1);
        yield return (stepVector.StepVectorMap.LambdaVolume(node.Upper), 1);
    }

    public static JacobianRow RequiredConstraint(
        StepVector stepVector,
        NodeBase node
    )
    {
        yield return (
            stepVector.StepVectorMap.NormalDisplacement(node),
            node.VolumeGradient.Normal
        );
        yield return (stepVector.StepVectorMap.FluxToUpper(node), -1);
        yield return (stepVector.StepVectorMap.FluxToUpper(node.Lower), 1);
    }
}