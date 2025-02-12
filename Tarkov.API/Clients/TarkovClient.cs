using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using Tarkov.API.Data;

namespace Tarkov.API.Clients;

public class TarkovClient : ITarkovClient
{
    private readonly GraphQLHttpClient _client;
    private readonly ILogger<TarkovClient> _logger;

    public TarkovClient(ILogger<TarkovClient> logger)
    {
        _logger = logger;
        _client = new GraphQLHttpClient("https://api.tarkov.dev/graphql", new NewtonsoftJsonSerializer());
    }
    
    public async Task<List<AchievementDto>> GetSaveAchievements()
    {
        _logger.LogInformation("Fetching achievements from API");

        var query = new GraphQLRequest
        {
            Query = @"
                query Achievements {
                    achievements {
                        name
                        description
                        hidden
                        playersCompletedPercent
                        side
                        rarity
                    }
                }"
        };

        var response = await _client.SendQueryAsync<AchievementsApi>(query);

        _logger.LogInformation("Fetched {Count} achievements from API", response.Data.Achievements?.Count);

        if (response.Errors != null)
            throw new Exception();

        return response.Data.Achievements!;
    }
}