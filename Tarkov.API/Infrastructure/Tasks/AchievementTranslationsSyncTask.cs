using Microsoft.EntityFrameworkCore;
using Tarkov.API.Database;
using Tarkov.API.Database.Entities;
using Tarkov.API.Database.Enumeration;
using Tarkov.API.Infrastructure.Clients;
using Tarkov.API.Infrastructure.Clients.Queries;

namespace Tarkov.API.Infrastructure.Tasks;

public class AchievementTranslationsSyncTask : AbstractSyncTask
{
    private const int BatchSize = 100;

    private readonly DatabaseContext _context;
    private readonly TarkovClient _client;
    private readonly ILogger<AchievementTranslationsSyncTask> _logger;

    public AchievementTranslationsSyncTask(DatabaseContext context, TarkovClient client, ILogger<AchievementTranslationsSyncTask> logger) : base(context,
        logger)
    {
        _context = context;
        _client = client;
        _logger = logger;
    }

    public async Task Run()
    {
        _logger.LogInformation("Synchronizing achievement translations");

        foreach (var lang in Enum.GetValues<LanguageCode>())
        {
            for (int offset = 0;; offset += BatchSize)
            {
                _logger.LogInformation("Fetching achievements translations for {Language} {Start} to {End}", lang, offset, offset + BatchSize);
                var achievements = await _client.AchievementTranslations(lang, offset, BatchSize);

                if (achievements.Count == 0)
                {
                    break;
                }

                await InsertMissingTranslationKeys(achievements
                    .Select(e => TranslationKey.Achievement.Name(e.Id))
                    .ToHashSet()
                );

                await InsertMissingTranslationKeys(achievements
                    .Select(e => TranslationKey.Achievement.Description(e.Id))
                    .ToHashSet()
                );

                await UpdateNameTranslations(achievements, lang);
                await UpdateDescriptionTranslations(achievements, lang);

                await _context.SaveChangesAsync();

                if (achievements.Count < BatchSize)
                {
                    break;
                }
            }
        }

        _logger.LogInformation("Achievement translations synchronized");
    }

    private async Task UpdateNameTranslations(List<AchievementTranslationsQuery.AchievementTranslation> achievements, LanguageCode lang)
    {
        var translationKeys = achievements
            .Select(e => TranslationKey.Achievement.Name(e.Id))
            .ToHashSet();

        var existingTranslations = await _context
            .Translations
            .Where(e => translationKeys.Contains(e.Key) && e.Language == lang)
            .ToDictionaryAsync(e => e.Key);

        foreach (var achievement in achievements)
        {
            if (existingTranslations.TryGetValue(achievement.Id, out var existing))
            {
                existing.Value = achievement.Name;
                continue;
            }

            var key = TranslationKey.Achievement.Name(achievement.Id);
            _logger.LogInformation("Inserting new achievement translation {Key} for {Language}", key, lang);
            _context.Translations.Add(new TranslationEntity
            {
                Key = key,
                Language = lang,
                Value = achievement.Name
            });
        }
    }

    private async Task UpdateDescriptionTranslations(List<AchievementTranslationsQuery.AchievementTranslation> achievements, LanguageCode lang)
    {
        var translationKeys = achievements
            .Select(e => TranslationKey.Achievement.Description(e.Id))
            .ToHashSet();

        var existingTranslations = await _context
            .Translations
            .Where(e => translationKeys.Contains(e.Key) && e.Language == lang)
            .ToDictionaryAsync(e => e.Key);

        foreach (var achievement in achievements)
        {
            if (existingTranslations.TryGetValue(achievement.Id, out var existing))
            {
                existing.Value = achievement.Description;
                continue;
            }

            var key = TranslationKey.Achievement.Description(achievement.Id);
            _logger.LogInformation("Inserting new achievement translation {Key} for {Language}", key, lang);
            _context.Translations.Add(new TranslationEntity
            {
                Key = key,
                Language = lang,
                Value = achievement.Description
            });
        }
    }
}