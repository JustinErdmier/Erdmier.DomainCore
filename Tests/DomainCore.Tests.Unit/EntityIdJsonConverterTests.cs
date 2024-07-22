using Erdmier.DomainCore.Tests.Unit.TextFixtures;

namespace Erdmier.DomainCore.Tests.Unit;

public class EntityIdJsonConverterTests : IClassFixture<EntityIdJsonConverterTestsFixture>
{
    private readonly EntityIdJsonConverterTestsFixture _fixture;

    public EntityIdJsonConverterTests(EntityIdJsonConverterTestsFixture fixture) => _fixture = fixture;

    [ Fact ]
    public void SerializeEntityIdImplementation_ShouldSucceed()
    {
        // Arrange
        string expected = _fixture.EntityIdImplementationJson;

        // Act
        string actual = JsonSerializer.Serialize(_fixture.EntityIdImplementation, _fixture.SerializerOptions);

        // Assert
        actual.Should()
              .BeEquivalentTo(expected);
    }

    [ Fact ]
    public void DeserializeEntityIdImplementation_ShouldSucceed()
    {
        // Arrange
        AuthorId expected = _fixture.EntityIdImplementation;

        // Act
        AuthorId? actual = JsonSerializer.Deserialize<AuthorId>(_fixture.EntityIdImplementationJson, _fixture.SerializerOptions);

        // Assert
        actual.Should()
              .NotBeNull()
              .And.BeEquivalentTo(expected);
    }

    [ Fact ]
    public void SerializeAggregateRootIdImplementation_ShouldSucceed()
    {
        // Arrange
        string expected = _fixture.AggregateRootIdImplementationJson;

        // Act
        string actual = JsonSerializer.Serialize(_fixture.AggregateRootIdImplementation, _fixture.SerializerOptions);

        // Assert
        actual.Should()
              .BeEquivalentTo(expected);
    }

    [ Fact ]
    public void DeserializeAggregateRootIdImplementation_ShouldSucceed()
    {
        // Arrange
        BookId expected = _fixture.AggregateRootIdImplementation;

        // Act
        BookId? actual = JsonSerializer.Deserialize<BookId>(_fixture.AggregateRootIdImplementationJson, _fixture.SerializerOptions);

        // Arrange
        actual.Should()
              .NotBeNull()
              .And.BeEquivalentTo(expected);
    }
}
