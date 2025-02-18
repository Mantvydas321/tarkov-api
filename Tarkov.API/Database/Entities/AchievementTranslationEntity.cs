using Tarkov.API.Database.Enumeration;

namespace Tarkov.API.Database.Entities;

public enum AchievementTranslationField
{
    Name = 1,
    Description = 2,
}

public class AchievementTranslationEntity : IMutableEntity
{
    public string AchievementId { get; protected set; }
    public AchievementEntity? Achievement { get; protected set; }

    public LanguageCode Language { get; protected set; }
    public string Value { get; protected set; }

    public AchievementTranslationField Field { get; protected set; }

    public DateTime? CreatedDate { get; set; }
    public DateTime? ModifiedDate { get; set; }

    // EF Core constructor
#pragma warning disable CS8618, CS9264
    protected AchievementTranslationEntity()
    {
    }
#pragma warning restore CS8618, CS9264

    public AchievementTranslationEntity(string achievementId, LanguageCode language, AchievementTranslationField field, string value)
    {
        AchievementId = achievementId;
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