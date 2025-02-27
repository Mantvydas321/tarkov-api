using Tarkov.API.Application.Models;
using Tarkov.API.Database.Enumeration;

namespace Tarkov.API.Database.Entities;

public class AchievementEntity : IMutableEntity
{
    public const int MaxIdLength = 30;

    public string Id { get; protected set; }

    public bool Hidden { get; protected set; }
    public string Side { get; protected set; }
    public string Rarity { get; protected set; }
    public float PlayersCompletedPercentage { get; protected set; }
    public float AdjustedPlayersCompletedPercentage { get; protected set; }

    public List<AchievementTranslationEntity> Translations { get; set; } = new();

    public DateTime? CreatedDate { get; set; }
    public DateTime? ModifiedDate { get; set; }

    // EF Core constructor
#pragma warning disable CS8618, CS9264
    protected AchievementEntity()
    {
    }
#pragma warning restore CS8618, CS9264

    public AchievementEntity(string id, bool hidden, string side, string rarity, float playersCompletedPercentage, float adjustedPlayersCompletedPercentage)
    {
        Id = id;
        Hidden = hidden;
        Side = side;
        Rarity = rarity;
        PlayersCompletedPercentage = playersCompletedPercentage;
        AdjustedPlayersCompletedPercentage = adjustedPlayersCompletedPercentage;
    }

    public void UpdateHidden(bool hidden)
    {
        if (Hidden == hidden)
            return;

        Hidden = hidden;
    }

    public void UpdateSide(string side)
    {
        if (Side == side)
            return;

        Side = side;
    }

    public void UpdateRarity(string rarity)
    {
        if (Rarity == rarity)
            return;

        Rarity = rarity;
    }

    public void UpdatePlayersCompletedPercentage(float playersCompletedPercentage)
    {
        if (Math.Abs(PlayersCompletedPercentage - playersCompletedPercentage) < 0.0001)
            return;

        PlayersCompletedPercentage = playersCompletedPercentage;
    }

    public void UpdateAdjustedPlayersCompletedPercentage(float adjustedPlayersCompletedPercentage)
    {
        if (Math.Abs(AdjustedPlayersCompletedPercentage - adjustedPlayersCompletedPercentage) < 0.0001)
            return;

        AdjustedPlayersCompletedPercentage = adjustedPlayersCompletedPercentage;
    }

    public AchievementData AsData(LanguageCode lang)
    {
        var nameTranslation = Translations.FirstOrDefault(t => t.Language == lang && t.Field == AchievementTranslationField.Name);
        var descriptionTranslation = Translations.FirstOrDefault(t => t.Language == lang && t.Field == AchievementTranslationField.Description);

        return new AchievementData
        {
            Id = Id,
            Name = nameTranslation?.Value ?? string.Empty,
            Description = descriptionTranslation?.Value ?? string.Empty,
            Hidden = Hidden,
            Side = Side,
            Rarity = Rarity,
            PlayersCompletedPercentage = PlayersCompletedPercentage,
            AdjustedPlayersCompletedPercentage = AdjustedPlayersCompletedPercentage
        };
    }
}