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
        PropertyInfo valuePropertyInfo = typeof(EntityId<TId>).GetProperty(name: "Value", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                                         ?? throw new JsonException();

        object? value = JsonSerializer.Deserialize(document.RootElement.GetProperty(propertyName: "value")
                                                           .GetRawText(),
                                                   valuePropertyInfo.PropertyType);

        // Get the constructor and instantiate a new instance
        ConstructorInfo constructor = derivedType.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, binder: null, Type.EmptyTypes, modifiers: null)
                                      ?? throw new JsonException();

        object entityId = constructor.Invoke([]);

        // Set the id value and return
        valuePropertyInfo.SetValue(entityId, value, index: null);

        return (EntityId<TId>)entityId;
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
