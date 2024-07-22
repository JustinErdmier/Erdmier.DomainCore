namespace Erdmier.DomainCore.Json.Converters;

public sealed class EntityIdJsonConverter<TId> : JsonConverter<EntityId<TId>>
{
    public override EntityId<TId> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType is JsonTokenType.Null or not JsonTokenType.StartObject)
        {
            throw new JsonException();
        }

        // Create a new instance of JsonDocument and deserialize the reader into it
        using JsonDocument document = JsonDocument.ParseValue(ref reader);

        // Get the type name from JSON
        string? typeName = document.RootElement.GetProperty(propertyName: "$type")
                                   .GetString();

        if (string.IsNullOrWhiteSpace(typeName))
        {
            throw new JsonException(message: "Unable to get the derived type name when parsing an entity id");
        }

        // Use type name to determine the derived class, e.g. `GameSettingsId`
        Type? derivedType = Type.GetType(typeName);

        if (derivedType is null)
        {
            throw new JsonException($"Unable to get the derived type when parsing an entity id using {typeName}");
        }

        // Deserialize the value property using the derived type
        object? value = JsonSerializer.Deserialize(document.RootElement.GetProperty(propertyName: "value")
                                                           .GetRawText(),
                                                   derivedType.GetProperty(name: "Value")
                                                              ?.PropertyType
                                                   ?? throw new JsonException());

        // Get create method and validate signature
        MethodInfo? createMethod = derivedType.GetMethod(name: "Create", BindingFlags.Public | BindingFlags.Static);

        if (createMethod is null
            || createMethod.ReturnType != derivedType
            || createMethod.IsGenericMethod)
        {
            throw new JsonException($"Invalid create method signature when parsing a(n) {typeName}");
        }

        // Create an instance of the derived class and return it
        return (EntityId<TId>?)createMethod.Invoke(obj: null, [value]) ?? throw new JsonException();
    }

    public override void Write(Utf8JsonWriter writer, EntityId<TId> value, JsonSerializerOptions options)
    {
        // Start creating a JSON object
        writer.WriteStartObject();

        // Write the type name, so it can be used during deserialization
        writer.WriteString(propertyName: "$type",
                           value.GetType()
                                .AssemblyQualifiedName);

        // Write the value
        writer.WritePropertyName(propertyName: "value");
        JsonSerializer.Serialize(writer, value.Value, options);

        // End creating the JSON object
        writer.WriteEndObject();
    }
}
