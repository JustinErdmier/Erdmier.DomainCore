using System.Text.Json;

using Erdmier.DomainCore.Demos.Console.Domain.BookAggregate;
using Erdmier.DomainCore.Demos.Console.Domain.BookAggregate.Entities;
using Erdmier.DomainCore.Demos.Console.Domain.BookAggregate.Enums;
using Erdmier.DomainCore.Demos.Console.Domain.BookAggregate.ValueObjects;
using Erdmier.DomainCore.JsonSerializers;


#region Variables

EditionId editionId = EditionId.CreateUnique();

Edition edition = Edition.Create(editionId, year: 2024);

AuthorId authorId = AuthorId.CreateUnique();

Author author = Author.Create(authorId, firstName: "Madeline", lastName: "Miller");

BookId bookId = BookId.CreateUnique();

Book book = Book.Create(bookId, title: "The Song of Achilles", numberOfPages: 378, Genres.HistoricalFiction, edition, authors: [author]);

JsonSerializerOptions serializerOptions =
    new(JsonSerializerDefaults.Web) { Converters = { new EntityIdJsonConverterFactory(), new EntityJsonConverterFactory() }, WriteIndented = true };

#endregion


#region Serializations & Deserializations

string editionIdJson = JsonSerializer.Serialize(editionId, serializerOptions);

Console.WriteLine(value: nameof(editionIdJson));
Console.WriteLine(value: editionIdJson + "\n");

EditionId? editionIdFromJson = JsonSerializer.Deserialize<EditionId>(editionIdJson, serializerOptions);

Console.WriteLine(value: nameof(editionIdFromJson));
Console.WriteLine(value: editionIdFromJson + "\n");

string editionJson = JsonSerializer.Serialize(edition, serializerOptions);

Console.WriteLine(value: nameof(editionJson));
Console.WriteLine(value: editionJson + "\n");

// Uncomment to throw error deserializing property of type int.
// Edition? editionFromJson = JsonSerializer.Deserialize<Edition>(editionJson, serializerOptions);
//
// Console.WriteLine(value: nameof(editionFromJson));
// Console.WriteLine(value: editionFromJson + "\n");

string authorIdJson = JsonSerializer.Serialize(authorId, serializerOptions);

Console.WriteLine(value: nameof(authorIdJson));
Console.WriteLine(value: authorIdJson + "\n");

AuthorId? authorIdFromJson = JsonSerializer.Deserialize<AuthorId>(authorIdJson, serializerOptions);

Console.WriteLine(value: nameof(authorIdFromJson));
Console.WriteLine(value: authorIdFromJson + "\n");

string authorJson = JsonSerializer.Serialize(author, serializerOptions);

Console.WriteLine(value: nameof(authorJson));
Console.WriteLine(value: authorJson + "\n");

// Uncomment to throw error deserializing property of type string.
// Author? authorFromJson = JsonSerializer.Deserialize<Author>(authorJson, serializerOptions);
//
// Console.WriteLine(value: nameof(authorFromJson));
// Console.WriteLine(value: authorFromJson + "\n");

string bookIdJson = JsonSerializer.Serialize(bookId, serializerOptions);

Console.WriteLine(value: nameof(bookIdJson));
Console.WriteLine(value: bookIdJson + "\n");

BookId? bookIdFromJson = JsonSerializer.Deserialize<BookId>(bookIdJson, serializerOptions);

Console.WriteLine(value: nameof(bookIdFromJson));
Console.WriteLine(value: bookIdFromJson + "\n");

// Uncomment to throw error for incompatible types
string bookJson = JsonSerializer.Serialize(book, serializerOptions);

Console.WriteLine(value: nameof(bookJson));
Console.WriteLine(value: bookJson + "\n");

// Uncomment to throw error regarding ambiguous reference to property named "Id"
// Book? bookFromJson = JsonSerializer.Deserialize<Book>(bookJson, serializerOptions);
//
// Console.WriteLine(nameof(bookFromJson));
// Console.WriteLine(bookFromJson + "\n");

#endregion
