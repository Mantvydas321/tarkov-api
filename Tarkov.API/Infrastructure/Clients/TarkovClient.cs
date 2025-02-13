using GraphQL.Client.Http;
using Tarkov.API.Database.Enumeration;
using Tarkov.API.Infrastructure.Clients.Queries;

namespace Tarkov.API.Infrastructure.Clients;

public class TarkovClient
{
    private readonly GraphQLHttpClient _client;
    private readonly ILogger<TarkovClient> _logger;

    public TarkovClient(GraphQLHttpClient client, ILogger<TarkovClient> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task<List<AchievementsQuery.Achievement>> Achievements(int offset, int limit)
    {
        return await _client.QueryAchievementsAsync(limit, offset);
    }

    public async Task<List<AchievementTranslationsQuery.AchievementTranslation>> AchievementTranslations(LanguageCode lang, int offset, int limit)
    {
        return await _client.QueryAchievementTranslationsAsync(lang, limit, offset);
    }
}