using Microsoft.EntityFrameworkCore;
using Tarkov.API.Data;
using Tarkov.API.Database;
using Tarkov.API.Database.Entities;

namespace Tarkov.API.Services;

public class AchievementsService : IAchievementsService
{
    private readonly DatabaseContext _context;

    public AchievementsService(DatabaseContext context)
    {
        _context = context;
    }

    public async Task SaveAchievementsToDatabase(List<AchievementDto> achievements)
    {
        foreach (var achievement in achievements)
        {
            var existingAchievement = await _context.Achievements.FirstOrDefaultAsync(a => a.Name == achievement.Name);
            if (existingAchievement != null)
            {
                continue;
            }

            var entity = new AchievementEntity
            {
                Id = Guid.NewGuid(),
                Name = achievement.Name,
                Description = achievement.Description,
                Hidden = achievement.Hidden,
                PlayersCompletedPercentage = achievement.PlayersCompletedPercentage,
                Side = achievement.Side,
                Rarity = achievement.Rarity
            };

            _context.Achievements.Add(entity);
        }

        await _context.SaveChangesAsync();
    }
}