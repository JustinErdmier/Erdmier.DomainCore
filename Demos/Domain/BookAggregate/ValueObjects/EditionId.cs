namespace Erdmier.DomainCore.Demos.Domain.BookAggregate.ValueObjects;

public sealed class EditionId : EntityId<Guid>
{
    private EditionId(Guid value)
        : base(value)
    { }

    private EditionId()
    { }

    public static EditionId Create(Guid value) => new(value);

    public static EditionId CreateUnique() => new(Guid.NewGuid());
}
