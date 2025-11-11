namespace CampsiteBooking.Models.Common;

/// <summary>
/// Base class for all entities in the domain.
/// Entities have identity and are compared by their ID, not by their properties.
/// </summary>
/// <typeparam name="TId">The type of the entity's identifier</typeparam>
public abstract class Entity<TId> : IEquatable<Entity<TId>>
    where TId : notnull
{
    /// <summary>
    /// The unique identifier for this entity
    /// </summary>
    public TId Id { get; protected set; } = default!;

    /// <summary>
    /// Protected constructor to prevent direct instantiation.
    /// Entities should be created through factory methods or aggregate roots.
    /// </summary>
    protected Entity()
    {
    }

    /// <summary>
    /// Constructor that accepts an ID
    /// </summary>
    protected Entity(TId id)
    {
        Id = id;
    }

    /// <summary>
    /// Entities are equal if they have the same ID
    /// </summary>
    public bool Equals(Entity<TId>? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        if (GetType() != other.GetType()) return false;
        
        return EqualityComparer<TId>.Default.Equals(Id, other.Id);
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as Entity<TId>);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    public static bool operator ==(Entity<TId>? left, Entity<TId>? right)
    {
        if (left is null && right is null) return true;
        if (left is null || right is null) return false;
        return left.Equals(right);
    }

    public static bool operator !=(Entity<TId>? left, Entity<TId>? right)
    {
        return !(left == right);
    }
}

