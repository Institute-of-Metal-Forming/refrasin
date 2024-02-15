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
        node.VolumeGradient.Normal * currentEstimation[node].LambdaVolume

    let contactTerm =
        match node with
        | :? ContactNodeBase as cn ->
            cn.ContactDistanceGradient.Normal * currentEstimation[cn].LambdaContactDistance
            + cn.ContactDirectionGradient.Normal
              * currentEstimation[cn].LambdaContactDirection
        | _ -> 0.0

    gibbsTerm + contactTerm + requiredConstraintsTerm

let StateVelocityDerivativeTangential (currentEstimation: StepVector) (node: ContactNodeBase) : float =

    let gibbsTerm = -node.GibbsEnergyGradient.Normal * (1.0 + currentEstimation.Lambda1)

    let requiredConstraintsTerm =
        node.VolumeGradient.Normal * currentEstimation[node].LambdaVolume

    let contactTerm =
        node.ContactDistanceGradient.Normal
        * currentEstimation[node].LambdaContactDistance
        + node.ContactDirectionGradient.Normal
          * currentEstimation[node].LambdaContactDirection

    gibbsTerm + contactTerm + requiredConstraintsTerm

let FluxDerivative (conditions: IProcessConditions) (currentEstimation: StepVector) (node: NodeBase) : float =
    let dissipationTerm =
        2.0 * conditions.GasConstant * conditions.Temperature
        / node.Particle.Material.MolarVolume
        / node.Particle.Material.EquilibriumVacancyConcentration
        * node.SurfaceDistance.ToUpper
        * currentEstimation[node].FluxToUpper
        / node.SurfaceDiffusionCoefficient.ToUpper
        * currentEstimation.Lambda1

    let thisRequiredConstraintsTerm = currentEstimation[node].LambdaVolume
    let upperRequiredConstraintsTerm = currentEstimation[node.Upper].LambdaVolume

    -dissipationTerm - thisRequiredConstraintsTerm + upperRequiredConstraintsTerm

let RequiredConstraint (currentEstimation: StepVector) (node: NodeBase) : float =
    let normalVolumeTerm = node.VolumeGradient.Normal * currentEstimation[node].NormalDisplacement

    let tangentialVolumeTerm =
        match node with
        | :? ContactNodeBase as cn -> node.VolumeGradient.Tangential * currentEstimation[cn].TangentialDisplacement
        | _ -> 0.0

    let fluxTerm = currentEstimation[node].FluxToUpper - currentEstimation[node.Lower].FluxToUpper

    normalVolumeTerm + tangentialVolumeTerm - fluxTerm

let ContactConstraints (currentEstimation: StepVector) (contact: ParticleContact) (node: ContactNodeBase) : float seq =
    let normalShift =
        currentEstimation[node].NormalDisplacement
        + currentEstimation[node.ContactedNode].NormalDisplacement

    let tangentialShift =
        currentEstimation[node].TangentialDisplacement
        + currentEstimation[node.ContactedNode].TangentialDisplacement

    let rotationShift =
        2.0
        * node.ContactedNode.Coordinates.R
        * Math.Sin(currentEstimation[contact].RotationDisplacement / 2.0)

    let rotationDirection =
        -(node.ContactedNode.Coordinates.Phi - node.ContactedNode.ContactDirection)
        + (Math.PI - currentEstimation[contact].RotationDisplacement) / 2.0

    seq {
        yield
            currentEstimation[contact].RadialDisplacement
            - node.ContactDistanceGradient.Normal * normalShift
            - node.ContactDistanceGradient.Tangential * tangentialShift
            + Math.Cos(rotationDirection) * rotationShift

        yield
            currentEstimation[contact].AngleDisplacement
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
                currentEstimation[n].LambdaContactDistance
                - currentEstimation[n.ContactedNode].LambdaContactDistance

            yield
                currentEstimation[n].LambdaContactDirection
                - currentEstimation[n.ContactedNode].LambdaContactDirection
    }

let ContactDistanceDerivative (currentEstimation: StepVector) (involvedNodes: ContactNodeBase seq) : float =
    query {
        for n in involvedNodes do
            sumBy currentEstimation[n].LambdaContactDistance
    }

let ContactDirectionDerivative (currentEstimation: StepVector) (involvedNodes: ContactNodeBase seq) : float =
    query {
        for n in involvedNodes do
            sumBy currentEstimation[n].LambdaContactDirection
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
                * Math.Sin(currentEstimation[contact].RotationDisplacement + angleDifference |> float)
                * currentEstimation[n].LambdaContactDistance
                + n.ContactedNode.Coordinates.R / contact.Distance
                  * Math.Cos(currentEstimation[contact].RotationDisplacement + angleDifference |> float)
                  * currentEstimation[n].LambdaContactDirection
            )
    }

let NodeEquations
    (conditions: IProcessConditions)
    (currentState: SolutionState)
    (currentEstimation: StepVector)
    : float seq =

    seq {
        for n in currentState.Nodes do
            yield StateVelocityDerivativeNormal currentEstimation n
            yield FluxDerivative conditions currentEstimation n
            yield RequiredConstraint currentEstimation n
    }

let ContactEquations (currentState: SolutionState) (currentEstimation: StepVector) : float seq =

    seq {
        for c in currentState.Contacts do
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
                sumBy (-n.GibbsEnergyGradient.Normal * currentEstimation[n].NormalDisplacement)
        }

    let dissipationTangential =
        query {
            for n in currentState.Nodes.OfType<ContactNodeBase>() do
                sumBy (-n.GibbsEnergyGradient.Tangential * currentEstimation[n].TangentialDisplacement)
        }

    let dissipationFunction =
        query {
            for p in currentState.Particles do
                sumBy (
                    query {
                        for n in p.Nodes do
                            sumBy (
                                n.SurfaceDistance.ToUpper * currentEstimation[n].FluxToUpper ** 2
                                / n.SurfaceDiffusionCoefficient.ToUpper
                            )
                    }
                    / (p.Material.MolarVolume * p.Material.EquilibriumVacancyConcentration)
                )
        }
        * conditions.GasConstant
        * conditions.Temperature

    dissipationNormal + dissipationTangential - dissipationFunction

let YieldEquations
    (conditions: IProcessConditions)
    (currentState: SolutionState)
    (currentEstimation: StepVector)
    : float seq =
    seq {
        yield! NodeEquations conditions currentState currentEstimation
        yield! ContactEquations currentState currentEstimation
        yield DissipationEquality conditions currentState currentEstimation
    }
