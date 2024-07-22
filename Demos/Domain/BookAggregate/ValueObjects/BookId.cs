namespace Erdmier.DomainCore.Demos.Domain.BookAggregate.ValueObjects;

public sealed class BookId : AggregateRootId<Guid>
{
    /// <inheritdoc />
    private BookId(Guid value)
        : base(value)
    { }

    public static BookId Create(Guid value) => new(value);

    public static BookId CreateUnique() => Create(Guid.NewGuid());
}
