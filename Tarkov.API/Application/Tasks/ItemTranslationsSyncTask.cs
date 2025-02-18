using MediatR;
using Microsoft.EntityFrameworkCore;
using Tarkov.API.Application.Client.Queries;
using Tarkov.API.Database;
using Tarkov.API.Database.Entities;
using Tarkov.API.Database.Enumeration;

namespace Tarkov.API.Application.Tasks;

public class ItemTranslationsSyncTask : ISyncTask
{
    private const int BatchSize = 100;

    private readonly IServiceProvider _serviceProvider;
    private readonly IMediator _mediator;
    private readonly ILogger<AchievementTranslationsSyncTask> _logger;

    public ItemTranslationsSyncTask(IServiceProvider serviceProvider, IMediator mediator, ILogger<AchievementTranslationsSyncTask> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _mediator = mediator;
    }

    public EntitiesCounter EntitiesCounter { get; } = new();

    public async Task Run(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Synchronizing item translations");

        foreach (var lang in Enum.GetValues<LanguageCode>())
        {
            for (int offset = 0; await FetchBatch(lang, offset, cancellationToken) && !cancellationToken.IsCancellationRequested; offset += BatchSize) ;
        }

        _logger.LogInformation("Item translations synchronized");
    }

    private async Task<bool> FetchBatch(LanguageCode lang, int offset, CancellationToken cancellationToken = default)
    {
        await using var scope = _serviceProvider.CreateAsyncScope();
        await using var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
        context.Counter = EntitiesCounter;

        _logger.LogInformation("Fetching item translations for {Language} {Start} to {End}", lang, offset, offset + BatchSize);
        var items = (
            await _mediator.Send(new ItemTranslationsClientRequest() { Lang = lang, Limit = BatchSize, Offset = offset }, cancellationToken)
        ).Items;

        if (items.Count == 0)
        {
            return false;
        }

        var itemIds = items
            .Select(e => e.Id)
            .ToHashSet();

        var itemEntities = await context
            .Items
            .AsSplitQuery()
            .Include(e => e.Translations.Where(t => t.Language == lang))
            .Where(e => itemIds.Contains(e.Id))
            .ToDictionaryAsync(e => e.Id, cancellationToken: cancellationToken);

        foreach (var item in items)
        {
            if (!itemEntities.TryGetValue(item.Id, out var itemEntity))
            {
                _logger.LogWarning("Item {Key} not found in database, skipping translation", item.Id);
                continue;
            }

            if (item.Name != null)
            {
                UpdateTranslation(itemEntity, item.Name, lang, ItemTranslationEntityField.Name);
            }

            if (item.Description != null)
            {
                UpdateTranslation(itemEntity, item.Description, lang, ItemTranslationEntityField.Description);
            }
        }

        await context.SaveChangesAsync(cancellationToken);
        return items.Count == BatchSize;
    }

    private void UpdateTranslation(ItemEntity entity, string value, LanguageCode lang, ItemTranslationEntityField field)
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
            nameTranslation = new ItemTranslationEntity(
                entity.Id,
                lang,
                field,
                value
            );
            entity.Translations.Add(nameTranslation);
        }
    }
}