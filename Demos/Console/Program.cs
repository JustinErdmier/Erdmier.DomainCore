using System.Text.Json;

using Erdmier.DomainCore.Demos.Domain.BookAggregate;
using Erdmier.DomainCore.Demos.Domain.BookAggregate.Entities;
using Erdmier.DomainCore.Demos.Domain.BookAggregate.Enums;
using Erdmier.DomainCore.Demos.Domain.BookAggregate.ValueObjects;
using Erdmier.DomainCore.Json.ConverterFactories;

#region Variables

EditionId editionId = EditionId.CreateUnique();

Edition edition = Edition.Create(editionId, year: 2024);

AuthorId authorId = AuthorId.CreateUnique();

Author author = Author.Create(authorId, firstName: "Madeline", lastName: "Miller");

BookId bookId = BookId.CreateUnique();

Book book = Book.Create(bookId, title: "The Song of Achilles", numberOfPages: 378, Genres.HistoricalFiction, edition, [author]);

JsonSerializerOptions serializerOptions =
    new(JsonSerializerDefaults.Web)
    {
        Converters =
        {
            new EntityIdJsonConverterFactory()
        },
        WriteIndented = true
    };

#endregion

#region Serializations & Deserializations

string editionIdJson = JsonSerializer.Serialize(editionId, serializerOptions);

Console.WriteLine(nameof(editionIdJson));
Console.WriteLine(editionIdJson + "\n");

EditionId? editionIdFromJson = JsonSerializer.Deserialize<EditionId>(editionIdJson, serializerOptions);

Console.WriteLine(nameof(editionIdFromJson));
Console.WriteLine(editionIdFromJson + "\n");

// string editionJson = JsonSerializer.Serialize(edition, serializerOptions);
//
// Console.WriteLine(nameof(editionJson));
// Console.WriteLine(editionJson + "\n");

// Edition? editionFromJson = JsonSerializer.Deserialize<Edition>(editionJson, serializerOptions);
//
// Console.WriteLine(value: nameof(editionFromJson));
// Console.WriteLine(value: editionFromJson + "\n");

string authorIdJson = JsonSerializer.Serialize(authorId, serializerOptions);

Console.WriteLine(nameof(authorIdJson));
Console.WriteLine(authorIdJson + "\n");

AuthorId? authorIdFromJson = JsonSerializer.Deserialize<AuthorId>(authorIdJson, serializerOptions);

Console.WriteLine(nameof(authorIdFromJson));
Console.WriteLine(authorIdFromJson + "\n");

// string authorJson = JsonSerializer.Serialize(author, serializerOptions);
//
// Console.WriteLine(nameof(authorJson));
// Console.WriteLine(authorJson + "\n");

// Author? authorFromJson = JsonSerializer.Deserialize<Author>(authorJson, serializerOptions);
//
// Console.WriteLine(value: nameof(authorFromJson));
// Console.WriteLine(value: authorFromJson + "\n");

string bookIdJson = JsonSerializer.Serialize(bookId, serializerOptions);

Console.WriteLine(nameof(bookIdJson));
Console.WriteLine(bookIdJson + "\n");

BookId? bookIdFromJson = JsonSerializer.Deserialize<BookId>(bookIdJson, serializerOptions);

Console.WriteLine(nameof(bookIdFromJson));
Console.WriteLine(bookIdFromJson + "\n");

// string bookJson = JsonSerializer.Serialize(book, serializerOptions);
//
// Console.WriteLine(nameof(bookJson));
// Console.WriteLine(bookJson + "\n");

// Book? bookFromJson = JsonSerializer.Deserialize<Book>(bookJson, serializerOptions);
//
// Console.WriteLine(nameof(bookFromJson));
// Console.WriteLine(bookFromJson + "\n");

#endregion
