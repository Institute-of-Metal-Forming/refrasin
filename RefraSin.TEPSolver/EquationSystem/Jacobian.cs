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

    private static JacobianRows YieldBorderBlockRows(
        SolutionState currentState,
        StepVector stepVector
    ) => YieldContactsEquations(currentState.Contacts, stepVector);

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

    private static JacobianRow ParticleRadialDisplacementDerivative(
        StepVector stepVector,
        ParticleContact contact
    ) =>
        contact.FromNodes.Select(node =>
            (stepVector.StepVectorMap[node, NodeUnknown.LambdaContactDistance], 1.0)
        );

    private static JacobianRow ParticleAngleDisplacementDerivative(
        StepVector stepVector,
        ParticleContact contact
    ) =>
        contact.FromNodes.Select(node =>
            (stepVector.StepVectorMap[node, NodeUnknown.LambdaContactDirection], 1.0)
        );

    private static JacobianRow ParticleRotationDerivative(
        StepVector stepVector,
        ParticleContact contact
    )
    {
        foreach (var node in contact.FromNodes)
        {
            yield return (
                stepVector.StepVectorMap[node, NodeUnknown.LambdaContactDistance],
                node.ContactedNode.Coordinates.R
              * Sin(node.ContactedNode.AngleDistanceFromContactDirection)
            );
            yield return (
                stepVector.StepVectorMap[node, NodeUnknown.LambdaContactDirection],
                -node.ContactedNode.Coordinates.R
              / contact.Distance
              * Cos(node.ContactedNode.AngleDistanceFromContactDirection)
            );
        }
    }

    public static JacobianRows YieldParticleEquations(
        Particle particle,
        StepVector stepVector
    )
    {
        yield return DissipationEquality(particle, stepVector);
    }

    private static JacobianRow StateVelocityDerivativeTangential(
        StepVector stepVector,
        ContactNodeBase node
    )
    {
        yield return (
            stepVector.StepVectorMap[node, NodeUnknown.LambdaContactDistance],
            -node.ContactDistanceGradient.Tangential
        );
        yield return (
            stepVector.StepVectorMap[node, NodeUnknown.LambdaContactDirection],
            -node.ContactDirectionGradient.Tangential
        );
    }

    private static JacobianRow DissipationEquality(
        Particle particle,
        StepVector stepVector
    )
    {
        foreach (var node in particle.Nodes)
        {
            yield return (
                stepVector.StepVectorMap[node, NodeUnknown.NormalDisplacement],
                -node.GibbsEnergyGradient.Normal
            );
            yield return (
                stepVector.StepVectorMap[node, NodeUnknown.FluxToUpper],
                -2
              * node.Particle.VacancyVolumeEnergy
              * node.SurfaceDistance.ToUpper
              / node.SurfaceDiffusionCoefficient.ToUpper
              * stepVector.FluxToUpper(node)
            );
        }
    }

    private static JacobianRow ContactConstraintDistance(
        StepVector stepVector,
        IParticleContact contact,
        ContactNodeBase node
    )
    {
        yield return (stepVector.StepVectorMap[contact, ContactUnknown.RadialDisplacement], 1.0);
        yield return (
            stepVector.StepVectorMap[contact, ContactUnknown.RotationDisplacement],
            node.ContactedNode.Coordinates.R
          * Sin(node.ContactedNode.AngleDistanceFromContactDirection)
        );

        if (node is NeckNode)
        {
            yield return (
                stepVector.StepVectorMap[node, NodeUnknown.TangentialDisplacement],
                -node.ContactDistanceGradient.Tangential
            );
            yield return (
                stepVector.StepVectorMap[node.ContactedNode, NodeUnknown.TangentialDisplacement],
                -node.ContactedNode.ContactDistanceGradient.Tangential
            );
        }
    }

    private static JacobianRow ContactConstraintDirection(
        StepVector stepVector,
        IParticleContact contact,
        ContactNodeBase node
    )
    {
        yield return (stepVector.StepVectorMap[contact, ContactUnknown.AngleDisplacement], 1.0);
        yield return (
            stepVector.StepVectorMap[contact, ContactUnknown.RotationDisplacement],
            -node.ContactedNode.Coordinates.R
          / contact.Distance
          * Cos(node.ContactedNode.AngleDistanceFromContactDirection)
        );

        if (node is NeckNode)
        {
            yield return (
                stepVector.StepVectorMap[node, NodeUnknown.TangentialDisplacement],
                -node.ContactDirectionGradient.Tangential
            );
            yield return (
                stepVector.StepVectorMap[node.ContactedNode, NodeUnknown.TangentialDisplacement],
                -node.ContactedNode.ContactDirectionGradient.Tangential
            );
        }
    }

    private static JacobianRows YieldContactNodesEquations(
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

    private static JacobianRows YieldContactAuxiliaryDerivatives(StepVector stepVector, ParticleContact contact)
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

    private static JacobianRows YieldParticleBlockEquations(
        Particle particle,
        StepVector stepVector
    ) => Join(
        YieldNodeEquations(particle.Nodes, stepVector),
        YieldParticleEquations(particle, stepVector)
    );

    private static JacobianRows YieldNodeEquations(
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

    private static JacobianRow StateVelocityDerivativeNormal(
        StepVector stepVector,
        NodeBase node
    )
    {
        yield return (
            stepVector.StepVectorMap[node, NodeUnknown.LambdaVolume],
            node.VolumeGradient.Normal
        );
        yield return (
            stepVector.StepVectorMap[node.Particle, ParticleUnknown.LambdaDissipation],
            -node.GibbsEnergyGradient.Normal
        );
    }

    private static JacobianRow FluxDerivative(
        StepVector stepVector,
        NodeBase node
    )
    {
        var bilinearPreFactor =
            -2
          * node.Particle.VacancyVolumeEnergy
          * node.SurfaceDistance.ToUpper
          / node.SurfaceDiffusionCoefficient.ToUpper;

        yield return (stepVector.StepVectorMap[node, NodeUnknown.FluxToUpper], bilinearPreFactor * stepVector.LambdaDissipation(node.Particle));
        yield return (stepVector.StepVectorMap[node.Particle, ParticleUnknown.LambdaDissipation], bilinearPreFactor * stepVector.FluxToUpper(node));
        yield return (stepVector.StepVectorMap[node, NodeUnknown.LambdaVolume], -1);
        yield return (stepVector.StepVectorMap[node.Upper, NodeUnknown.LambdaVolume], 1);
    }

    private static JacobianRow RequiredConstraint(
        StepVector stepVector,
        NodeBase node
    )
    {
        yield return (
            stepVector.StepVectorMap[node, NodeUnknown.NormalDisplacement],
            node.VolumeGradient.Normal
        );
        yield return (stepVector.StepVectorMap[node, NodeUnknown.FluxToUpper], -1);
        yield return (stepVector.StepVectorMap[node.Lower, NodeUnknown.FluxToUpper], 1);
    }
}