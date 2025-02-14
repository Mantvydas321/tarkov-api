using Tarkov.API.Database.Enumeration;

namespace Tarkov.API.Database.Entities;

public class TranslationEntity : IMutableEntity
{
    public string Key { get; protected set; }
    public TranslationKeyEntity? KeyEntity { get; protected set; }

    public LanguageCode Language { get; protected set; }
    public string Value { get; protected set; }

    public DateTime? CreatedDate { get; set; }
    public DateTime? ModifiedDate { get; set; }

    // EF Core constructor
#pragma warning disable CS8618, CS9264
    protected TranslationEntity()
    {
    }
#pragma warning restore CS8618, CS9264

    public TranslationEntity(string key, LanguageCode language, string value)
    {
        Key = key;
        Language = language;
        Value = value;
    }

    public void UpdateValue(string value)
    {
        if (Value == value)
            return;

        Value = value;
    }
}