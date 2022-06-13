using System.Linq;
using IMF.Coordinates;
using MathNet.Numerics.RootFinding;
using Microsoft.Extensions.Logging;
using static System.Math;
using static MathNet.Numerics.Constants;

namespace RefraSin.Core.Materials
{
    /// <summary>
    /// Data class containing data about material interfaces.
    /// </summary>
    /// <param name="CurrentMaterial"></param>
    /// <param name="ContactedMaterial"></param>
    /// <param name="InterfaceEnergy"></param>
    /// <param name="DiffusionCoefficient"></param>
    public record MaterialInterface(Material CurrentMaterial, Material ContactedMaterial, double InterfaceEnergy, double DiffusionCoefficient)
    {
        /// <summary>
        /// Dihedral angle at triple points of this interface and two free surfaces.
        /// </summary>
        public Angle DihedralAngle = ComputeDihedralAngle(CurrentMaterial, ContactedMaterial, InterfaceEnergy);

        private static Angle ComputeDihedralAngle(Material currentMaterial, Material contactedMaterial, double interfaceEnergy)
        {
            var interfaceEnergies = new[]
            {
                contactedMaterial.SurfaceEnergy, currentMaterial.SurfaceEnergy, interfaceEnergy
            };
            var averageInterfaceEnergy = interfaceEnergies.Average();
            var relativeInterfaceEnergies = interfaceEnergies.Select(e => e / averageInterfaceEnergy).ToArray();

            var result = Broyden.TryFindRootWithJacobianStep(
                psis => new[]
                {
                    relativeInterfaceEnergies[0] / Sin(psis[0]) - relativeInterfaceEnergies[1] / Sin(psis[1]),
                    relativeInterfaceEnergies[2] / Sin(psis[2]) - relativeInterfaceEnergies[0] / Sin(psis[0]),
                    Abs(psis[0]) + Abs(psis[1]) + Abs(psis[2]) - Pi2
                },
                new[] { Pi2Over3, Pi2Over3, Pi2Over3 }
                , 1e-8, 1000, 1e-4, out var psisSol
            );

            if (result)
                return Angle.FromRadians(psisSol[0], Angle.ReductionDomain.AllPositive);

            Configuration.CreateLogger<MaterialInterface>()
                .LogError(
                    "Dihedral angle of interface {CurrentMaterial} <-> {ContactedMaterial} with interface energies {InterfaceEnergies} could not been calculated. Using 120° instead.",
                    currentMaterial.Label, contactedMaterial.Label, interfaceEnergies);
            return Angle.FromRadians(Pi2Over3);
        }

        private const double Pi2Over3 = Pi2 / 3;
    }
}