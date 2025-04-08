using MediatR;
using Microsoft.AspNetCore.Mvc;
using Tarkov.API.Application.Models;
using Tarkov.API.Application.Queries;

namespace Tarkov.API.Controllers;

[ApiController]
[Route("api/v1/item")]
public class ItemController : ControllerBase
{
    private readonly IMediator _mediator;

    public ItemController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<Page<ItemData>> GetTasks(ItemQuery request)
    {
        return await _mediator.Send(request);
    }
}