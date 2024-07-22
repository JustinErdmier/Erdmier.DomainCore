namespace Erdmier.DomainCore.Demos.Domain.BookAggregate.Entities;

public sealed class Author : Entity<AuthorId>
{
    private Author(AuthorId authorId, string firstName, string lastName)
        : base(authorId)
        => UpdateName(firstName, lastName);

    private Author()
    { }

    public string FirstName { get; private set; } = default!;

    public string LastName { get; private set; } = default!;

    public Author UpdateName(string firstName, string lastName)
    {
        firstName.ThrowIfNull()
                 .IfEmpty()
                 .IfWhiteSpace();

        lastName.ThrowIfNull()
                .IfEmpty()
                .IfWhiteSpace();

        // Additional business logic/validation...

        FirstName = firstName;
        LastName  = lastName;

        return this;
    }

    public Author UpdateFirstName(string firstName)
    {
        firstName.ThrowIfNull()
                 .IfEmpty()
                 .IfWhiteSpace();

        // Additional business logic/validation...

        FirstName = firstName;

        return this;
    }

    public Author UpdateLastName(string lastName)
    {
        lastName.ThrowIfNull()
                .IfEmpty()
                .IfWhiteSpace();

        // Additional business logic/validation...

        LastName = lastName;

        return this;
    }

    public static Author Create(AuthorId authorId, string firstName, string lastName) => new(authorId, firstName, lastName);

    public static Author CreateUnique(string firstName, string lastName) => Create(AuthorId.CreateUnique(), firstName, lastName);
}
