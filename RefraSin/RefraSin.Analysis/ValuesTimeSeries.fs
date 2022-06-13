namespace RefraSin.Analysis

open System
open System.Runtime.CompilerServices
open MathNet.Numerics.Interpolation
open RefraSin.Analysis

type ValuesTimeSeries = (double * double seq) seq

module ValuesTimeSeries =
    let Times (timeSeries: ValuesTimeSeries) : float seq = timeSeries |> Seq.map fst

    let Values (timeSeries: ValuesTimeSeries) : float seq seq = timeSeries |> Seq.map snd

    let Interpolate (timeSeries: ValuesTimeSeries) : IInterpolation seq =
        let timesArray = timeSeries |> Times |> Seq.toArray

        timeSeries
        |> Values
        |> Seq.map (
            Seq.toArray
            >> (ValueTimeSeries.interpolate timesArray)
        )

    let SelectValue (selector: double seq -> double) (timeSeries: ValuesTimeSeries) : ValueTimeSeries =
        timeSeries
        |> Seq.map (fun (t, vs) -> t, selector vs)

    let FirstValue (timeSeries: ValuesTimeSeries) : ValueTimeSeries = timeSeries |> SelectValue Seq.head

[<Extension>]
module ValuesTimeSeriesExtensions =
    [<Extension>]
    let Times (timeSeries: ValuesTimeSeries) : float seq = ValuesTimeSeries.Times timeSeries

    [<Extension>]
    let Values (timeSeries: ValuesTimeSeries) : float seq seq = ValuesTimeSeries.Values timeSeries

    [<Extension>]
    let Interpolate (timeSeries: ValuesTimeSeries) : IInterpolation seq = ValuesTimeSeries.Interpolate timeSeries

    [<Extension>]
    let InterpolateAll (timeSeriesSeq: ValuesTimeSeries seq) : IInterpolation seq seq =
        timeSeriesSeq |> Seq.map ValuesTimeSeries.Interpolate

    [<Extension>]
    let SelectValue (timeSeries: ValuesTimeSeries, selector: Func<double seq, double>) : ValueTimeSeries =
        ValuesTimeSeries.SelectValue selector.Invoke timeSeries

    [<Extension>]
    let FirstValue (timeSeries: ValuesTimeSeries) : ValueTimeSeries = ValuesTimeSeries.FirstValue timeSeries
