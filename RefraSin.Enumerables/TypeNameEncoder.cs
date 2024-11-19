using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RefraSin.Enumerables;

/// <summary>
/// Static class for encoding <see cref="Type"/> objects as string.
/// </summary>
internal static class TypeNameEncoder
{
    private static readonly Dictionary<Type, string> TypeNames = new();

    /// <summary>
    /// Creates a string representaion of a <see cref="TreeItem{TValue}"/>
    /// </summary>
    /// <param name="valueType">type of the value</param>
    /// <param name="value">value instance</param>
    public static string ToString(Type valueType, object? value) =>
        $"TreeItem<{EncodeTypeName(valueType)}> Value={value}";

    /// <summary>
    /// Kodiert einen Typnamen passend für XML-Dokumente.
    /// </summary>
    /// <param name="type">Typ</param>
    /// <returns></returns>
    public static string EncodeTypeName(Type? type)
    {
        if (type == null)
            return string.Empty;

        if (TypeNames.TryGetValue(type, out var cachedTypeName))
        {
            return cachedTypeName;
        }

        if (type.IsArray) // Arrays
        {
            var typeName = $"{EncodeTypeName(type.GetElementType())}[]";
            TypeNames.Add(type, typeName);
            return typeName;
        }

        var underlyingNullableType = Nullable.GetUnderlyingType(type);
        if (underlyingNullableType != null)
        {
            var typeName = $"{EncodeTypeName(underlyingNullableType)}?";
            TypeNames.Add(type, typeName);
            return typeName;
        }

        if (type.IsGenericType) // generische Typen
        {
            var genericArguments = type.GetGenericArguments().ToArray();
            var rawName = new StringBuilder(type.Name.Split('`')[0]);
            rawName.Append('<');
            rawName.Append(EncodeTypeName(genericArguments.First()));
            foreach (var param in genericArguments.Skip(1))
            {
                rawName.Append(", ");
                rawName.Append(EncodeTypeName(param));
            }

            rawName.Append('>');

            var typeName = rawName.ToString();
            TypeNames.Add(type, typeName);
            return typeName;
        }

        return type.Name;
    }
}
