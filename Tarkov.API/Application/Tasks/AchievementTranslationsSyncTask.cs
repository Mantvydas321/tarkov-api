using MediatR;
using Microsoft.EntityFrameworkCore;
using Tarkov.API.Application.Client.Queries;
using Tarkov.API.Database;
using Tarkov.API.Database.Entities;
using Tarkov.API.Database.Enumeration;

namespace Tarkov.API.Application.Tasks;

public class AchievementTranslationsSyncTask : ISyncTask
{
    private const int BatchSize = 100;

    private readonly DatabaseContext _context;
    private readonly IMediator _mediator;
    private readonly ILogger<AchievementTranslationsSyncTask> _logger;

    public AchievementTranslationsSyncTask(DatabaseContext context, IMediator mediator, ILogger<AchievementTranslationsSyncTask> logger)
    {
        _context = context;
        _logger = logger;
        _mediator = mediator;
    }

    public async Task Run(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Synchronizing achievement translations");

        foreach (var lang in Enum.GetValues<LanguageCode>())
        {
            for (int offset = 0;; offset += BatchSize)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }

                _logger.LogInformation("Fetching achievements translations for {Language} {Start} to {End}", lang, offset, offset + BatchSize);
                var achievements = (
                    await _mediator.Send(new AchievementTranslationsClientRequest() { Lang = lang, Limit = BatchSize, Offset = offset }, cancellationToken)
                ).Achievements;

                if (achievements.Count == 0)
                {
                    break;
                }

                var achievementIds = achievements
                    .Select(e => e.Id)
                    .ToHashSet();

                var achievementEntities = await _context
                    .Achievements
                    .AsSplitQuery()
                    .Include(e => e.Translations.Where(t => t.Language == lang))
                    .Where(e => achievementIds.Contains(e.Id))
                    .ToDictionaryAsync(e => e.Id, cancellationToken: cancellationToken);

                foreach (var achievement in achievements)
                {
                    if (!achievementEntities.TryGetValue(achievement.Id, out var achievementEntity))
                    {
                        _logger.LogWarning("Achievement {Key} not found in database, skipping translation", achievement.Id);
                        continue;
                    }

                    var nameTranslation = achievementEntity.Translations.FirstOrDefault(
                        e => e.Language == lang && e.Field == AchievementTranslationField.Name
                    );
                    if (nameTranslation != null)
                    {
                        nameTranslation.UpdateValue(achievement.Name);
                    }
                    else
                    {
                        nameTranslation = new AchievementTranslationEntity(
                            achievement.Id,
                            lang,
                            AchievementTranslationField.Name,
                            achievement.Name
                        );
                        achievementEntity.Translations.Add(nameTranslation);
                    }

                    var descriptionTranslation = achievementEntity.Translations.FirstOrDefault(
                        e => e.Language == lang && e.Field == AchievementTranslationField.Description
                    );
                    if (descriptionTranslation != null)
                    {
                        descriptionTranslation.UpdateValue(achievement.Description);
                    }
                    else
                    {
                        descriptionTranslation = new AchievementTranslationEntity(
                            achievement.Id,
                            lang,
                            AchievementTranslationField.Description,
                            achievement.Description
                        );
                        achievementEntity.Translations.Add(descriptionTranslation);
                    }
                }

                await _context.SaveChangesAsync();

                if (achievements.Count < BatchSize)
                {
                    break;
                }
            }
        }

        _logger.LogInformation("Achievement translations synchronized");
    }
}