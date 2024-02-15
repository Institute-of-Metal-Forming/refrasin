module RefraSin.TEPSolver.EquationSystem.Lagrangian

open Microsoft.FSharp.Core
open RefraSin.Coordinates
open RefraSin.ProcessModel
open RefraSin.TEPSolver
open RefraSin.TEPSolver.ParticleModel
open RefraSin.TEPSolver.StepVectors
open System.Linq
open System

let StateVelocityDerivativeNormal (currentEstimation: StepVector) (node: NodeBase) : float =

    let gibbsTerm = -node.GibbsEnergyGradient.Normal * (1.0 + currentEstimation.Lambda1)

    let requiredConstraintsTerm =
        node.VolumeGradient.Normal * currentEstimation.LambdaVolume(node)

    let contactTerm =
        match node with
        | :? ContactNodeBase as cn ->
            cn.ContactDistanceGradient.Normal * currentEstimation.LambdaContactDistance(cn)
            + cn.ContactDirectionGradient.Normal
              * currentEstimation.LambdaContactDirection(cn)
        | _ -> 0.0

    gibbsTerm + contactTerm + requiredConstraintsTerm

let StateVelocityDerivativeTangential (currentEstimation: StepVector) (node: ContactNodeBase) : float =

    let gibbsTerm = -node.GibbsEnergyGradient.Normal * (1.0 + currentEstimation.Lambda1)

    let requiredConstraintsTerm =
        node.VolumeGradient.Normal * currentEstimation.LambdaVolume(node)

    let contactTerm =
        node.ContactDistanceGradient.Normal
        * currentEstimation.LambdaContactDistance(node)
        + node.ContactDirectionGradient.Normal
          * currentEstimation.LambdaContactDirection(node)

    gibbsTerm + contactTerm + requiredConstraintsTerm

let FluxDerivative (conditions: IProcessConditions) (currentEstimation: StepVector) (node: NodeBase) : float =
    let dissipationTerm =
        2.0 * conditions.GasConstant * conditions.Temperature
        / node.Particle.Material.MolarVolume
        / node.Particle.Material.EquilibriumVacancyConcentration
        * node.SurfaceDistance.ToUpper
        * currentEstimation.FluxToUpper(node)
        / node.SurfaceDiffusionCoefficient.ToUpper
        * currentEstimation.Lambda1

    let thisRequiredConstraintsTerm = currentEstimation.LambdaVolume(node)
    let upperRequiredConstraintsTerm = currentEstimation.LambdaVolume(node.Upper)

    -dissipationTerm - thisRequiredConstraintsTerm + upperRequiredConstraintsTerm

let RequiredConstraint (currentEstimation: StepVector) (node: NodeBase) : float =
    let normalVolumeTerm = node.VolumeGradient.Normal * currentEstimation.NormalDisplacement(node)

    let tangentialVolumeTerm =
        match node with
        | :? ContactNodeBase as cn -> node.VolumeGradient.Tangential * currentEstimation.TangentialDisplacement(cn)
        | _ -> 0.0

    let fluxTerm = currentEstimation.FluxToUpper(node) - currentEstimation.FluxToUpper(node.Lower)

    normalVolumeTerm + tangentialVolumeTerm - fluxTerm

let ContactConstraints (currentEstimation: StepVector) (contact: ParticleContact) (node: ContactNodeBase) : float seq =
    let normalShift =
        currentEstimation.NormalDisplacement(node)
        + currentEstimation.NormalDisplacement(node.ContactedNode)

    let tangentialShift =
        currentEstimation.TangentialDisplacement(node)
        + currentEstimation.TangentialDisplacement(node.ContactedNode)

    let rotationShift =
        2.0
        * node.ContactedNode.Coordinates.R
        * Math.Sin(currentEstimation.RotationDisplacement(contact) / 2.0)

    let rotationDirection =
        -(node.ContactedNode.Coordinates.Phi - node.ContactedNode.ContactDirection)
        + (Math.PI - currentEstimation.RotationDisplacement(contact)) / 2.0

    seq {
        yield
            currentEstimation.RadialDisplacement(contact)
            - node.ContactDistanceGradient.Normal * normalShift
            - node.ContactDistanceGradient.Tangential * tangentialShift
            + Math.Cos(rotationDirection) * rotationShift

        yield
            currentEstimation.AngleDisplacement(contact)
            - node.ContactDirectionGradient.Normal * normalShift
            - node.ContactDirectionGradient.Tangential * tangentialShift
            - Math.Sin(rotationDirection) / contact.Distance * rotationShift
    }

