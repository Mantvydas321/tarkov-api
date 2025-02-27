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

    private readonly IServiceProvider _serviceProvider;
    private readonly IMediator _mediator;
    private readonly ILogger<AchievementTranslationsSyncTask> _logger;

    public AchievementTranslationsSyncTask(IServiceProvider serviceProvider, IMediator mediator, ILogger<AchievementTranslationsSyncTask> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _mediator = mediator;
    }

    public EntitiesCounter EntitiesCounter { get; } = new();

    public async Task Run(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Synchronizing achievement translations");

        foreach (var lang in Enum.GetValues<LanguageCode>())
        {
            for (int offset = 0; await FetchBatch(lang, offset, cancellationToken) && !cancellationToken.IsCancellationRequested; offset += BatchSize) ;
        }

        _logger.LogInformation("Achievement translations synchronized");
    }

    private async Task<bool> FetchBatch(LanguageCode lang, int offset, CancellationToken cancellationToken = default)
    {
        await using var scope = _serviceProvider.CreateAsyncScope();
        await using var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
        context.Counter = EntitiesCounter;

        _logger.LogInformation("Fetching achievements translations for {Language} {Start} to {End}", lang, offset, offset + BatchSize);
        var achievements = (
            await _mediator.Send(new AchievementTranslationsClientRequest() { Lang = lang, Limit = BatchSize, Offset = offset }, cancellationToken)
        ).Achievements;

        if (achievements.Count == 0)
        {
            return false;
        }

        var achievementIds = achievements
            .Select(e => e.Id)
            .ToHashSet();

        var achievementEntities = await context
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

            if (achievement.Name != null)
            {
                UpdateTranslation(achievementEntity, achievement.Name, lang, AchievementTranslationField.Name);
            }

            if (achievement.Description != null)
            {
                UpdateTranslation(achievementEntity, achievement.Description, lang, AchievementTranslationField.Description);
            }
        }

        await context.SaveChangesAsync(cancellationToken);
        return achievements.Count == BatchSize;
    }

    private void UpdateTranslation(AchievementEntity entity, string value, LanguageCode lang, AchievementTranslationField field)
    {
        var nameTranslation = entity.Translations.FirstOrDefault(
            e => e.Language == lang && e.Field == field
        );
        if (nameTranslation != null)
        {
            nameTranslation.UpdateValue(value);
        }
        else
        {
            nameTranslation = new AchievementTranslationEntity(
                entity.Id,
                lang,
                field,
                value
            );
            entity.Translations.Add(nameTranslation);
        }
    }
}