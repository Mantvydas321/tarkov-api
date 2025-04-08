using System.ComponentModel.DataAnnotations;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tarkov.API.Application.Models;
using Tarkov.API.Database;
using Tarkov.API.Database.Entities;
using Tarkov.API.Database.Enumeration;

namespace Tarkov.API.Application.Queries;

public class ItemQuery : IRequest<Page<ItemData>>
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

public class ItemQueryHandler : IRequestHandler<ItemQuery, Page<ItemData>>
{
    private readonly DatabaseContext _context;

    public ItemQueryHandler(DatabaseContext context)
    {
        _context = context;
    }

    public async Task<Page<ItemData>> Handle(ItemQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Items
            .Include(e => e.Translations)
            .AsSplitQuery()
            .AsNoTracking()
            .AsQueryable();

        var items = await query
            .OrderBy(e => e.Id)
            .Skip(request.Offset)
            .Take(request.Limit)
            .ToListAsync(cancellationToken: cancellationToken);

        var tasks = items.Select(t => t.AsData(request.Language)).ToList();

        var total = await _context.Items.CountAsync(cancellationToken: cancellationToken);

        return new Page<ItemData>()
        {
            Items = tasks,
            Total = total
        };
    }
}