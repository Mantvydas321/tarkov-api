using Microsoft.EntityFrameworkCore;
using Tarkov.API.Database;
using Tarkov.API.Database.Entities;
using Tarkov.API.Database.Enumeration;
using Tarkov.API.Infrastructure.Clients;
using Tarkov.API.Infrastructure.Clients.Queries;

namespace Tarkov.API.Application.Tasks;

public class AchievementTranslationsSyncTask : AbstractSyncTask
{
    private const int BatchSize = 100;

    private readonly DatabaseContext _context;
    private readonly TarkovClient _client;
    private readonly ILogger<AchievementTranslationsSyncTask> _logger;

    public AchievementTranslationsSyncTask(DatabaseContext context, TarkovClient client, ILogger<AchievementTranslationsSyncTask> logger) : base(context,
        logger)
    {
        _context = context;
        _client = client;
        _logger = logger;
    }

    public override async Task Run(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Synchronizing achievement translations");

        foreach (var lang in Enum.GetValues<LanguageCode>())
        {
            for (int offset = 0;; offset += BatchSize)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }

                _logger.LogInformation("Fetching achievements translations for {Language} {Start} to {End}", lang, offset, offset + BatchSize);
                var achievements = await _client.AchievementTranslations(lang, offset, BatchSize);

                if (achievements.Count == 0)
                {
                    break;
                }

                await InsertMissingTranslationKeys(achievements
                    .Select(e => TranslationKey.Achievement.Name(e.Id))
                    .ToHashSet()
                );

                await InsertMissingTranslationKeys(achievements
                    .Select(e => TranslationKey.Achievement.Description(e.Id))
                    .ToHashSet()
                );
                
                await _context.SaveChangesAsync();

                await UpdateTranslations(
                    lang,
                    achievements,
                    e => TranslationKey.Achievement.Name(e.Id),
                    e => e.Name
                );

                await UpdateTranslations(
                    lang,
                    achievements,
                    e => TranslationKey.Achievement.Description(e.Id),
                    e => e.Description
                );

                await _context.SaveChangesAsync();

                if (achievements.Count < BatchSize)
                {
                    break;
                }
            }
        }

        _logger.LogInformation("Achievement translations synchronized");
    }
}