using MediatR;
using Microsoft.AspNetCore.Mvc;
using Tarkov.API.Application.Commands;
using Tarkov.API.Application.Models;
using Tarkov.API.Application.Queries;

namespace Tarkov.API.Controllers;

[ApiController]
[Route("api/v1/achievements")]
public class AchievementsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AchievementsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<Page<AchievementData>> GetTasks(AchievementsQueryRequest request)
    {
        return await _mediator.Send(request);
    }
}