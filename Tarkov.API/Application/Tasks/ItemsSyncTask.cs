using MediatR;
using Microsoft.EntityFrameworkCore;
using Tarkov.API.Application.Client.Queries;
using Tarkov.API.Database;
using Tarkov.API.Database.Entities;

namespace Tarkov.API.Application.Tasks;

public class ItemsSyncTask : ISyncTask
{
    private const int BatchSize = 100;

    private readonly IServiceProvider _serviceProvider;
    private readonly IMediator _mediator;
    private readonly ILogger<AchievementsSyncTask> _logger;

    public ItemsSyncTask(IServiceProvider serviceProvider, IMediator mediator, ILogger<AchievementsSyncTask> logger)
    {
        _serviceProvider = serviceProvider;
        _mediator = mediator;
        _logger = logger;
    }

    public EntitiesCounter EntitiesCounter { get; } = new();

    public async Task Run(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Synchronizing items");

        for (int offset = 0; await FetchBatch(offset, cancellationToken) && !cancellationToken.IsCancellationRequested; offset += BatchSize) ;

        _logger.LogInformation("Items synchronized");
    }

    private async Task<bool> FetchBatch(int offset, CancellationToken cancellationToken = default)
    {
        await using var scope = _serviceProvider.CreateAsyncScope();
        await using var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
        context.Counter = EntitiesCounter;

        _logger.LogInformation("Fetching items {Start} to {End}", offset, offset + BatchSize);

        var items = (
            await _mediator.Send(new ItemsClientRequest() { Limit = BatchSize, Offset = offset }, cancellationToken)
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
            .Include(e => e.Types)
            .Where(e => itemIds.Contains(e.Id))
            .ToDictionaryAsync(e => e.Id, cancellationToken: cancellationToken);

        var itemTypes = await context
            .ItemTypes
            .AsSplitQuery()
            .ToListAsync(cancellationToken);

        foreach (var item in items)
        {
            if (item.ConflictingSlotIds.Length > 1)
            {
                _logger.LogWarning("Conflicting slot ids are not supported, found {Count} conflicting slot ids for item {Key}",
                    item.ConflictingSlotIds.Length, item.Id);
            }

            if (itemEntities.TryGetValue(item.Id, out var itemEntity))
            {
                itemEntity.Update(item);
            }
            else
            {
                itemEntity = new ItemEntity(item);
                context.Items.Add(itemEntity);
            }

            foreach (var type in item.Types)
            {
                if (itemEntity.Types.Any(x => x.Name == type))
                {
                    continue;
                }

                var typeEntity = itemTypes.FirstOrDefault(x => x.Name == type);
                if (typeEntity == null)
                {
                    typeEntity = new ItemTypeEntity(type);
                    itemTypes.Add(typeEntity);
                }

                itemEntity.Types.Add(typeEntity);
            }
        }

        await context.SaveChangesAsync(cancellationToken);
        return items.Count == BatchSize;
    }
}