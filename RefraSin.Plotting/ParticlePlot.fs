module RefraSin.Plotting.ParticlePlot

open System.Collections.Generic
open System.Globalization
open Plotly.NET
open Plotly.NET.LayoutObjects
open Plotly.NET.StyleParam
open RefraSin.Coordinates
open RefraSin.ParticleModel.Nodes
open RefraSin.ParticleModel.Particles

let ApplyDefaultPlotProperties plot =
    plot
    |> Chart.withXAxisStyle (TitleText = "X")
    |> Chart.withYAxis (
        LinearAxis.init (Title = Title.init (Text = "Y"), ScaleAnchor = ScaleAnchor.X 1, ScaleRatio = 1)
    )
    |> Chart.withSize (1920, 1080)

let PlotParticle (particle: IParticle<IParticleNode>) : GenericChart =
    let absolutePoints =
        [ for n in particle.Nodes -> n.Coordinates.Absolute ]
        @ [ particle.Nodes[0].Coordinates.Absolute ]

    let xy = [ for p in absolutePoints -> (p.X, p.Y) ]

    Chart.Line(xy = xy, Name = particle.ToString(), ShowMarkers = true, MarkerSymbol = MarkerSymbol.X)
    |> ApplyDefaultPlotProperties


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
    |> ApplyDefaultPlotProperties

let PlotPoints (points: IPoint seq) (label: string) =
    let absolutePoints = [ for p in points -> p.Absolute ]
    let xy = [ for p in absolutePoints -> (p.X, p.Y) ]
    let labels = [ for p in absolutePoints -> p.ToString("(,)", CultureInfo.InvariantCulture) ]

    Chart.Point(xy = xy, Name = label, MultiText = labels, TextPosition=TextPosition.TopRight, MarkerSymbol=MarkerSymbol.Cross)
    |> ApplyDefaultPlotProperties

let PlotContactEdge (edge: IParticleContactEdge<_>) : GenericChart =
    let from = edge.From.Coordinates.Absolute
    let _to = edge.To.Coordinates.Absolute
    let xy = [(from.X, from.Y); (_to.X, _to.Y)]
    
    Chart.Line(xy=xy, MarkerSymbol=MarkerSymbol.Cross, Name=edge.ToString())