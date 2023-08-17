using System;
using RefraSin.Coordinates.Absolute;
using static System.String;

namespace RefraSin.Coordinates;

/// <summary>
///     Abstract base class for coordinates systems.
/// </summary>
public abstract class CoordinateSystem : ICoordinateSystem
{
    private IPoint? _origin;
    private Angle? _rotationAngle;

    /// <summary>
    ///     Creates a new instance.
    /// </summary>
    protected CoordinateSystem() { }

    /// <summary>
    ///     Creates a new instance with the specified origin and rotation.
    /// </summary>
    /// <param name="origin">origin of this system's axes</param>
    /// <param name="rotationAngle">rotation angle of this system's axes</param>
    protected CoordinateSystem(IPoint? origin, double rotationAngle = 0)
    {
        _origin = origin;
        RotationAngle = rotationAngle;
    }

    /// <summary>
    ///     Gets or sets the human readable label of this system. Used in <see cref="ToString" />.
    /// </summary>
    public string Label { get; set; } = Empty;

    /// <summary>
    ///     Gets or sets the delegate used to determine the rotation angle of this system.
    ///     <remarks>
    ///         This property can be used to build dynamic trees of coordinate systems.
    ///         If its value is not null, it takes precedence over the value set to <see cref="RotationAngle" />.
    ///         This delegate may return null, to make the getter of <see cref="RotationAngle" /> to behave like this delegate
    ///         were null.
    ///         The getter of <see cref="RotationAngle" /> tries always to invoke this delegate, if it is not null.
    ///     </remarks>
    /// </summary>
    public Func<Angle?>? RotationAngleSource { get; set; }

    /// <summary>
    ///     Gets or sets the delegate used to determine the origin of this system.
    ///     <remarks>
    ///         This property can be used to build dynamic trees of coordinate systems.
    ///         If its value is not null, it takes precedence over the value set to <see cref="Origin" />.
    ///         This delegate may return null, to make the getter of <see cref="Origin" /> to behave like this delegate were
    ///         null.
    ///         The getter of <see cref="Origin" /> tries always to invoke this delegate, if it is not null.
    ///     </remarks>
    /// </summary>
    public Func<IPoint?>? OriginSource { get; set; }

    /// <inheritdoc />
    /// <remarks>
    ///     Use the setter to set an explicit rotation angle of this system.
    ///     The getter always tries to invoke <see cref="RotationAngleSource" /> for determining the rotation angle.
    ///     If <see cref="RotationAngleSource" /> is null or returns null, this property returns the value set by the setter.
    ///     If no value was explicitly set, it is initialized on first use with 0 rad.
    /// </remarks>
    public Angle RotationAngle
    {
        get => RotationAngleSource?.Invoke() ?? (_rotationAngle ??= new Angle(0));
        set => _rotationAngle = value;
    }

    /// <inheritdoc />
    /// <remarks>
    ///     Use the setter to set an explicit origin of this system.
    ///     The getter always tries to invoke <see cref="OriginSource" /> for determining the origin.
    ///     If <see cref="OriginSource" /> is null or returns null, this property returns the value set by the setter.
    ///     If no value was explicitly set, it is initialized on first use with <c>new AbsolutePoint()</c> (the
    ///     <see cref="AbsolutePoint" /> (0, 0))
    /// </remarks>
    public IPoint Origin
    {
        get => OriginSource?.Invoke() ?? (_origin ??= new AbsolutePoint());
        set => _origin = value;
    }

    /// <inheritdoc />
    public override string ToString() => $"{GetType().Name} {Label}".Trim();
}