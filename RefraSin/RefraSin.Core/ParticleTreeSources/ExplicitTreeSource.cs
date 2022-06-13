using System.Collections.Generic;
using IMF.Coordinates;
using IMF.Coordinates.Absolute;
using IMF.Coordinates.Polar;
using IMF.Enumerables;
using RefraSin.Core.ParticleModel;
using RefraSin.Core.ParticleSources;
using RefraSin.Core.SinteringProcesses;

namespace RefraSin.Core.ParticleTreeSources
{
    /// <summary>
    /// Generates a particle tree by placing particles in explicit hierarchy and position.
    /// </summary>
    public class ExplicitTreeSource : IParticleTreeSource
    {
        /// <summary>
        /// Creates a new instance with the specified root particle.
        /// </summary>
        /// <param name="root"></param>
        public ExplicitTreeSource(ExplicitTreeItem root)
        {
            Root = root;
        }

        /// <summary>
        /// Root particle.
        /// </summary>
        public ExplicitTreeItem Root { get; }

        /// <inheritdoc />
        public Tree<Particle> GetParticleTree(ISinteringProcess process,
            SurfaceNodeCountFunction surfaceNodeCountFunction) =>
            Root.GetTree().TreeMap(item =>
            {
                var particle = item.ParticleSource.GetParticle(process, surfaceNodeCountFunction);
                particle.CenterCoordinates = item.CenterCoordinates;
                particle.RotationAngle = item.RotationAngle;
                return particle;
            });
    }

    /// <summary>
    /// Item in the <see cref="ExplicitTreeSource"/>.
    /// </summary>
    public class ExplicitTreeItem : ITreeItem<ExplicitTreeItem>
    {
        private ExplicitTreeItem? _parent;

        private readonly PolarCoordinateSystem _coordinateSystem = new()
        {
            OriginSource = null,
            Origin = new AbsolutePoint()
        };

        /// <summary>
        /// Constructor.
        /// </summary>
        public ExplicitTreeItem(PolarPoint centerCoordinates, IParticleSource particleSource, Angle rotationAngle, IEnumerable<ExplicitTreeItem>? children = null)
        {
            ParticleSource = particleSource;
            RotationAngle = rotationAngle;
            CenterCoordinates = centerCoordinates;
            if (children != null)
                Children = new(this, children);
            else
                Children = new(this);
        }

        /// <inheritdoc />
        public ExplicitTreeItem? Parent
        {
            get => _parent;
            set
            {
                _parent = value;
                if (_parent != null)
                    _coordinateSystem.OriginSource = () => _parent.CenterCoordinates;
                else
                    _coordinateSystem.OriginSource = null;
            }
        }

        /// <inheritdoc />
        public TreeChildrenCollection<ExplicitTreeItem> Children { get; }

        /// <summary>
        /// Particle source, from which the particle in this item is generated.
        /// </summary>
        public IParticleSource ParticleSource { get; }

        /// <summary>
        /// Coordinates of the particles center in this item.
        /// </summary>
        public PolarPoint CenterCoordinates { get; }
        
        /// <summary>
        /// Angle of the particles rotation around its center.
        /// </summary>
        public Angle RotationAngle { get; }
    }
}