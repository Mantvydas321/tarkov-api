using Microsoft.AspNetCore.Mvc;
using Tarkov.API.Infrastructure.Tasks;

namespace Tarkov.API.Controllers;

[ApiController]
[Route("[controller]")]
public class SyncController : ControllerBase
{
    private readonly AchievementsSyncTask _achievementsSyncTask;
    private readonly AchievementTranslationsSyncTask _achievementTranslationsSyncTask;

    public SyncController(AchievementsSyncTask achievementsSyncTask, AchievementTranslationsSyncTask achievementTranslationsSyncTask)
    {
        _achievementsSyncTask = achievementsSyncTask;
        _achievementTranslationsSyncTask = achievementTranslationsSyncTask;
    }

    [HttpPost("achievements")]
    public async Task SyncAchievements()
    {
        await _achievementsSyncTask.Run();
    }

    [HttpPost("achievements-translations")]
    public async Task SyncAchievementsTranslations()
    {
        await _achievementTranslationsSyncTask.Run();
    }
}