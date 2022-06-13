using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Globalization;
using System.IO;
using System.Linq;
using IMF.CommandLine;
using IMF.Utils;
using MathNet.Numerics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OxyPlot;
using RefraSin.CLI.Core;
using RefraSin.Core.SinteringProcesses;
using RefraSin.Plotting;

namespace RefraSin.CLI.ParticlePlotPlugin
{
    public class ParticlePlotCommand : Command
    {
        public ParticlePlotCommand(IHost host) : base("plot-particles", "draw the group of particles at a specified time")
        {
            Add(new Argument<string>(
                "processes",
                () => "all",
                "index of the process to select, give a ',' seperated list of integers or keyword 'all'"
            ));

            Add(new Argument<string>(
                "times",
                () => "all",
                "point in time, where to plot, give a ',' seperated list of decimal numbers or keyword 'all'"
            ));

            Add(new Option<string>(
                new[] {"-o", "--file-name-pattern"},
                () => "plots/particle-plot_{process}_{time}.svg",
                "pattern of the exported SVG file name (use placeholders {process} and {time} for the process label resp. the actual point in time)"
            ));

            Add(new Option<PlotSize>(
                new[] {"-s", "--size"},
                () => new PlotSize(5000, 5000),
                "size of the output SVG in the format width,height"
            ));

            Add(new Option<bool>(
                new[] {"-f", "--force-overwrite"},
                () => false,
                "Whether to overwrite existing files or not."
            ));

            Add(new Option<bool>(
                "--draw-center-center-conjunctions",
                () => false,
                "whether to draw lines between the centers of the particles"
            ));

            Add(new Option<bool>(
                "--draw-node-center-conjunctions",
                () => false,
                "whether to draw lines between the nodes and the center of their particle"
            ));

            Add(new Option<bool>(
                "--draw-node-type-markers",
                () => false,
                "whether to draw colored markers indicating the node type"
            ));

            Add(new Option<bool>(
                "--shade",
                () => false,
                "whether to shade areas belonging to a particles"
            ));

            Add(new Option<string>(
                "--length-unit",
                () => "μm",
                "unit of the axes"
            ));

            Add(new Option<string>(
                "--background-color",
                () => "#FFFFFFFF",
                "background color of the plot in HTML ARGB notation"
            ));

            Handler = CommandHandler.Create<string, string, string, PlotSize, bool, bool, bool, bool, bool, string, string, IHost>(Handle);
        }

        public void Handle(
            string processes,
            string times,
            string fileNamePattern,
            PlotSize size,
            bool forceOverwrite,
            bool drawCenterCenterConjunctions,
            bool drawNodeCenterConjunctions,
            bool drawNodeTypeMarkers,
            bool shade,
            string lengthUnit,
            string backgroundColor,
            IHost host
        )
        {
            var applicationContext = host.Services.GetRequiredService<ApplicationContext>();
            var logger = host.Services.GetRequiredService<ILogger<ParticlePlotCommand>>();

            var processList = processes == "all"
                ? applicationContext.SinteringProcesses
                : processes.Split(',').Select(s => applicationContext.SinteringProcesses[int.Parse(s)]).ToList();

            var states = times == "all"
                ? processList.SelectMany(p => p.Solution?.SolutionStates ?? Array.Empty<SolutionState>(), (p, s) => (p, s))
                : processList.Where(p => p.Solution != null).SelectMany(
                    _ => times.Split(',').Select(double.Parse),
                    (p, t) =>
                    {
                        var state = p.Solution!.SolutionStates.FirstOrDefault(s => s.Time >= t);
                        if (state == null)
                        {
                            logger.LogWarning("No state after time {Time} available. Selecting last state.", t);
                            state = p.Solution.SolutionStates[^1];
                        }

                        if (state.Time.AlmostEqual(t))
                            logger.LogInformation("Selected state at time {ActualTime} instead of {RequestedTime}.", state.Time, t);

                        return (p, state);
                    }
                );

            OxyColor backgroundOxyColor;
            try
            {
                backgroundOxyColor = OxyColor.Parse(backgroundColor);
            }
            catch (FormatException e)
            {
                logger.LogError("Color specification {Color} is invalid.", backgroundColor);
                return;
            }

            foreach (var (process, state) in states)
            {
                if (state.ParticleStates == null)
                {
                    logger.LogError("Particle data of this state is invalid.");
                    continue;
                }

                var particles = state.ParticleStates.ToArray();

                var plotModel = FlatSpacePlotModel.Create(
                    lengthUnit: lengthUnit,
                    backgroundColor: backgroundOxyColor
                );

                foreach (var particle in particles)
                {
                    if (shade)
                        plotModel.ShadeParticleArea(particle);
                    else
                        plotModel.DrawSurfaceLine(particle);
                }

                if (drawCenterCenterConjunctions) plotModel.DrawCenterCenterConjunctions(particles);
                if (drawNodeCenterConjunctions)
                    foreach (var particle in particles)
                        plotModel.DrawNodeCenterConjunctions(particle);
                if (drawNodeTypeMarkers)
                    foreach (var particle in particles)
                        plotModel.DrawNodeTypeMarkers(particle);

                var fileName = string.Format(CultureInfo.InvariantCulture,
                    fileNamePattern.Replace("{process}", "{0}")
                        .Replace("{time}", "{1}"),
                    process.Label,
                    state.Time
                );

                var svg = SvgExporter.ExportToString(plotModel, size.Width, size.Height, true, useVerticalTextAlignmentWorkaround: true);

                FileHelper.Write(new FileInfo(fileName), svg, applicationContext.IsInteractiveSession, forceOverwrite, logger);
            }
        }
    }
}