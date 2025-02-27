using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tarkov.API.Application.Exceptions;
using Tarkov.API.Application.Models;
using Tarkov.API.Database;

namespace Tarkov.API.Application.Queries;

public class TaskQueryRequest : IRequest<TaskData>
{
    [FromRoute]
    public Guid TaskId { get; set; }
}

public class TaskQueryHandler : IRequestHandler<TaskQueryRequest, TaskData>
{
    private readonly DatabaseContext _context;

    public TaskQueryHandler(DatabaseContext context)
    {
        _context = context;
    }

    public async Task<TaskData> Handle(TaskQueryRequest request, CancellationToken cancellationToken)
    {
        var task = await _context.Tasks.FirstOrDefaultAsync(e => e.Id == request.TaskId, cancellationToken);
        if (task == null)
        {
            throw new NotFoundException("Task", request.TaskId.ToString());
        }

        return task.AsData();
    }
}