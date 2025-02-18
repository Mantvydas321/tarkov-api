using Tarkov.API.Database.Enumeration;

namespace Tarkov.API.Database.Entities;

public enum ItemTranslationEntityField
{
    Name = 1,
    Description = 2,
}

public class ItemTranslationEntity : IMutableEntity
{
    public string ItemId { get; protected set; }
    public ItemEntity? Item { get; protected set; }

    public LanguageCode Language { get; protected set; }
    public string Value { get; protected set; }

    public ItemTranslationEntityField Field { get; protected set; }

    public DateTime? CreatedDate { get; set; }
    public DateTime? ModifiedDate { get; set; }

    // EF Core constructor
#pragma warning disable CS8618, CS9264
    protected ItemTranslationEntity()
    {
    }
#pragma warning restore CS8618, CS9264

    public ItemTranslationEntity(string itemId, LanguageCode language, ItemTranslationEntityField field, string value)
    {
        ItemId = itemId;
        Language = language;
        Field = field;
        Value = value;
    }

    public void UpdateValue(string value)
    {
        if (Value == value)
            return;

        Value = value;
    }
}