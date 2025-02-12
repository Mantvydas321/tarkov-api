using tarkov_api.Data;

namespace tarkov_api.Services;

public interface IAchievementsService
{
    public Task SaveAchievementsToDatabase(List<AchievementDto> achievements);
}