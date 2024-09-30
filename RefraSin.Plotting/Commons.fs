module RefraSin.Plotting.Commons

open Plotly.NET
open Plotly.NET.ConfigObjects
open Plotly.NET.LayoutObjects
open Plotly.NET.StyleParam

let ApplyDefaultPlotProperties (plot: GenericChart) : GenericChart =
    plot
    |> Chart.withConfig (Config.init (ToImageButtonOptions = ToImageButtonOptions.init (Format = ImageFormat.SVG)))

let ApplyPlainSpaceDefaultPlotProperties (plot: GenericChart) : GenericChart =
    plot
    |> ApplyDefaultPlotProperties
    |> Chart.withXAxisStyle (TitleText = "X")
    |> Chart.withYAxis (
        LinearAxis.init (Title = Title.init (Text = "Y"), ScaleAnchor = ScaleAnchor.X 1, ScaleRatio = 1)
    )
    |> Chart.withSize (1920, 1080)
