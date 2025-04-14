module RefraSin.Analysis.SinteringAnalysis

open RefraSin.ParticleModel
open RefraSin.ParticleModel.Nodes
open RefraSin.ParticleModel.Particles
open RefraSin.ProcessModel
open RefraSin.ParticleModel.Particles.Extensions

let ContactEdgeDistance (contact: ContactPair<#IParticle<_>>) : float =
    contact.First.Coordinates.VectorTo(contact.Second.Coordinates).Norm

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
        ParticleContactExtensions.CreateContactedParticlePairs(initialState.Particles)
        |> Seq.map ContactEdgeDistance
        |> Seq.sum

    let currentDistance =
        ParticleContactExtensions.CreateContactedParticlePairs(currentState.Particles)
        |> Seq.map ContactEdgeDistance
        |> Seq.sum

    1.0 - currentDistance / initialDistance


let ShrinkagesByDistance (states: ISystemState<_, _> seq) : float seq =
    let distances =
        [ for s in states ->
              ParticleContactExtensions.CreateContactedParticlePairs(s.Particles)
              |> Seq.map ContactEdgeDistance
              |> Seq.sum ]

    [ for v in distances -> 1.0 - v / distances[0] ]

let NeckWidths (states: ISystemState<_, _> seq) : float seq =
    let widths =
        [ for s in states ->
              ParticleContactExtensions.CreateContactedParticlePairs(s.Particles)
              |> Seq.map (fun c ->
                  c.First.Nodes
                  |> Seq.sumBy (fun n ->
                      match n.Type with
                      | NodeType.GrainBoundary -> n.SurfaceDistance.Sum / 2.0
                      | NodeType.Neck when n.Upper.Type = NodeType.GrainBoundary -> n.SurfaceDistance.ToUpper / 2.0
                      | NodeType.Neck when n.Lower.Type = NodeType.GrainBoundary -> n.SurfaceDistance.ToLower / 2.0
                      | _ -> 0.0))
              |> Seq.sum ]

    widths
