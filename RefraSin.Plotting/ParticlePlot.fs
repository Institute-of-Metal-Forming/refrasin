module RefraSin.Plotting.ParticlePlot

open System.Collections.Generic
open System.Globalization
open Plotly.NET
open Plotly.NET.StyleParam
open RefraSin.Coordinates
open RefraSin.Graphs
open RefraSin.ParticleModel.Nodes
open RefraSin.ParticleModel.Particles

let PlotParticle (particle: IParticle<IParticleNode>) : GenericChart =
    let absolutePoints =
        [ for n in particle.Nodes -> n.Coordinates.Absolute ]
        @ [ particle.Nodes[0].Coordinates.Absolute ]

    let xy = [ for p in absolutePoints -> (p.X, p.Y) ]

    Chart.Line(
        xy = xy,
        Name = particle.Id.ToString().Substring(0, 8),
        ShowMarkers = true,
        MarkerSymbol = MarkerSymbol.X
    )
    |> Commons.ApplyPlainSpaceDefaultPlotProperties


let PlotParticles (particles: IEnumerable<IParticle<IParticleNode>>) : GenericChart =
    let particlePlots = [ for p in particles -> PlotParticle p ]

    particlePlots |> Chart.combine

let PlotPoint (point: IPoint) (label: string) =
    let abs = point.Absolute

    Chart.Point(
        x = [ abs.X ],
        y = [ abs.Y ],
        Name = label,
        MarkerSymbol = MarkerSymbol.Cross,
        TextPosition = TextPosition.TopRight,
        MultiText = [ abs.ToString("(,)", CultureInfo.InvariantCulture) ]
    )
    |> Commons.ApplyPlainSpaceDefaultPlotProperties

let PlotPoints (points: IPoint seq) (label: string) =
    let absolutePoints = [ for p in points -> p.Absolute ]
    let xy = [ for p in absolutePoints -> (p.X, p.Y) ]

    let labels =
        [ for p in absolutePoints -> p.ToString("(,)", CultureInfo.InvariantCulture) ]

    Chart.Point(
        xy = xy,
        Name = label,
        MultiText = labels,
        TextPosition = TextPosition.TopRight,
        MarkerSymbol = MarkerSymbol.Cross
    )
    |> Commons.ApplyPlainSpaceDefaultPlotProperties

let PlotLineString (points: IPoint seq) (label: string) =
    let absolutePoints = [ for p in points -> p.Absolute ]
    let xy = [ for p in absolutePoints -> (p.X, p.Y) ]

    Chart.Line(xy = xy, Name = label, ShowMarkers = true, MarkerSymbol = MarkerSymbol.Cross)
    |> Commons.ApplyPlainSpaceDefaultPlotProperties

let PlotLineRing (points: IPoint seq) (label: string) =
    let pointsList = points |> Seq.toList
    PlotLineString (pointsList @ [ pointsList[0] ]) label

let PlotContactEdge (edge: IEdge<IParticle<_>>) : GenericChart =
    let from = edge.From.Coordinates.Absolute
    let _to = edge.To.Coordinates.Absolute
    let xy = [ (from.X, from.Y); (_to.X, _to.Y) ]

    Chart.Line(
        xy = xy,
        MarkerSymbol = MarkerSymbol.Cross,
        Name = $"%s{edge.From.Id.ToString().Substring(0, 8)} -> %s{edge.To.Id.ToString().Substring(0, 8)}"
    )
    |> Commons.ApplyPlainSpaceDefaultPlotProperties
