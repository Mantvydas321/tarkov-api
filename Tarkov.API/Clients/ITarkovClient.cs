using Tarkov.API.Data;

namespace Tarkov.API.Clients;

public interface ITarkovClient
{
    public Task<List<AchievementDto>> GetSaveAchievements();
}