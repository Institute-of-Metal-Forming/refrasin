using System.Collections.Generic;
using System.Linq;
using RefraSin.Core.ParticleModel;
using RefraSin.Core.ParticleModel.States;

namespace RefraSin.Core.Solver.Solution
{
    /// <summary>
    /// Stellt den Zustand des Modells zu einen bestimmten zeitpunkt dar.
    /// </summary>
    public class TimeSeriesItem
    {
        /// <summary>
        /// Erstellt eine Instanz auf Basis einer Auflistung von Partikeln.
        /// </summary>
        /// <param name="time">Zeitpunkt</param>
        /// <param name="particles">Aufzählung von Partikeln.</param>
        public TimeSeriesItem(double time, IEnumerable<IParticle> particles)
        {
            Time = time;
            Particles = particles.Select(p => new ParticleState(p)).ToArray();
        }
        
        /// <summary>
        /// Zeitpunkt.
        /// </summary>
        public double Time { get; }

        /// <summary>
        /// Sequence of all particles.
        /// </summary>
        public IReadOnlyList<IParticle> Particles { get; }
    }
}