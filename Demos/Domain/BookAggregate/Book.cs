using Erdmier.DomainCore.Demos.Domain.BookAggregate.Entities;
using Erdmier.DomainCore.Demos.Domain.BookAggregate.Enums;

namespace Erdmier.DomainCore.Demos.Domain.BookAggregate;

public sealed class Book : AggregateRoot<BookId, Guid>
{
    private readonly List<Author> _authors = [];

    private Book(BookId bookId, string title, int numberOfPages, Genres genre, Edition edition, List<Author> authors)
        : base(bookId)
    {
        UpdateTitle(title);
        SetNumberOfPages(NumberOfPages);
        SetGenre(genre);
        SetEdition(edition);
        AddAuthors(authors);
    }

    private Book()
    { }

    public string Title { get; private set; } = default!;

    public int NumberOfPages { get; private set; }

    public Genres Genre { get; private set; }

    public Edition Edition { get; private set; } = default!;

    public IReadOnlyList<Author> Authors => _authors.AsReadOnly();

    public Book UpdateTitle(string title)
    {
        title.ThrowIfNull()
             .IfEmpty()
             .IfWhiteSpace();

        // Additional business logic/validation...

        Title = title;

        return this;
    }

    public Book SetNumberOfPages(int numberOfPages)
    {
        numberOfPages.Throw()
                     .IfNegative();

        // Additional business logic/validation...

        NumberOfPages = numberOfPages;

        return this;
    }

    public Book SetGenre(Genres genre)
    {
        genre.Throw()
             .IfOutOfRange();

        // Additional business logic/validation...

        Genre = genre;

        return this;
    }

    public Book SetEdition(Edition edition)
    {
        edition.ThrowIfNull();

        // Additional business logic/validation...

        Edition = edition;

        return this;
    }

    public Book AddAuthor(Author author)
    {
        if (!_authors.Contains(author))
        {
            _authors.Add(author);
        }

        return this;
    }

    public Book AddAuthors(List<Author> authors)
    {
        authors.ForEach(a => AddAuthor(a));

        return this;
    }

    public static Book Create(BookId bookId, string title, int numberOfPages, Genres genre, Edition edition, List<Author> authors)
        => new(bookId, title, numberOfPages, genre, edition, authors);

    public static Book CreateUnique(string title, int numberOfPages, Genres genre, Edition edition) => Create(BookId.CreateUnique(), title, numberOfPages, genre, edition, []);
}
