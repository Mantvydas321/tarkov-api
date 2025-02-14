using System.ComponentModel.DataAnnotations;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tarkov.API.Application.Exceptions;
using Tarkov.API.Database;

namespace Tarkov.API.Application.Commands;

public class ScheduledTaskCommand : IRequest<Unit>
{
    [Required]
    [FromRoute]
    public required Guid TaskId { get; set; }

    [Required]
    [FromBody]
    public required CommandBody Body { get; set; }

    public class CommandBody
    {
        [Required]
        [FromBody]
        public required DateTime ScheduledTime { get; set; }
    }
}

public class ScheduledTaskCommandHandler : IRequestHandler<ScheduledTaskCommand, Unit>
{
    private readonly DatabaseContext _context;
    private readonly ILogger<ScheduledTaskCommandHandler> _logger;

    public ScheduledTaskCommandHandler(DatabaseContext context, ILogger<ScheduledTaskCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Unit> Handle(ScheduledTaskCommand request, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        if (request.Body.ScheduledTime < now)
        {
            throw new BadRequestException("Scheduled time must be in the future");
        }

        if (request.Body.ScheduledTime > now.AddHours(24))
        {
            throw new BadRequestException("Scheduled time must be within 24 hours");
        }

        var task = await _context.Tasks.FirstOrDefaultAsync(e => e.Id == request.TaskId, cancellationToken);
        if (task == null)
        {
            throw new NotFoundException("Task", request.TaskId.ToString());
        }

        task.UpdateNextScheduledRun(request.Body.ScheduledTime);

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Scheduled task {TaskName} for {ScheduledTime}", task.Name, task.NextScheduledRun);

        return Unit.Value;
    }
}