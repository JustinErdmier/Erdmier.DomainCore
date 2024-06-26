namespace Erdmier.DomainCore.JsonSerializers;

public sealed class EntityJsonConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type? typeToConvert)
    {
        while (typeToConvert != null && typeToConvert != typeof(object))
        {
            Type currentType = typeToConvert.IsGenericType ? typeToConvert.GetGenericTypeDefinition() : typeToConvert;

            if (currentType == typeof(Entity<>))
            {
                return true;
            }

            typeToConvert = typeToConvert.BaseType;
        }

        return false;
    }

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        PropertyInfo propertyInfo = FindPropertyInHierarchy(typeToConvert, propertyName: "Id") ?? throw new JsonException();

        Type valueType = propertyInfo.PropertyType;

        Type converterType = typeof(EntityJsonConverter<>).MakeGenericType(valueType);

        return (JsonConverter?)Activator.CreateInstance(converterType) ?? throw new JsonException();
    }

    private static PropertyInfo? FindPropertyInHierarchy(Type? type, string propertyName)
    {
        PropertyInfo? property = null;

        while (type != null && type != typeof(object) && property == null)
        {
            property = type.GetProperties(bindingAttr: BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                           .FirstOrDefault(p => p.Name == propertyName);

            type = type.BaseType;
        }

        return property;
    }
}
