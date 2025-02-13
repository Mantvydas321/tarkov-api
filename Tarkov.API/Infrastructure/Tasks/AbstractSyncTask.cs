using Microsoft.EntityFrameworkCore;
using Tarkov.API.Database;
using Tarkov.API.Database.Entities;

namespace Tarkov.API.Infrastructure.Tasks;

public abstract class AbstractSyncTask
{
    private readonly DatabaseContext _context;
    private readonly ILogger _logger;

    protected AbstractSyncTask(DatabaseContext context, ILogger logger)
    {
        _context = context;
        _logger = logger;
    }

    protected async Task InsertMissingTranslationKeys(HashSet<string> keys)
    {
        var existingKeys = await _context
            .TranslationKeys
            .Select(e => e.Key)
            .Where(k => keys.Contains(k))
            .ToHashSetAsync();

        keys.ExceptWith(existingKeys);
        if (keys.Count == 0)
        {
            return;
        }

        _logger.LogInformation("Inserting {Count} missing translation keys", keys.Count);
        foreach (var key in keys)
        {
            _context.TranslationKeys.Add(new TranslationKeyEntity { Key = key });
        }
    }
}