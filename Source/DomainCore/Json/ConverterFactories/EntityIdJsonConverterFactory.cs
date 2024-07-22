using Erdmier.DomainCore.Json.Converters;

namespace Erdmier.DomainCore.Json.ConverterFactories;

public sealed class EntityIdJsonConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type? typeToConvert)
    {
        while (typeToConvert    != null
               && typeToConvert != typeof(object))
        {
            Type currentType = typeToConvert.IsGenericType ? typeToConvert.GetGenericTypeDefinition() : typeToConvert;

            if (currentType == typeof(EntityId<>))
            {
                return true;
            }

            typeToConvert = typeToConvert.BaseType;
        }

        return false;
    }

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        Type valueType = typeToConvert.GetProperty(name: "Value")
                                      ?.PropertyType
                         ?? throw new JsonException();

        Type converterType = typeof(EntityIdJsonConverter<>).MakeGenericType(valueType);

        return (JsonConverter?)Activator.CreateInstance(converterType) ?? throw new JsonException();
    }
}
