using Tarkov.API.Data;

namespace Tarkov.API.Services;

public interface IAchievementsService
{
    public Task SaveAchievementsToDatabase(List<AchievementDto> achievements);
}