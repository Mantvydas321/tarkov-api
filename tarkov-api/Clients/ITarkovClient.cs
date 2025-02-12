using tarkov_api.Data;

namespace tarkov_api.Services;

public interface ITarkovClient
{
    public Task<List<AchievementDto>> GetSaveAchievements();
}