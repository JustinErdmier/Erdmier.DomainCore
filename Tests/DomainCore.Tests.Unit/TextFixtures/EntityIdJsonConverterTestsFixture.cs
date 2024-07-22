namespace Erdmier.DomainCore.Tests.Unit.TextFixtures;

public sealed class EntityIdJsonConverterTestsFixture
{
    public AuthorId EntityIdImplementation { get; } = AuthorId.CreateUnique();

    public string EntityIdImplementationJson => $$"""{"$type":"{{typeof(AuthorId).AssemblyQualifiedName}}","value":"{{EntityIdImplementation}}"}""";

    public BookId AggregateRootIdImplementation { get; } = BookId.CreateUnique();

    public string AggregateRootIdImplementationJson => $$"""{"$type":"{{typeof(BookId).AssemblyQualifiedName}}","value":"{{AggregateRootIdImplementation}}"}""";

    public JsonSerializerOptions SerializerOptions { get; } = new(JsonSerializerDefaults.Web)
    {
        Converters =
        {
            new EntityIdJsonConverterFactory()
        }
    };
}
