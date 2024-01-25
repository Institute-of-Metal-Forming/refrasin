namespace RefraSin.TEPSolver.EquationSystem.StepVectors

open System
open MathNet.Numerics.LinearAlgebra
open MathNet.Numerics.LinearAlgebra.Double
open RefraSin.ParticleModel


type StepVector(storage: double array, map: StepVectorMap) =
    inherit DenseVector(storage)

    new(vector: Vector<double>, map: StepVectorMap) =
        let array = vector.AsArray()

        StepVector(
            match array with
            | null -> vector.ToArray()
            | _ -> array
            , map
        )

    member this.Map = map

type NodeView(vector: StepVector, nodeId: Guid) =
    member this.NormalDisplacement =
        vector[vector.Map[nodeId, NodeUnknown.NormalDisplacement]]

    member this.FluxToUpper = vector[vector.Map[nodeId, NodeUnknown.FluxToUpper]]
    member this.LambdaVolume = vector[vector.Map[nodeId, NodeUnknown.LambdaVolume]]

type ContactNodeView(vector: StepVector, nodeId: Guid) =
    inherit NodeView(vector, nodeId)

    member this.TangentialDisplacement =
        vector[vector.Map[nodeId, NodeUnknown.TangentialDisplacement]]

    member this.LambdaContactDistance =
        vector[vector.Map[nodeId, NodeUnknown.LambdaContactDistance]]

    member this.LambdaContactDirection =
        vector[vector.Map[nodeId, NodeUnknown.LambdaContactDirection]]

type ContactView(vector: StepVector, fromId: Guid, toId: Guid) =
    member this.NormalDisplacement =
        vector[vector.Map[fromId, toId, ContactUnknown.RadialDisplacement]]

    member this.FluxToUpper =
        vector[vector.Map[fromId, toId, ContactUnknown.AngleDisplacement]]

    member this.LambdaVolume =
        vector[vector.Map[fromId, toId, ContactUnknown.RotationDisplacement]]

type StepVector with
    member this.Lambda1 = this[this.Map[GlobalUnknown.Lambda1]]

    member this.Item
        with get (node: INode) = NodeView(this, node.Id)

    member this.Item
        with get (node: IContactNode) = ContactNodeView(this, node.Id)

    member this.Item
        with get (contact: IParticleContact) = ContactView(this, contact.From.Id, contact.To.Id)
