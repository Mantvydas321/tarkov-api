using MediatR;
using Microsoft.AspNetCore.Mvc;
using Tarkov.API.Application.Commands;
using Tarkov.API.Application.Models;
using Tarkov.API.Application.Queries;

namespace Tarkov.API.Controllers;

[ApiController]
[Route("api/v1/tasks")]
public class TasksController : ControllerBase
{
    private readonly IMediator _mediator;

    public TasksController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<Page<TaskData>> GetTasks(TasksQueryRequest request)
    {
        return await _mediator.Send(request);
    }

    [HttpGet("{TaskId}")]
    public async Task<TaskData> GetTask(TaskQueryRequest request)
    {
        return await _mediator.Send(request);
    }

    [HttpPost("{TaskId}/schedule")]
    public async Task ScheduleTask(ScheduledTaskCommand command)
    {
        await _mediator.Send(command);
    }

    [HttpPost("{TaskId}/start")]
    public async Task StartTask([FromRoute(Name = "TaskId")] Guid taskId)
    {
        var command = new ScheduledTaskCommand()
        {
            TaskId = taskId,
            Body = new() { ScheduledTime = DateTime.UtcNow + TimeSpan.FromSeconds(5) }
        };

        await _mediator.Send(command);
    }
}