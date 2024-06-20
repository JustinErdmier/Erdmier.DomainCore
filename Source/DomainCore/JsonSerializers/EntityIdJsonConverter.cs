﻿namespace Erdmier.DomainCore.JsonSerializers;

public sealed class EntityIdJsonConverter<TId> : JsonConverter<EntityId<TId>>
{
    public override EntityId<TId> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
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
            throw new JsonException(message: $"Unable to get the derived type when parsing an entity id using {typeName}");
        }

        // Deserialize the value property using the derived type
        object? value = JsonSerializer.Deserialize(json: document.RootElement.GetProperty(propertyName: "value")
                                                                 .GetRawText(),
                                                   returnType: derivedType.GetProperty(name: "Value")
                                                                          ?.PropertyType ??
                                                               throw new JsonException());

        // Create an instance of the derived class and return it
        MethodInfo? createMethod = derivedType.GetMethod(name: "Create", bindingAttr: BindingFlags.Public | BindingFlags.Static);

        return (EntityId<TId>?)createMethod?.Invoke(obj: null, parameters: [value]) ?? throw new JsonException();
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
