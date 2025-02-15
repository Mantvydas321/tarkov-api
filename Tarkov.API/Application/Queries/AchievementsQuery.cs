using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tarkov.API.Application.Models;
using Tarkov.API.Database;
using Tarkov.API.Database.Enumeration;

namespace Tarkov.API.Application.Queries;

public class AchievementsQueryRequest : IRequest<Page<AchievementData>>
{
    [FromQuery]
    [Range(0, int.MaxValue)]
    public int Offset { get; set; } = 0;

    [FromQuery]
    [Range(1, 100)]
    public int Limit { get; set; } = 100;

    [FromQuery]
    public LanguageCode Language { get; set; } = LanguageCode.EN;
}

public class AchievementsQueryHandler : IRequestHandler<AchievementsQueryRequest, Page<AchievementData>>
{
    private readonly DatabaseContext _context;

    public AchievementsQueryHandler(DatabaseContext context)
    {
        _context = context;
    }

    public async Task<Page<AchievementData>> Handle(AchievementsQueryRequest request, CancellationToken cancellationToken)
    {
        var query = _context.Achievements
            .Include(e => e.Translations)
            .AsQueryable();

        var tasks = await query
            .Skip(request.Offset)
            .Take(request.Limit)
            .Select(t => t.AsData(request.Language))
            .ToListAsync(cancellationToken: cancellationToken);

        var total = await query.CountAsync(cancellationToken: cancellationToken);

        return new Page<AchievementData>()
        {
            Items = tasks,
            Total = total
        };
    }
}