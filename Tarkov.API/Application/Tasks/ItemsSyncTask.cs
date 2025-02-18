using MediatR;
using Microsoft.EntityFrameworkCore;
using Tarkov.API.Application.Client.Queries;
using Tarkov.API.Database;
using Tarkov.API.Database.Entities;

namespace Tarkov.API.Application.Tasks;

public class ItemsSyncTask : ISyncTask
{
    private const int BatchSize = 100;

    private readonly DatabaseContext _context;
    private readonly IMediator _mediator;
    private readonly ILogger<AchievementsSyncTask> _logger;

    public ItemsSyncTask(DatabaseContext context, IMediator mediator, ILogger<AchievementsSyncTask> logger)
    {
        _context = context;
        _mediator = mediator;
        _logger = logger;
    }

    public async Task Run(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Synchronizing items");

        for (int offset = 0;; offset += BatchSize)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                break;
            }

            _logger.LogInformation("Fetching items {Start} to {End}", offset, offset + BatchSize);
            var items = (
                await _mediator.Send(new ItemsClientRequest() { Limit = BatchSize, Offset = offset }, cancellationToken)
            ).Items;

            if (items.Count == 0)
            {
                break;
            }

            var itemIds = items
                .Select(e => e.Id)
                .ToHashSet();

            var itemEntities = await _context
                .Items
                .AsSplitQuery()
                .Include(e => e.Types)
                .Where(e => itemIds.Contains(e.Id))
                .ToDictionaryAsync(e => e.Id, cancellationToken: cancellationToken);

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
                    _context.Items.Add(itemEntity);
                }

                foreach (var type in item.Types)
                {
                    if (itemEntity.Types.Any(x => x.Name == type))
                    {
                        continue;
                    }
                    
                    var typeEntity = _context.ItemTypes.Local.FirstOrDefault(x => x.Name == type);
                    if (typeEntity != null)
                    {
                        itemEntity.Types.Add(typeEntity);
                    }
                    else
                    {
                        typeEntity = new ItemTypeEntity(type);
                        _context.ItemTypes.Add(typeEntity);
                        itemEntity.Types.Add(typeEntity);
                    }
                }
            }

            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();

            if (items.Count < BatchSize)
            {
                break;
            }
        }

        _logger.LogInformation("Items synchronized");
    }
}