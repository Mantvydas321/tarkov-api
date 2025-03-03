using Microsoft.EntityFrameworkCore;
using Tarkov.API.Application.Tasks;
using Tarkov.API.Database.Entities;

namespace Tarkov.API.Database;

public class DatabaseContextSeed
{
    private readonly ILogger<DatabaseContextSeed> _logger;
    private readonly DatabaseContext _context;

    public DatabaseContextSeed(ILogger<DatabaseContextSeed> logger, DatabaseContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task SeedAsync()
    {
        // Apply any pending migrations
        await _context.Database.MigrateAsync();

        // Seed tasks
        var tasks = await _context.Tasks.ToDictionaryAsync(e => e.Id);
        foreach (var task in GetTasks())
        {
            if (tasks.TryGetValue(task.Id, out var existing))
            {
                existing.UpdateCronExpression(task.CronExpression);
                continue;
            }

            _context.Tasks.Add(task);
        }

        await _context.SaveChangesAsync();
    }

    private static TaskEntity[] GetTasks()
    {
        return
        [
            TaskEntity.Create<AchievementsSyncTask>(
                Guid.Parse("03586437-669b-427d-95cc-8ed9695bbdf4"),
                "0 0 * * 0" // At 00:00 on Sunday
            ),
            TaskEntity.Create<AchievementTranslationsSyncTask>(
                Guid.Parse("1d50d99a-2814-4863-aba9-64a7d0fb87b5"),
                "0 5 * * 0" // At 00:05 on Sunday
            ),
            TaskEntity.Create<ItemsSyncTask>(
                Guid.Parse("6797d62e-a662-45e3-81d8-5a885d871b20"),
                "0 15 * * 0" // At 00:15 on Sunday
            ),
            TaskEntity.Create<ItemTranslationsSyncTask>(
                Guid.Parse("353342a6-1d22-4821-b00d-8d5f6dd5f07e"),
                "0 20 * * 0" // At 00:20 on Sunday
            ),
        ];
    }
}