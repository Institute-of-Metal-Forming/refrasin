namespace RefraSin.TEPSolver.EquationSystem.StepVectors

open System
open RefraSin.ParticleModel

type NodeUnknown =
    | NormalDisplacement = 0
    | TangentialDisplacement = 1
    | FluxToUpper = 2
    | LambdaVolume = 3
    | LambdaContactDistance = 4
    | LambdaContactDirection = 5

type GlobalUnknown =
    | Lambda1 = 0


type ContactUnknown =
    | RadialDisplacement = 0
    | AngleDisplacement = 1
    | RotationDisplacement = 2


type StepVectorMap(contacts: IParticleContact seq, nodes: INode seq) =
    let mutable _index = -1

    let makeMap elements =
        elements
        |> Seq.map (fun t ->
            _index <- _index + 1
            (t, _index))
        |> Map

    let _nodeUnknownIndices =
        (seq {
            for node in nodes do
                yield (node.Id, NodeUnknown.NormalDisplacement)
                yield (node.Id, NodeUnknown.FluxToUpper)
                yield (node.Id, NodeUnknown.LambdaVolume)
         },
         seq {
             for node in nodes do
                 yield (node.Id, NodeUnknown.TangentialDisplacement)
                 yield (node.Id, NodeUnknown.LambdaContactDistance)
                 yield (node.Id, NodeUnknown.LambdaContactDirection)
         })
        ||> Seq.append
        |> makeMap

    let _contactUnknownIndices =
        seq {
            for contact in contacts do
                yield (contact.From.Id, contact.To.Id, ContactUnknown.RadialDisplacement)
                yield (contact.From.Id, contact.To.Id, ContactUnknown.AngleDisplacement)
                yield (contact.From.Id, contact.To.Id, ContactUnknown.RotationDisplacement)
        }
        |> makeMap

    member this.Item
        with get (unknown: GlobalUnknown) = _index + LanguagePrimitives.EnumToValue unknown

    member this.Item
        with get (node: INode, unknown: NodeUnknown) = _nodeUnknownIndices[(node.Id, unknown)]

    member this.Item
        with get (nodeId: Guid, unknown: NodeUnknown) = _nodeUnknownIndices[(nodeId, unknown)]

    member this.Item
        with get (contact: IParticleContact, unknown: ContactUnknown) =
            _contactUnknownIndices[(contact.From.Id, contact.To.Id, unknown)]

    member this.Item
        with get (fromId: Guid, toId: Guid, unknown: ContactUnknown) = _contactUnknownIndices[(fromId, toId, unknown)]
