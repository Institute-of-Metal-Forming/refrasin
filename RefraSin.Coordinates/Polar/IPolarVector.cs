using RefraSin.Coordinates.Helpers;

namespace RefraSin.Coordinates.Polar;

public interface IPolarVector : IVector, IPolarCoordinates, IIsClose<IPolarVector>;