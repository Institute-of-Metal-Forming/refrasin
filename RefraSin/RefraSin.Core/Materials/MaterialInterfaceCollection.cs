using System.Collections;
using System.Collections.Generic;

namespace RefraSin.Core.Materials
{
    /// <summary>
    /// Collection that holds instances of <see cref="MaterialInterface"/> and indexes them by involved <see cref="Material"/> instances.
    /// </summary>
    public class MaterialInterfaceCollection : IReadOnlyCollection<MaterialInterface>, ICollection<MaterialInterface>,
        IReadOnlyDictionary<(Material current, Material other), MaterialInterface>
    {
        /// <summary>
        /// Creates a new empty instance.
        /// </summary>
        public MaterialInterfaceCollection() { }

        /// <summary>
        /// Creates a new instance with the specified items.
        /// </summary>
        /// <param name="items"></param>
        public MaterialInterfaceCollection(IEnumerable<MaterialInterface> items)
        {
            foreach (var item in items) Add(item);
        }

        private readonly Dictionary<(Material current, Material other), MaterialInterface> _dictionary = new();

        /// <summary>
        /// Removes an item from the collection.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(MaterialInterface item) => _dictionary.Remove((item.CurrentMaterial, item.ContactedMaterial));

        /// <summary>
        /// Gets the number of elements.
        /// </summary>
        public int Count => _dictionary.Count;

        /// <inheritdoc />
        public bool IsReadOnly => false;

        /// <summary>
        /// Gets a sequence of the keys.
        /// </summary>
        public IEnumerable<(Material current, Material other)> Keys => _dictionary.Keys;

        /// <summary>
        /// Gets a sequence of the values.
        /// </summary>
        public IEnumerable<MaterialInterface> Values => _dictionary.Values;

        /// <summary>
        /// Adds an item to the collection.
        /// </summary>
        /// <param name="item"></param>
        public void Add(MaterialInterface item) => _dictionary.Add((item.CurrentMaterial, item.ContactedMaterial), item);

        /// <inheritdoc />
        public void Clear() => _dictionary.Clear();

        /// <summary>
        /// Checks whether the specified item is contained in the collection.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(MaterialInterface item) => _dictionary.ContainsKey((item.CurrentMaterial, item.ContactedMaterial));

        /// <inheritdoc />
        public void CopyTo(MaterialInterface[] array, int arrayIndex) => _dictionary.Values.CopyTo(array, arrayIndex);

        /// <inheritdoc />
        public bool ContainsKey((Material current, Material other) key) => _dictionary.ContainsKey(key);

        /// <inheritdoc />
        public IEnumerator<MaterialInterface> GetEnumerator() => _dictionary.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        IEnumerator<KeyValuePair<(Material current, Material other), MaterialInterface>>
            IEnumerable<KeyValuePair<(Material current, Material other), MaterialInterface>>.GetEnumerator() => _dictionary.GetEnumerator();

        /// <inheritdoc />
        public bool TryGetValue((Material current, Material other) key, out MaterialInterface value) => _dictionary.TryGetValue(key, out value);

        /// <inheritdoc />
        public MaterialInterface this[(Material current, Material other) key] => _dictionary[key];
    }
}