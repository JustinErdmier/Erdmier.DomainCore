using System.Text.Json.Serialization.Metadata;

namespace Erdmier.DomainCore.JsonTypeInfoResolvers;

public sealed class EntityTypeInfoResolver<TBaseType, TAttribute> : DefaultJsonTypeInfoResolver
    where TAttribute : Attribute
{
    private readonly Assembly[] _assembliesToScan;

    public EntityTypeInfoResolver(params Assembly[] assembliesToScan) => _assembliesToScan = assembliesToScan;

    /// <inheritdoc />
    public override JsonTypeInfo GetTypeInfo(Type type, JsonSerializerOptions options)
    {
        Type         baseType     = typeof(TBaseType);
        JsonTypeInfo jsonTypeInfo = base.GetTypeInfo(type, options);

        List<JsonDerivedType> derivedTypes = _assembliesToScan.SelectMany(a => a.GetTypes()
                                                                                .Where(t => t.GetCustomAttributes(attributeType: typeof(TAttribute),
                                                                                                                  inherit: true)
                                                                                             .Any())
                                                                                .ToList()) // TODO: Is .ToList() needed here?
                                                              .Select(t => new JsonDerivedType(t))
                                                              .ToList();

        if (jsonTypeInfo.Type != baseType)
        {
            return jsonTypeInfo;
        }

        jsonTypeInfo.PolymorphismOptions = new JsonPolymorphismOptions
        {
            TypeDiscriminatorPropertyName        = "$type",
            IgnoreUnrecognizedTypeDiscriminators = true,
            UnknownDerivedTypeHandling           = JsonUnknownDerivedTypeHandling.FailSerialization
        };

        derivedTypes.ForEach(d => jsonTypeInfo.PolymorphismOptions.DerivedTypes.Add(d));

        return jsonTypeInfo;
    }
}
