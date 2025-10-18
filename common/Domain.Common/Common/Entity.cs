
namespace Domain.Common;

public abstract class Entity<TKey> : IEquatable<Entity<TKey>>
{
    protected Entity(TKey id) : this()
    {
        Id = id;
    }

    protected Entity()
    {
    }

    public TKey Id { get; private set; }


    public override bool Equals(object obj)
    {
        return Equals(obj as Entity<TKey>);
    }

    public bool Equals(Entity<TKey> other)
    {
        return other != null &&
               EqualityComparer<TKey>.Default.Equals(Id, other.Id);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine<TKey>(Id);
    }

    public static bool operator ==(Entity<TKey> entity1, Entity<TKey> entity2)
    {
        return EqualityComparer<Entity<TKey>>.Default.Equals(entity1, entity2);
    }

    public static bool operator !=(Entity<TKey> entity1, Entity<TKey> entity2)
    {
        return !(entity1 == entity2);
    }
}