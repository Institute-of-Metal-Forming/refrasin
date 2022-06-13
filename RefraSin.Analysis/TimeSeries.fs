namespace RefraSin.Analysis

open System
open System.Runtime.CompilerServices
open Microsoft.FSharp.Collections
open RefraSin.Core.Solver.Solution

type TimeSeries = TimeSeriesItem seq

module TimeSeries =

    let Split (timeSeries: TimeSeries) : Map<Guid, ParticleTimeSeries> =
        let particles =
            (Seq.head timeSeries).Particles
            |> Seq.map (fun p -> p.Id)

        let times =
            timeSeries |> Seq.map (fun tsi -> tsi.Time)

        seq {
            for id in particles do
                id,
                timeSeries
                |> Seq.map (fun tsi -> tsi.Particles |> Seq.find (fun p -> id = p.Id))
        }
        |> Seq.map (fun (id, states) -> id, Seq.zip times states)
        |> Map

    let SplitPairs (timeSeries: TimeSeries) : Map<Guid * Guid, ParticlePairTimeSeries> =
        let particles =
            (Seq.head timeSeries).Particles
            |> Seq.map (fun p -> p.Id)

        let particlePairs =
            Seq.allPairs particles particles
            |> Seq.filter (fun (a, b) -> not (a = b))

        let times =
            timeSeries |> Seq.map (fun tsi -> tsi.Time)

        seq {
            for id1, id2 in particlePairs do
                (id1, id2),
                timeSeries
                |> Seq.map
                    (fun tsi ->
                        (tsi.Particles |> Seq.find (fun p -> id1 = p.Id),
                         tsi.Particles |> Seq.find (fun p -> id2 = p.Id)))
        }
        |> Seq.map (fun (id, states) -> id, Seq.zip times states)
        |> Map

[<Extension>]
module TimeSeriesExtensions =

    [<Extension>]
    let Split (timeSeries: TimeSeriesItem seq) : Map<Guid, ParticleTimeSeries> = TimeSeries.Split timeSeries

    [<Extension>]
    let SplitPairs (timeSeries: TimeSeriesItem seq) : Map<Guid * Guid, ParticlePairTimeSeries> =
        TimeSeries.SplitPairs timeSeries
