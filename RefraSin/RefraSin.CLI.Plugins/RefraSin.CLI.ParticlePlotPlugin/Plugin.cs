using System.CommandLine;
using Microsoft.Extensions.Hosting;
using RefraSin.CLI.Core;

namespace RefraSin.CLI.ParticlePlotPlugin
{
    public class Plugin : ICommandPlugin
    {
        public Command GetCommand(IHost host) => new ParticlePlotCommand(host);
    }
}