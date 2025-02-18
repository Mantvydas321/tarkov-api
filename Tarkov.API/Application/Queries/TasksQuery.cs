using System.ComponentModel.DataAnnotations;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tarkov.API.Application.Models;
using Tarkov.API.Database;

namespace Tarkov.API.Application.Queries;

public class TasksQueryRequest : IRequest<Page<TaskData>>
{
    [FromQuery]
    [Range(0, int.MaxValue)]
    public int Offset { get; set; } = 0;

    [FromQuery]
    [Range(1, 100)]
    public int Limit { get; set; } = 100;

    public bool? LastRunSuccessful { get; set; }
}

public class TasksQueryHandler : IRequestHandler<TasksQueryRequest, Page<TaskData>>
{
    private readonly DatabaseContext _context;

    public TasksQueryHandler(DatabaseContext context)
    {
        _context = context;
    }

    public async Task<Page<TaskData>> Handle(TasksQueryRequest request, CancellationToken cancellationToken)
    {
        var query = _context.Tasks
            .AsQueryable();

        if (request.LastRunSuccessful == true)
        {
            query = query.Where(t => t.LastRunSuccessful == request.LastRunSuccessful.Value && t.LastRun != null);
        }
        else if (request.LastRunSuccessful == false)
        {
            query = query.Where(t => t.LastRunSuccessful == request.LastRunSuccessful);
        }

        var tasks = await query
            .OrderBy(e => e.Id)
            .Skip(request.Offset)
            .Take(request.Limit)
            .Select(t => t.AsData())
            .ToListAsync(cancellationToken: cancellationToken);

        var total = await query.CountAsync(cancellationToken: cancellationToken);

        return new Page<TaskData>()
        {
            Items = tasks,
            Total = total
        };
    }
}