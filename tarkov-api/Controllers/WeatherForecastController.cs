using Microsoft.AspNetCore.Mvc;
using tarkov_api.Services;

namespace tarkov_api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        public IGetAchievementsService _getAchievementsService;

        public WeatherForecastController(IGetAchievementsService getAchievementsService)
        {
            _getAchievementsService = getAchievementsService;
        }

        [HttpPost()]
        public async void DoThis()
        {
            try
            {
                await _getAchievementsService.SaveAchievementsToDatabase();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
