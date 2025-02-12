using Microsoft.AspNetCore.Mvc;
using Tarkov.API.Clients;
using Tarkov.API.Services;

namespace Tarkov.API.Controllers;

[ApiController]
[Route("[controller]")]
public class SaveItemsToDbController : ControllerBase
{
    private readonly ITarkovClient _tarkovClient;
    private readonly IAchievementsService _achievementsService;

    public SaveItemsToDbController(IAchievementsService achievementsService, ITarkovClient tarkovClient)
    {
        _achievementsService = achievementsService;
        _tarkovClient = tarkovClient;
    }

    [HttpPost]
    public async Task<IActionResult> SaveAchievementsToDb()
    {
        var achievements = await _tarkovClient.GetSaveAchievements();
        await _achievementsService.SaveAchievementsToDatabase(achievements);
        return Ok();
    }
}