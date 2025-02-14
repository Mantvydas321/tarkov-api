using Microsoft.EntityFrameworkCore;
using Tarkov.API.Database;
using Tarkov.API.Database.Entities;
using Tarkov.API.Infrastructure.Clients;

namespace Tarkov.API.Application.Tasks;

public class AchievementsSyncTask : AbstractSyncTask
{
    private const int BatchSize = 100;

    private readonly DatabaseContext _context;
    private readonly TarkovClient _client;
    private readonly ILogger<AchievementsSyncTask> _logger;

    public AchievementsSyncTask(DatabaseContext context, TarkovClient client, ILogger<AchievementsSyncTask> logger) : base(context, logger)
    {
        _context = context;
        _client = client;
        _logger = logger;
    }

    public override async Task Run(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Synchronizing achievements");

        for (int offset = 0;; offset += BatchSize)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                break;
            }

            _logger.LogInformation("Fetching achievements {Start} to {End}", offset, offset + BatchSize);
            var achievements = await _client.Achievements(offset, BatchSize);

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
            
            await _context.SaveChangesAsync();

            var ids = achievements.Select(e => e.Id).ToHashSet();
            var existingAchievements = await _context.Achievements
                .Where(e => ids.Contains(e.Id))
                .ToDictionaryAsync(e => e.Id);

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

                _context.Achievements.Add(new AchievementEntity(
                    achievement.Id,
                    achievement.Hidden,
                    achievement.NormalizedSide,
                    achievement.NormalizedRarity,
                    achievement.PlayersCompletedPercent,
                    achievement.AdjustedPlayersCompletedPercent
                ));
            }

            await _context.SaveChangesAsync();

            if (achievements.Count < BatchSize)
            {
                break;
            }
        }

        _logger.LogInformation("Achievements synchronized");
    }
}