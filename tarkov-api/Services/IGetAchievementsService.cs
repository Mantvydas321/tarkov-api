using tarkov_api.Data;

namespace tarkov_api.Services;

public interface IGetAchievementsService
{
    public Task SaveAchievementsToDatabase();
}