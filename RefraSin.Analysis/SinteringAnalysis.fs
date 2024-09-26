module RefraSin.Analysis.SinteringAnalysis

open System.Linq
open RefraSin.ParticleModel.Nodes
open RefraSin.ProcessModel

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
    let initialDistance = initialState.ParticleContacts |> Seq.map _.Distance |> Seq.sum
    let currentDistance = currentState.ParticleContacts |> Seq.map _.Distance |> Seq.sum

    1.0 - currentDistance / initialDistance


let ShrinkagesbyDistance (states: ISystemState<_, _> seq) : float seq =
    let distances = [ for s in states -> s.ParticleContacts |> Seq.map _.Distance |> Seq.sum ]

    [ for v in distances -> 1.0 - v / distances[0] ]

let NeckWidths (states: ISystemState<_, _> seq) : float seq =
    let widths =
        [ for s in states ->
              s.ParticleContacts
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
