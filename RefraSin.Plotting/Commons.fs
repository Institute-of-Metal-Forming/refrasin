module RefraSin.Plotting.Commons

open Plotly.NET
open Plotly.NET.LayoutObjects
open Plotly.NET.StyleParam

let ApplyPlainSpaceDefaultPlotProperties plot =
    plot
    |> Chart.withXAxisStyle (TitleText = "X")
    |> Chart.withYAxis (
        LinearAxis.init (Title = Title.init (Text = "Y"), ScaleAnchor = ScaleAnchor.X 1, ScaleRatio = 1)
    )
    |> Chart.withSize (1920, 1080)
