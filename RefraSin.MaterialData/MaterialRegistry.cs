namespace RefraSin.MaterialData;

/// <summary>
/// A simple in-memory material registry using backing dictionaries.
/// </summary>
public record MaterialRegistry : IMaterialRegistry
{
    private Dictionary<Guid, IMaterial> _materials = new();
    private Dictionary<(Guid From, Guid To), IMaterialInterface> _interfaces = new();

    /// <inheritdoc />
    public IReadOnlyList<IMaterial> Materials => _materials.Values.ToArray();

    /// <inheritdoc />
    public IReadOnlyList<IMaterialInterface> MaterialInterfaces => _interfaces.Values.ToArray();

    /// <inheritdoc />
    public void RegisterMaterial(IMaterial material) => _materials.Add(material.Id, material);

    /// <inheritdoc />
    public IMaterial GetMaterial(Guid id) => _materials[id];

    /// <inheritdoc />
    public IMaterial GetMaterial(string name) => _materials.Values.First(m => m.Name == name);

    /// <inheritdoc />
    public void RegisterMaterialInterface(IMaterialInterface materialInterface) =>
        _interfaces.Add((materialInterface.From, materialInterface.To), materialInterface);

    /// <inheritdoc />
    public IMaterialInterface GetMaterialInterface(Guid from, Guid to) => _interfaces[(from, to)];

    /// <inheritdoc />
    public IMaterialInterface GetMaterialInterface(IMaterial from, IMaterial to) => _interfaces[(from.Id, to.Id)];

    /// <inheritdoc />
    public IMaterialInterface GetMaterialInterface(string from, string to) =>
        _interfaces.Values.First(i => _materials[i.From].Name == from && _materials[i.To].Name == to);
}