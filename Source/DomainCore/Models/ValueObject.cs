﻿namespace Erdmier.DomainCore.Models;

// TODO: Using source generation, add deconstruct method.
// TODO: Make value object serializable.

/// <summary>An immutable object.</summary>
public abstract class ValueObject : IEquatable<ValueObject>
{
    public bool Equals(ValueObject? other) => Equals(obj: other);

    protected abstract IEnumerable<object?> GetEqualityComponents();

    public override bool Equals(object? obj)
    {
        if (obj is null || obj.GetType() != GetType())
        {
            return false;
        }

        ValueObject valueObject = (ValueObject)obj;

        return GetEqualityComponents()
            .SequenceEqual(second: valueObject.GetEqualityComponents());
    }

    public static bool operator ==(ValueObject left, ValueObject right) => Equals(left, right);

    public static bool operator !=(ValueObject left, ValueObject right) => !Equals(left, right);

    public override int GetHashCode() =>
        GetEqualityComponents()
            .Select(x => x?.GetHashCode() ?? 0)
            .Aggregate((x, y) => x ^ y);
}
