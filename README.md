# RefraSin - 2D Sintering Simulation with Sharp Interfaces

RefraSin is a library for simulation of sintering processes regarding surface and grain boundary diffusion using a sharp interface description and a thermodynamic extremal principle (TEP) solution approach.
It provides higher efficiency and computational speed, with equal versatility, compared to state-of-the-art phase field method (PFM) approaches.
The software is implemented based on the .NET-Core platform in the C# and F# programming languages.

## Documentation

Details on the approach and theoretical background will be published in the doctoral thesis of M. Weiner ([the working copy is publicly available here](https://github.com/axtimhaus/dissertation/tree/main/dissertation/dissertation.pdf)).

For examples on how to use the software please refer to the respective test assemblies (especially [`RefraSin.TEPSolver.Test`](https://github.com/Institute-of-Metal-Forming/refrasin/tree/main/RefraSin.TEPSolver.Test)) and the [simulations included in the repository of the dissertation](https://github.com/axtimhaus/dissertation/tree/main/dissertation/sim).

## License

The software is available under the terms of the [MIT License](LICENSE).

## Citing

The software is published and archived on Zenodo.
Please refer to it using the Zenodo DOI and the information given in the [CITATION.cff file](CITATION.cff).

## Funding

The software was developed within the FOR 3010 Refrabund project, funded by the Deutsche Forschungsgemeinschaft (DFG) under project number 416817512.

## Major Components

### `RefraSin.Analysis`

Routines in F# for computing characteristic values such as particle volumes, shrinkages, neck sizes, ...

### `RefraSin.Compaction`

Routines to create particle packings from distinct particles.

### `RefraSin.Coordinates`

A 2D library for dealing with nested cartesian and polar coordinate systems.

### `RefraSin.Enumerables`

Provides special collection types.

### `RefraSin.MaterialData`

Provides data structures storing material data.

### `RefraSin.Numerics`

Provides routines and wrappers for numerical root finding of linear and nonlinear systems.

### `RefraSin.ParquetStorage`

Provides an implementation of the storage interface for storing simulation results in Parquet files.

### `RefraSin.ParticleModel`

Provides object oriented data structures to represent particles, their surfaces consisting of nodes, as well as routines for generating and modifying them (e.g. remeshing).

### `RefraSin.Plotting`

Provides routines in F# for plotting of simulation results using Plotly.NET.

### `RefraSin.ProcessModel`

Provides data structures that define a sintering process.

### `RefraSin.Storage`

Provides an abstract interface for storing simulation results.

### `RefraSin.TEPSolver`

The heart of the library implementing the sintering model based on the thermodynamic extremal principle (TEP).

### `RefraSin.Vertex`

Provides an abstract interface representing a vertex with a GUID, which is implemented by various types throughout the library such as materials, particles and nodes.
