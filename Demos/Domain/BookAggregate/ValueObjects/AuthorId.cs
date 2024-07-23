namespace Erdmier.DomainCore.Demos.Domain.BookAggregate.ValueObjects;

public sealed class AuthorId : EntityId<Guid>
{
    private AuthorId(Guid value)
        : base(value)
    { }

    private AuthorId()
    { }

    public static AuthorId Create(Guid value) => new(value);

    public static AuthorId CreateUnique() => Create(Guid.NewGuid());
}
