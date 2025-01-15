module RefraSin.Analysis.SinteringAnalysis

open RefraSin.Graphs
open RefraSin.ParticleModel.Nodes
open RefraSin.ParticleModel.Particles
open RefraSin.ParticleModel.System
open RefraSin.ProcessModel


let ContactEdgeDistance (contact: IEdge<IParticle<_>>) : float =
    contact.From.Coordinates.VectorTo(contact.To.Coordinates).Norm

let ContactDistance (p1: IParticle<_>) (p2: IParticle<_>) : float =
    p1.Coordinates.VectorTo(p2.Coordinates).Norm

let ShrinkageByVolume (initialState: ISystemState<_, _>) (currentState: ISystemState<_, _>) : float =
    let initialVolume =
        ParticleSystemAnalysis.CenterHullParticle initialState
        |> ParticleAnalysis.Volume

    let currentVolume =
        ParticleSystemAnalysis.CenterHullParticle currentState
        |> ParticleAnalysis.Volume

    1.0 - currentVolume / initialVolume

let ShrinkagesByVolume (states: ISystemState<_, _> seq) : float seq =
    let volumes =
        [ for s in states -> ParticleSystemAnalysis.CenterHullParticle s |> ParticleAnalysis.Volume ]

    [ for v in volumes -> 1.0 - v / volumes[0] ]

let ShrinkageByDistance (initialState: ISystemState<_, _>) (currentState: ISystemState<_, _>) : float =
    let initialDistance =
        initialState.ParticleContacts() |> Seq.map ContactEdgeDistance |> Seq.sum

    let currentDistance =
        currentState.ParticleContacts() |> Seq.map ContactEdgeDistance |> Seq.sum

    1.0 - currentDistance / initialDistance


let ShrinkagesbyDistance (states: ISystemState<_, _> seq) : float seq =
    let distances =
        [ for s in states -> s.ParticleContacts() |> Seq.map ContactEdgeDistance |> Seq.sum ]

    [ for v in distances -> 1.0 - v / distances[0] ]

let NeckWidths (states: ISystemState<_, _> seq) : float seq =
    let widths =
        [ for s in states ->
              s.ParticleContacts()
              |> Seq.map (fun c ->
                  c.From.Nodes
                  |> Seq.sumBy (fun n ->
                      match n.Type with
                      | NodeType.GrainBoundary -> n.SurfaceDistance.Sum / 2.0
                      | NodeType.Neck when n.Upper.Type = NodeType.GrainBoundary -> n.SurfaceDistance.ToUpper / 2.0
                      | NodeType.Neck when n.Lower.Type = NodeType.GrainBoundary -> n.SurfaceDistance.ToLower / 2.0
                      | _ -> 0.0))
              |> Seq.sum ]

    widths
