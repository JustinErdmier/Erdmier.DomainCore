namespace Erdmier.DomainCore.Demos.Domain.BookAggregate.Entities;

public sealed class Edition : Entity<EditionId>
{
    private Edition(EditionId id, int year)
        : base(id)
        => Year = year;

    private Edition()
    { }

    public int Year { get; }

    public static Edition Create(EditionId id, int year) => new(id, year);

    public static Edition CreateUnique(int year) => new(EditionId.CreateUnique(), year);
}
