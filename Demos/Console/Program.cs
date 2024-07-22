using Erdmier.DomainCore.Demos.Domain.BookAggregate;
using Erdmier.DomainCore.Demos.Domain.BookAggregate.Entities;
using Erdmier.DomainCore.Demos.Domain.BookAggregate.Enums;
using Erdmier.DomainCore.Demos.Domain.BookAggregate.ValueObjects;

#region Variables

EditionId editionId = EditionId.CreateUnique();

Edition edition = Edition.Create(editionId, year: 2024);

AuthorId authorId = AuthorId.CreateUnique();

Author author = Author.Create(authorId, firstName: "Madeline", lastName: "Miller");

BookId bookId = BookId.CreateUnique();

Book book = Book.Create(bookId, title: "The Song of Achilles", numberOfPages: 378, Genres.HistoricalFiction, edition, [author]);

#endregion
