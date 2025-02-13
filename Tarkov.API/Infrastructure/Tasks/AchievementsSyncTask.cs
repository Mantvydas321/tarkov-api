using Microsoft.EntityFrameworkCore;
using Tarkov.API.Database;
using Tarkov.API.Database.Entities;
using Tarkov.API.Infrastructure.Clients;
using Tarkov.API.Infrastructure.Clients.Queries;

namespace Tarkov.API.Infrastructure.Tasks;

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

    public async Task Run()
    {
        _logger.LogInformation("Synchronizing achievements");

        for (int offset = 0;; offset += BatchSize)
        {
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

            var ids = achievements.Select(e => e.Id).ToHashSet();
            var existingAchievements = await _context.Achievements
                .Where(e => ids.Contains(e.Id))
                .ToDictionaryAsync(e => e.Id);

            foreach (var achievement in achievements)
            {
                if (existingAchievements.TryGetValue(achievement.Id, out var existing))
                {
                    existing.Hidden = achievement.Hidden;
                    existing.Side = achievement.NormalizedSide;
                    existing.Rarity = achievement.NormalizedRarity;
                    existing.PlayersCompletedPercentage = achievement.PlayersCompletedPercent;
                    existing.AdjustedPlayersCompletedPercentage = achievement.AdjustedPlayersCompletedPercent;
                    continue;
                }

                _logger.LogInformation("Inserting new achievement {Id}", achievement.Id);

                _context.Achievements.Add(new AchievementEntity
                {
                    Id = achievement.Id,
                    NameTranslationKey = TranslationKey.Achievement.Name(achievement.Id),
                    DescriptionTranslationKey = TranslationKey.Achievement.Description(achievement.Id),
                    Hidden = achievement.Hidden,
                    Side = achievement.NormalizedSide,
                    Rarity = achievement.NormalizedRarity,
                    PlayersCompletedPercentage = achievement.PlayersCompletedPercent,
                    AdjustedPlayersCompletedPercentage = achievement.AdjustedPlayersCompletedPercent,
                });
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