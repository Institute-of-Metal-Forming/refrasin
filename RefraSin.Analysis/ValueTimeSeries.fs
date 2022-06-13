namespace RefraSin.Analysis

open System.Runtime.CompilerServices
open MathNet.Numerics
open MathNet.Numerics.Interpolation

type ValueTimeSeries = (double * double) seq

module ValueTimeSeries =
    let Times (timeSeries: ValueTimeSeries) : float seq = timeSeries |> Seq.map fst

    let Values (timeSeries: ValueTimeSeries) : float seq = timeSeries |> Seq.map snd

    let internal interpolate times values =
        (times, values) |> LinearSpline.InterpolateSorted :> IInterpolation

    let Interpolate (timeSeries: ValueTimeSeries) : IInterpolation =
        (timeSeries |> Times |> Seq.toArray, timeSeries |> Values |> Seq.toArray)
        ||> interpolate

    let private raster (count: int) (min: float) (max: float) =
        if count <= 0 then
            invalidArg (nameof count) "must be > 0"

        if min <= 0 then
            invalidArg (nameof min) "must be > 0"

        if max <= 0 then
            invalidArg (nameof max) "must be > 0"

        if max <= min then
            invalidArg (nameof max) "must be > min"

        Generate.LogSpaced(count, log10 min, log10 max)

    let private findFirstGreaterZero values = values |> Seq.find (fun v -> v > 0.)

    let Raster (count: int) (timeSeries: ValueTimeSeries) : float seq =
        let min =
            timeSeries |> Times |> findFirstGreaterZero

        let max = timeSeries |> Times |> Seq.last

        raster count min max

    let InnerRaster (count: int) (timeSeries: ValueTimeSeries seq) : float seq =
        let min =
            timeSeries
            |> Seq.map (Times >> findFirstGreaterZero)
            |> Seq.max

        let max =
            timeSeries
            |> Seq.map (Times >> Seq.last)
            |> Seq.min

        raster count min max

    let OuterRaster (count: int) (timeSeries: ValueTimeSeries seq) : float seq =
        let min =
            timeSeries
            |> Seq.map (Times >> findFirstGreaterZero)
            |> Seq.min

        let max =
            timeSeries
            |> Seq.map (Times >> Seq.last)
            |> Seq.max

        raster count min max

[<Extension>]
module ValueTimeSeriesExtensions =
    [<Extension>]
    let Times (timeSeries: ValueTimeSeries) : float seq = ValueTimeSeries.Times timeSeries

    [<Extension>]
    let Values (timeSeries: ValueTimeSeries) : float seq = ValueTimeSeries.Values timeSeries

    [<Extension>]
    let Interpolate (timeSeries: ValueTimeSeries) : IInterpolation = ValueTimeSeries.Interpolate timeSeries

    [<Extension>]
    let InterpolateAll (timeSeriesSeq: ValueTimeSeries seq) : IInterpolation seq =
        timeSeriesSeq
        |> Seq.map ValueTimeSeries.Interpolate

    [<Extension>]
    let Raster (timeSeries: ValueTimeSeries, count: int) : float seq = ValueTimeSeries.Raster count timeSeries

    [<Extension>]
    let InnerRaster (timeSeries: ValueTimeSeries seq, count: int) : float seq =
        ValueTimeSeries.InnerRaster count timeSeries

    [<Extension>]
    let OuterRaster (timeSeries: ValueTimeSeries seq, count: int) : float seq =
        ValueTimeSeries.OuterRaster count timeSeries
