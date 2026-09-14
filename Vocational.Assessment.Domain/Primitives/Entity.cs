public abstract class Entity<TId>
{
    public TId Id { get; protected set; }

    protected Entity(TId id) => Id = id;

    //se sobrescriben Equals y GetHashCode para comparar por Id
}