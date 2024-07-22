# The Goal Behind Erdmier.DomainCore

## Overview

`Erdmier.DomainCore` is a simple library of base classes for designing and implementing domain aggregates and entities.
The `ValueObject`, `EntityId`, `AggregateRootId`, `Entity`, and `AggregateRoot` classes are all based on the classes designed by Amichai Mantinband in
his [YouTube DDD series](https://youtube.com/playlist?list=PLzYkqgWkHPKBcDIP5gzLfASkQyTdy0t4k&si=H_Aj_i7wusL1iIoo). There are some differences in my classes
that make them more compatible with EF Core. The purpose of this package is to create a single library for these classes and relevant features (e.g.,
serialization) that can be published to NuGet and reused across my and others apps.

## Enhanced EF Core Compatibility

As originally designed by Amichai, the class definitions worked with EF Core with no incidents; however, they only work due to the simplicity of the tutorial's
app. There's no work found in this repo (yet) regarding this issue/solution because I documented and solved it in another
repo, [Inheritance Issue Demo](https://github.com/JustinErdmier/InheritanceIssueDemo/tree/PassIdToBaseCtorUsingEntityFrameworkCore). That hyperlink will take
you to the branch with the detailed `README` file which thoroughly documents the issue and explains how it was solved.

## Intended Design Practices

While it's not entirely possible to enforce this on derived types, the intended design for classes deriving from `EntityId`, `AggregateRootId`, `Entity`,
and `AggregateRoot` is to make all constructors private and to use a static method instead. Granted, I can't really control how others will define their
derived types if anyone ever does use this library, however, my intention is to always have a `Create()` method for types deriving from `Entity` and
`AggregateRoot`. Types derived from `EntityId` and `AggregateRootId` will always have a `Create()` method (where the inner value is given) and a
`CreateUnique()` method (where the inner value is auto-generated).

For the purposes of the MVP and just getting this to work, I am dictating that every derived type must have a public, static, non-generic method name "Create"
whose return type is that of the derived type. This is not ideal and does conflict with the intended design as the `Create()` method shouldn't take a value for
every single property (e.g., collection properties). Collection properties are meant to be backed as a field (
e.g., `private readonly List<string> _strings = []`) and exposed with a computed property (e.g., `public List<string> Strings => _strings.AsReadOnly()`).

I have some ideas on how to improve this in the future. But the goal right now is to get serialization and deserialization working regardless of the method
name.

## The Issue

The issue is that this design does not work well with the default JSON serialization and deserialization. Before creating this library, I
attempted to follow the steps provided in
the [documentation regarding polymorphic serialization](https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/polymorphism?pivots=dotnet-8-0).
But even if it did work, it's not ideal at all
to [configure polymorphic serialization using contract models](https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/polymorphism?pivots=dotnet-8-0#configure-polymorphism-with-the-contract-model)
not to mention completely unsustainable for an app or repo with a large domain.

One of, but not the sole reason as to why the built-in polymorphic serializer won't work is because of the intentional design behind the access of the
constructors and the intended way to create new objects. All constructors should be private and a static method named `Create` be used instead. This is further
explained in a previous section.

### Tests & Demos

I did start to write unit tests, and do plan to write many more, but decided to take a different approach because my IDE is currently having an issue with
debugging unit tests. Instead, I created a console demo app (`Erdmier.DomainCore.Demos.Console`) where I've defined some dummy domain types, and I am
serializing/deserializing them. For the rest of this document, I will be referring to the types defined in `Erdmier.DomainCore.Demos.Console.Domain`. To
reproduce any of the issues described hereafter, simply uncomment/comment the relevant sections in `Erdmier.DomainCore.Demos.Console.Program` and run.

### Serializing

I was able to get the serialization for types deriving from the `EntityId`, `AggregateRootId`, and `Entity` classes but I am running into an issue when trying
to
serialize a type deriving from `AggregateRoot`. When trying to serialize the `Book` aggregate root, for example,
the `EntityJsonConverterFactory.CreateConverter()` method throws the following error:

```text
Unhandled exception. System.Reflection.AmbiguousMatchException: Ambiguous match found for 'Erdmier.DomainCore.Models.AggregateRoot`2[Erdmier.DomainCore.Demos.Console.Domain.BookAggregate.ValueObjects.BookId,System.Guid] Erdmier.DomainCore.Models.Identities.AggregateRootId`1[System.Guid] Id'.
   at System.RuntimeType.GetPropertyImpl(String name, BindingFlags bindingAttr, Binder binder, Type returnType, Type[] types, ParameterModifier[] modifiers)
   at System.Type.GetProperty(String name, BindingFlags bindingAttr)
   at Erdmier.DomainCore.JsonSerializers.EntityJsonConverterFactory.CreateConverter(Type typeToConvert, JsonSerializerOptions options) in C:\Users\justi\Source\Personal\Erdmier.DomainCore\Source\DomainCore\JsonSerializers\EntityJsonConverterFactory.cs:line 31 
   at System.Text.Json.Serialization.JsonConverterFactory.GetConverterInternal(Type typeToConvert, JsonSerializerOptions options)
   at System.Text.Json.Serialization.Metadata.DefaultJsonTypeInfoResolver.GetConverterForType(Type typeToConvert, JsonSerializerOptions options, Boolean resolveJsonConverterAttribute)
   at System.Text.Json.Serialization.Metadata.DefaultJsonTypeInfoResolver.GetTypeInfo(Type type, JsonSerializerOptions options)
   at System.Text.Json.JsonSerializerOptions.GetTypeInfoNoCaching(Type type)
   at System.Text.Json.JsonSerializerOptions.CachingContext.CreateCacheEntry(Type type, CachingContext context)
--- End of stack trace from previous location ---
   at System.Text.Json.JsonSerializerOptions.GetTypeInfoInternal(Type type, Boolean ensureConfigured, Nullable`1 ensureNotNull, Boolean resolveIfMutable, Boolean fallBackToNearestAncestorType)
   at System.Text.Json.JsonSerializerOptions.GetTypeInfoForRootType(Type type, Boolean fallBackToNearestAncestorType)
   at System.Text.Json.JsonSerializer.GetTypeInfo[T](JsonSerializerOptions options)
   at System.Text.Json.JsonSerializer.Serialize[TValue](TValue value, JsonSerializerOptions options)
   at Program.<Main>$(String[] args) in C:\Users\justi\Source\Personal\Erdmier.DomainCore\Demos\Console\Program.cs:line 31
```

I used the debugger to step through the `GetProperty()` method, and was able to find that the issue is that the following two properties are found:

```text
+-+----------+-------+--------+----------------------+--------------------------------------------------------------------------------------------------------------------------------+----------------------------------------------------------------------------------+-------------+-------------+----------+-------------+--------------+----+-------------------------------------------------------------------------+----------------------------------------------------------+--------------------------------------------------------------------------------------+
|#|Attributes|CanRead|CanWrite|CustomAttributes      |DeclaringType                                                                                                                   |GetMethod                                                                         |IsCollectible|IsSpecialName|MemberType|MetadataToken|Module        |Name|PropertyType                                                             |ReflectedType                                             |SetMethod                                                                             |
+-+----------+-------+--------+----------------------+--------------------------------------------------------------------------------------------------------------------------------+----------------------------------------------------------------------------------+-------------+-------------+----------+-------------+--------------+----+-------------------------------------------------------------------------+----------------------------------------------------------+--------------------------------------------------------------------------------------+
|0|None      |true   |false   |CustomAttributeData[0]|Erdmier.DomainCore.Models.AggregateRoot`2[Erdmier.DomainCore.Demos.Console.Domain.BookAggregate.ValueObjects.BookId,System.Guid]|Erdmier.DomainCore.Models.Identities.AggregateRootId`1[System.Guid] get_Id()      |false        |false        |Property  |385875969    |DomainCore.dll|Id  |Erdmier.DomainCore.Models.Identities.AggregateRootId`1[System.Guid]      |Erdmier.DomainCore.Demos.Console.Domain.BookAggregate.Book|null                                                                                  |
|1|None      |true   |true    |CustomAttributeData[0]|Erdmier.DomainCore.Models.Entity`1[Erdmier.DomainCore.Demos.Console.Domain.BookAggregate.ValueObjects.BookId]                   |Erdmier.DomainCore.Demos.Console.Domain.BookAggregate.ValueObjects.BookId get_Id()|false        |false        |Property  |385875970    |DomainCore.dll|Id  |Erdmier.DomainCore.Demos.Console.Domain.BookAggregate.ValueObjects.BookId|Erdmier.DomainCore.Demos.Console.Domain.BookAggregate.Book|Void set_Id(Erdmier.DomainCore.Demos.Console.Domain.BookAggregate.ValueObjects.BookId)|
+-+----------+-------+--------+----------------------+--------------------------------------------------------------------------------------------------------------------------------+----------------------------------------------------------------------------------+-------------+-------------+----------+-------------+--------------+----+-------------------------------------------------------------------------+----------------------------------------------------------+--------------------------------------------------------------------------------------+
```

This makes sense, but I'm not really sure how to resolve it. The intended serialization of the class's properties is the same for any types derived from
either `Entity` or `AggregateRoot`. Creating an entirely new factory and converter class for types deriving from `AggregateRoot` seems overkill, and if possible
I'd like to avoid it. Instead, we need to identify if the derived type is that of `AggregateRoot` and handle appropriately. I did try checking the type similar
to the `EntityJsonConverterFactory.CanConvert()` method:

```csharp
public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        while (typeToConvert != null && typeToConvert != typeof(object))
        {
            Type currentType = typeToConvert.IsGenericType ? typeToConvert.GetGenericTypeDefinition() : typeToConvert;

            if (currentType == typeof(AggregateRoot<,>))
            {
                throw new JsonException();
            }

            typeToConvert = typeToConvert.BaseType;
        }

        Type valueType = typeToConvert.GetProperty(name: "Id", bindingAttr: BindingFlags.Public | BindingFlags.Instance)
                                      ?.PropertyType ??
                         throw new JsonException();

        Type converterType = typeof(EntityJsonConverter<>).MakeGenericType(valueType);

        return (JsonConverter?)Activator.CreateInstance(converterType) ?? throw new JsonException();
    }
```

I know this doesn't actually do anything, but before I even tried implementing the body of the `if` statement, I just wanted to see if it would ever enter.
However, when the method was hit for the `Book` object, it never hit the `if` statement. I even tried replacing `currentType == typeof(AggregateRoot<,>)`
with `currentType == typeof(Entity<>)`, which is exactly the same check we're doing in the previous method, and it also never hit for either the `Book`
or `Author` objects.

### Deserialization

Deserializing types derived from `EntityId` and `AggregateRootId` works as currently expected. I say "currently expected" because I'm not entirely sure if
anything will break/change or if types derived from `AggregateRootId` will need to be handled differently based on whatever it takes to get `AggregateRoot`
derived types working.

I'm sure there's a cleaner and/or more sophisticated way to write the logic of the `EntityJsonConverter.Read()` method, but I haven't gotten there yet and am
more focused on the MVP (i.e., just getting it working) first. The simple idea is to parse each of the JSON properties based on derived type's definition.
Immediately, you might note the difference between how I'm reading the data in the `EntityJsonConverter` versus `EntityIdJsonConverter`. The logic
in `EntityIdJsonConverter`, using the `JsonDocument` object, was generated by ChatGPT and modified by me. The use of the `while` loop is what is used in the
documentation examples
for [creating custom converters](https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/converters-how-to?pivots=dotnet-8-0).

Apart from basic validation, we want to be able to parse the properties of derived types agnostic to the property's type. However, the only thing I have found
to even remotely work like this is to pass `reader.ValueSpan` to the `JsonSerializer.Deserialize()` method. However, this is throwing an error when trying to
parse the value for a property of type string. For example, the `Author.FirstName` of the author object is instantiated as "Madeline", and trying to deserialize
the author object throws the following error:

```text
Unhandled exception. System.Text.Json.JsonException: 'M' is an invalid start of a value. Path: $ | LineNumber: 0 | BytePositionInLine: 0.
 ---> System.Text.Json.JsonReaderException: 'M' is an invalid start of a value. LineNumber: 0 | BytePositionInLine: 0.
   at System.Text.Json.ThrowHelper.ThrowJsonReaderException(Utf8JsonReader& json, ExceptionResource resource, Byte nextByte, ReadOnlySpan`1 bytes)
   at System.Text.Json.Utf8JsonReader.ConsumeValue(Byte marker)
   at System.Text.Json.Utf8JsonReader.ReadFirstToken(Byte first)
   at System.Text.Json.Utf8JsonReader.ReadSingleSegment()
   at System.Text.Json.Utf8JsonReader.Read()
   at System.Text.Json.Serialization.JsonConverter`1.ReadCore(Utf8JsonReader& reader, JsonSerializerOptions options, ReadStack& state)
   --- End of inner exception stack trace ---
   at System.Text.Json.ThrowHelper.ReThrowWithPath(ReadStack& state, JsonReaderException ex)
   at System.Text.Json.Serialization.JsonConverter`1.ReadCore(Utf8JsonReader& reader, JsonSerializerOptions options, ReadStack& state)
   at System.Text.Json.Serialization.Metadata.JsonTypeInfo`1.DeserializeAsObject(Utf8JsonReader& reader, ReadStack& state)
   at System.Text.Json.JsonSerializer.ReadFromSpanAsObject(ReadOnlySpan`1 utf8Json, JsonTypeInfo jsonTypeInfo, Nullable`1 actualByteCount)
   at System.Text.Json.JsonSerializer.Deserialize(ReadOnlySpan`1 utf8Json, Type returnType, JsonSerializerOptions options)
   at Erdmier.DomainCore.JsonSerializers.EntityJsonConverter`1.Read(Utf8JsonReader& reader, Type typeToConvert, JsonSerializerOptions options) in C:\Users\justi\Source\Personal\Erdmier.DomainCore\Source\DomainCore\JsonSerializers\EntityJsonConverter.cs:line 66
   at System.Text.Json.Serialization.JsonConverter`1.ReadAsObject(Utf8JsonReader& reader, Type typeToConvert, JsonSerializerOptions options)
   at System.Text.Json.Serialization.Converters.CastingConverter`1.Read(Utf8JsonReader& reader, Type typeToConvert, JsonSerializerOptions options)
   at System.Text.Json.Serialization.JsonConverter`1.TryRead(Utf8JsonReader& reader, Type typeToConvert, JsonSerializerOptions options, ReadStack& state, T& value, Boolean& isPopulatedValue)
   at System.Text.Json.Serialization.JsonConverter`1.ReadCore(Utf8JsonReader& reader, JsonSerializerOptions options, ReadStack& state)
   at System.Text.Json.JsonSerializer.ReadFromSpan[TValue](ReadOnlySpan`1 utf8Json, JsonTypeInfo`1 jsonTypeInfo, Nullable`1 actualByteCount)
   at System.Text.Json.JsonSerializer.ReadFromSpan[TValue](ReadOnlySpan`1 json, JsonTypeInfo`1 jsonTypeInfo)
   at System.Text.Json.JsonSerializer.Deserialize[TValue](String json, JsonSerializerOptions options)
   at Program.<Main>$(String[] args) in C:\Users\justi\Source\Personal\Erdmier.DomainCore\Demos\Console\Program.cs:line 44
```

This issue has particularly stumped me. The only "solution" I can think of would be to manually check the type of the value and write a `switch` statement (or
series of `if` statements) calling the appropriate method on the `Utf8JsonReader` object (e.g., `reader.GetString()`, `reader.GetInt()`, etc.). However, this is
not sustainable nor is it desirable.

Another example is when deserializing the edition object, the following error is thrown:

```text
Unhandled exception. System.Text.Json.JsonException: Expected depth to be zero at the end of the JSON payload. There is an open JSON object or array that should be closed. Path: $ | LineNumber: 0 | BytePositionInLine: 1.
 ---> System.Text.Json.JsonReaderException: Expected depth to be zero at the end of the JSON payload. There is an open JSON object or array that should be closed. LineNumber: 0 | BytePositionInLine: 1.
   at System.Text.Json.ThrowHelper.ThrowJsonReaderException(Utf8JsonReader& json, ExceptionResource resource, Byte nextByte, ReadOnlySpan`1 bytes)
   at System.Text.Json.Utf8JsonReader.ReadSingleSegment()
   at System.Text.Json.Utf8JsonReader.Read()
   at System.Text.Json.Utf8JsonReader.TrySkip()
   at System.Text.Json.JsonDocument.TryParseValue(Utf8JsonReader& reader, JsonDocument& document, Boolean shouldThrow, Boolean useArrayPools)
   at System.Text.Json.JsonDocument.ParseValue(Utf8JsonReader& reader)
   at Erdmier.DomainCore.JsonSerializers.EntityIdJsonConverter`1.Read(Utf8JsonReader& reader, Type typeToConvert, JsonSerializerOptions options) in C:\Users\justi\Source\Personal\Erdmier.DomainCore\Source\DomainCore\JsonSerializers\EntityIdJsonConverter.cs:line 13
   at System.Text.Json.Serialization.JsonConverter`1.ReadAsObject(Utf8JsonReader& reader, Type typeToConvert, JsonSerializerOptions options)
   at System.Text.Json.Serialization.Converters.CastingConverter`1.Read(Utf8JsonReader& reader, Type typeToConvert, JsonSerializerOptions options)
   at System.Text.Json.Serialization.JsonConverter`1.TryRead(Utf8JsonReader& reader, Type typeToConvert, JsonSerializerOptions options, ReadStack& state, T& value, Boolean& isPopulatedValue)
   at System.Text.Json.Serialization.JsonConverter`1.ReadCore(Utf8JsonReader& reader, JsonSerializerOptions options, ReadStack& state)
   --- End of inner exception stack trace ---
   at System.Text.Json.ThrowHelper.ReThrowWithPath(ReadStack& state, JsonReaderException ex)
   at System.Text.Json.Serialization.JsonConverter`1.ReadCore(Utf8JsonReader& reader, JsonSerializerOptions options, ReadStack& state)
   at System.Text.Json.Serialization.Metadata.JsonTypeInfo`1.DeserializeAsObject(Utf8JsonReader& reader, ReadStack& state)
   at System.Text.Json.JsonSerializer.ReadFromSpanAsObject(ReadOnlySpan`1 utf8Json, JsonTypeInfo jsonTypeInfo, Nullable`1 actualByteCount)
   at System.Text.Json.JsonSerializer.Deserialize(ReadOnlySpan`1 utf8Json, Type returnType, JsonSerializerOptions options)
   at Erdmier.DomainCore.JsonSerializers.EntityJsonConverter`1.Read(Utf8JsonReader& reader, Type typeToConvert, JsonSerializerOptions options) in C:\Users\justi\Source\Personal\Erdmier.DomainCore\Source\DomainCore\JsonSerializers\EntityJsonConverter.cs:line 66
   at System.Text.Json.Serialization.JsonConverter`1.ReadAsObject(Utf8JsonReader& reader, Type typeToConvert, JsonSerializerOptions options)
   at System.Text.Json.Serialization.Converters.CastingConverter`1.Read(Utf8JsonReader& reader, Type typeToConvert, JsonSerializerOptions options)
   at System.Text.Json.Serialization.JsonConverter`1.TryRead(Utf8JsonReader& reader, Type typeToConvert, JsonSerializerOptions options, ReadStack& state, T& value, Boolean& isPopulatedValue)
   at System.Text.Json.Serialization.JsonConverter`1.ReadCore(Utf8JsonReader& reader, JsonSerializerOptions options, ReadStack& state)
   at System.Text.Json.JsonSerializer.ReadFromSpan[TValue](ReadOnlySpan`1 utf8Json, JsonTypeInfo`1 jsonTypeInfo, Nullable`1 actualByteCount)
   at System.Text.Json.JsonSerializer.ReadFromSpan[TValue](ReadOnlySpan`1 json, JsonTypeInfo`1 jsonTypeInfo)
   at System.Text.Json.JsonSerializer.Deserialize[TValue](String json, JsonSerializerOptions options)
   at Program.<Main>$(String[] args) in C:\Users\justi\Source\Personal\Erdmier.DomainCore\Demos\Console\Program.cs:line 47
```
