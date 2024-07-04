using Erdmier.DomainCore.Attributes;

namespace Erdmier.DomainCore.Models;

[ Entity ]
public abstract class Entity<TId> : IEquatable<Entity<TId>>
    where TId : ValueObject
{
    protected Entity(TId id) => Id = id;

    protected Entity()
    { }

    public TId Id { get; protected init; } = default!;

    public bool Equals(Entity<TId>? other) => Equals(obj: other);

    public override bool Equals(object? obj) => obj is Entity<TId> entity && Id.Equals(entity.Id);

    public static bool operator ==(Entity<TId> left, Entity<TId> right) => Equals(left, right);

    public static bool operator !=(Entity<TId> left, Entity<TId> right) => !Equals(left, right);

    public override int GetHashCode() => Id.GetHashCode();
}
