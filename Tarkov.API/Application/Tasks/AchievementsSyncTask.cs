using MediatR;
using Microsoft.EntityFrameworkCore;
using Tarkov.API.Application.Client.Queries;
using Tarkov.API.Database;
using Tarkov.API.Database.Entities;

namespace Tarkov.API.Application.Tasks;

public class AchievementsSyncTask : ISyncTask
{
    private const int BatchSize = 100;

    private readonly IServiceProvider _serviceProvider;
    private readonly IMediator _mediator;
    private readonly ILogger<AchievementsSyncTask> _logger;

    public AchievementsSyncTask(IServiceProvider serviceProvider, IMediator mediator, ILogger<AchievementsSyncTask> logger)
    {
        _serviceProvider = serviceProvider;
        _mediator = mediator;
        _logger = logger;
    }

    public EntitiesCounter EntitiesCounter { get; } = new();

    public async Task Run(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Synchronizing achievements");

        for (int offset = 0; await FetchBatch(offset, cancellationToken) && !cancellationToken.IsCancellationRequested; offset += BatchSize) ;

        _logger.LogInformation("Achievements synchronized");
    }

    private async Task<bool> FetchBatch(int offset, CancellationToken cancellationToken = default)
    {
        await using var scope = _serviceProvider.CreateAsyncScope();
        await using var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
        context.Counter = EntitiesCounter;

        _logger.LogInformation("Fetching achievements {Start} to {End}", offset, offset + BatchSize);

        var achievements = (
            await _mediator.Send(new AchievementClientRequest() { Limit = BatchSize, Offset = offset }, cancellationToken)
        ).Achievements;

        if (achievements.Count == 0)
        {
            return false;
        }

        var ids = achievements.Select(e => e.Id).ToHashSet();
        var existingAchievements = await context.Achievements
            .Where(e => ids.Contains(e.Id))
            .ToDictionaryAsync(e => e.Id, cancellationToken: cancellationToken);

        foreach (var achievement in achievements)
        {
            if (existingAchievements.TryGetValue(achievement.Id, out var existing))
            {
                existing.UpdateHidden(achievement.Hidden);
                existing.UpdateSide(achievement.NormalizedSide);
                existing.UpdateRarity(achievement.NormalizedRarity);
                existing.UpdatePlayersCompletedPercentage(achievement.PlayersCompletedPercent);
                existing.UpdateAdjustedPlayersCompletedPercentage(achievement.AdjustedPlayersCompletedPercent);
                continue;
            }

            _logger.LogInformation("Inserting new achievement {Id}", achievement.Id);

            context.Achievements.Add(new AchievementEntity(
                achievement.Id,
                achievement.Hidden,
                achievement.NormalizedSide,
                achievement.NormalizedRarity,
                achievement.PlayersCompletedPercent,
                achievement.AdjustedPlayersCompletedPercent
            ));
        }

        await context.SaveChangesAsync(cancellationToken);
        return achievements.Count == BatchSize;
    }
}