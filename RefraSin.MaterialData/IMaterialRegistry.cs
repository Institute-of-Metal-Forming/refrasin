namespace RefraSin.MaterialData;

public interface IMaterialRegistry
{
    IReadOnlyList<IMaterial> Materials { get; }

    IReadOnlyList<IMaterialInterface> MaterialInterfaces { get; }

    void RegisterMaterial(IMaterial material);
    IMaterial GetMaterial(Guid id);
    IMaterial GetMaterial(string name);

    void RegisterMaterialInterface(IMaterial from, IMaterial to, IMaterialInterface materialInterface);
    void RegisterMaterialInterface(Guid from, Guid to, IMaterialInterface materialInterface);
    void RegisterMaterialInterface(string from, string to, IMaterialInterface materialInterface);
    IMaterialInterface GetMaterialInterface(Guid from, Guid to);
    IMaterialInterface GetMaterialInterface(IMaterial from, IMaterial to);
    IMaterialInterface GetMaterialInterface(string from, string to);
}