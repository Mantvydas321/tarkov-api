namespace Tarkov.API.Database.Entities;

public class ItemTypeEntity : IImmutableEntity
{
    public const int MaxNameLength = 20;

    public string Name { get; protected set; }

    public DateTime? CreatedDate { get; set; }

    public List<ItemEntity> Items { get; protected set; } = new();

    // EF Core constructor
#pragma warning disable CS8618, CS9264
    protected ItemTypeEntity()
    {
    }
#pragma warning restore CS8618, CS9264

    public ItemTypeEntity(string name)
    {
        Name = name;
    }
}