let ContactNodeEquations
    (currentEstimation: StepVector)
    (contact: ParticleContact)
    (involvedNodes: ContactNodeBase seq)
    : float seq =
    seq {
        for n in involvedNodes do
            yield StateVelocityDerivativeTangential currentEstimation n
            yield StateVelocityDerivativeTangential currentEstimation n.ContactedNode

            yield! ContactConstraints currentEstimation contact n

            yield
                currentEstimation.LambdaContactDistance(n)
                - currentEstimation.LambdaContactDistance(n.ContactedNode)

            yield
                currentEstimation.LambdaContactDirection(n)
                - currentEstimation.LambdaContactDirection(n.ContactedNode)
    }

let ContactDistanceDerivative (currentEstimation: StepVector) (involvedNodes: ContactNodeBase seq) : float =
    query {
        for n in involvedNodes do
            sumBy (currentEstimation.LambdaContactDistance(n))
    }

let ContactDirectionDerivative (currentEstimation: StepVector) (involvedNodes: ContactNodeBase seq) : float =
    query {
        for n in involvedNodes do
            sumBy (currentEstimation.LambdaContactDirection(n))
    }

let ContactRotationDerivative
    (currentEstimation: StepVector)
    (contact: ParticleContact)
    (involvedNodes: ContactNodeBase seq)
    : float =
    query {
        for n in involvedNodes do
            let angleDifference =
                (n.ContactedNode.Coordinates.Phi - n.ContactedNode.ContactDirection)
                    .Reduce(Angle.ReductionDomain.WithNegative)

            sumBy (
                -n.ContactedNode.Coordinates.R
                * Math.Sin(currentEstimation.RotationDisplacement(contact) + angleDifference |> float)
                * currentEstimation.LambdaContactDistance(n)
                + n.ContactedNode.Coordinates.R / contact.Distance
                  * Math.Cos(currentEstimation.RotationDisplacement(contact) + angleDifference |> float)
                  * currentEstimation.LambdaContactDirection(n)
            )
    }

let NodeEquations (conditions: IProcessConditions) (currentEstimation: StepVector) (nodes: NodeBase seq) : float seq =

    seq {
        for n in nodes do
            yield StateVelocityDerivativeNormal currentEstimation n
            yield FluxDerivative conditions currentEstimation n
            yield RequiredConstraint currentEstimation n
    }

let ContactEquations (currentEstimation: StepVector) (contacts: ParticleContact seq) : float seq =

    seq {
        for c in contacts do
            let involvedNodes =
                query {
                    for n in c.From.Nodes.OfType<ContactNodeBase>() do
                        where (n.ContactedParticleId = c.To.Id)
                        select n
                }

            yield! ContactNodeEquations currentEstimation c involvedNodes

            yield ContactDistanceDerivative currentEstimation involvedNodes
            yield ContactDirectionDerivative currentEstimation involvedNodes
            yield ContactRotationDerivative currentEstimation c involvedNodes
    }

let DissipationEquality
    (conditions: IProcessConditions)
    (currentState: SolutionState)
    (currentEstimation: StepVector)
    : float =

    let dissipationNormal =
        query {
            for n in currentState.Nodes do
                sumBy (-n.GibbsEnergyGradient.Normal * currentEstimation.NormalDisplacement(n))
        }

    let dissipationTangential =
        query {
            for n in currentState.Nodes.OfType<ContactNodeBase>() do
                sumBy (-n.GibbsEnergyGradient.Tangential * currentEstimation.TangentialDisplacement(n))
        }

    let dissipationFunction =
        query {
            for p in currentState.Particles do
                sumBy (
                    query {
                        for n in p.Nodes do
                            sumBy (
                                n.SurfaceDistance.ToUpper * currentEstimation.FluxToUpper(n) ** 2
                                / n.SurfaceDiffusionCoefficient.ToUpper
                            )
                    }
                    / (p.Material.MolarVolume * p.Material.EquilibriumVacancyConcentration)
                )
        }
        * conditions.GasConstant
        * conditions.Temperature

    dissipationNormal + dissipationTangential - dissipationFunction

let FullEquationSet
    (conditions: IProcessConditions)
    (currentState: SolutionState)
    (currentEstimation: StepVector)
    : float seq =
    seq {
        yield! NodeEquations conditions currentEstimation currentState.Nodes
        yield! ContactEquations currentEstimation currentState.Contacts
        yield DissipationEquality conditions currentState currentEstimation
    }

let BlockForParticle (conditions: IProcessConditions) (currentEstimation: StepVector) (particle: Particle) : float seq =
    NodeEquations conditions currentEstimation particle.Nodes
