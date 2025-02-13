namespace Tarkov.API.Database.Entities;

public class AchievementEntity
{
    public required string Id { get; set; }

    public required string NameTranslationKey { get; set; }
    public TranslationKeyEntity? NameTranslationKeyEntity { get; set; }

    public required string DescriptionTranslationKey { get; set; }
    public TranslationKeyEntity? DescriptionTranslationKeyEntity { get; set; }

    public required bool Hidden { get; set; }
    public required string Side { get; set; }
    public required string Rarity { get; set; }
    public required float PlayersCompletedPercentage { get; set; }
    public required float AdjustedPlayersCompletedPercentage { get; set; }
}