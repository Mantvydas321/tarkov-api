using Microsoft.EntityFrameworkCore;
using Tarkov.API.Database;
using Tarkov.API.Database.Entities;
using Tarkov.API.Database.Enumeration;

namespace Tarkov.API.Application.Tasks;

public abstract class AbstractSyncTask : ISyncTask
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
            _context.TranslationKeys.Add(new TranslationKeyEntity(key));
        }
    }

    protected async Task UpdateTranslations<T>(
        LanguageCode lang,
        ICollection<T> collection,
        Func<T, string> translationKeySelector,
        Func<T, string> valueSelector
    )
    {
        var translationKeys = collection
            .Select(translationKeySelector)
            .ToHashSet();

        var existingTranslations = await _context
            .Translations
            .Where(e => translationKeys.Contains(e.Key) && e.Language == lang)
            .ToDictionaryAsync(e => e.Key);

        foreach (var entity in collection)
        {
            var key = translationKeySelector(entity);
            if (existingTranslations.TryGetValue(key, out var existing))
            {
                existing.UpdateValue(valueSelector(entity));
            }
            else
            {
                _logger.LogInformation("Inserting new translation {Key} for {Language}", key, lang);
                _context.Translations.Add(new TranslationEntity(key, lang, valueSelector(entity)));
            }
        }
    }

    public abstract Task Run(CancellationToken cancellationToken = default);
}