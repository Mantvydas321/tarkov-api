namespace Tarkov.API.Database;

public class EntitiesCounter
{
    public int EntitiesUpdated { get; set; } = 0;
    public int EntitiesCreated { get; set; } = 0;
    public int EntitiesDeleted { get; set; } = 0;

    public void Add(EntitiesCounter counter)
    {
        EntitiesUpdated += counter.EntitiesUpdated;
        EntitiesCreated += counter.EntitiesCreated;
        EntitiesDeleted += counter.EntitiesDeleted;
    }
}