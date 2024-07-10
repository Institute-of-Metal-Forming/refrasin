using RefraSin.Coordinates.Helpers;

namespace RefraSin.Coordinates.Cartesian;

public interface ICartesianVector : IVector, ICartesianCoordinates, IIsClose<ICartesianVector>;