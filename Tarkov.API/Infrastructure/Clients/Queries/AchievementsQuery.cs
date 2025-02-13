using GraphQL;
using GraphQL.Client.Http;

namespace Tarkov.API.Infrastructure.Clients.Queries;

public static class AchievementsQuery
{
    private static readonly string Query = @"
        query Achievements($limit: Int, $offset: Int) {
            achievements(limit: $limit, offset: $offset) {
                id
                hidden
                side
                rarity
                playersCompletedPercent
                adjustedPlayersCompletedPercent
                normalizedRarity
                normalizedSide
            }
        }
    ";

    public static async Task<List<Achievement>> QueryAchievementsAsync(this GraphQLHttpClient client, int limit, int offset)
    {
        var query = new GraphQLRequest
        {
            Query = Query,
            OperationName = "Achievements",
            Variables = new
            {
                limit,
                offset
            }
        };

        var response = await client.SendQueryAsync<Response>(query);
        if (response.Errors != null)
        {
            throw new Exception(string.Join(", ", response.Errors.Select(e => e.Message)));
        }

        return response.Data.Achievements;
    }

    public class Achievement
    {
        public required string Id { get; set; }
        public required bool Hidden { get; set; }
        public required string Side { get; set; }
        public required string Rarity { get; set; }
        public required float PlayersCompletedPercent { get; set; }
        public required float AdjustedPlayersCompletedPercent { get; set; }
        public required string NormalizedRarity { get; set; }
        public required string NormalizedSide { get; set; }
    }

    private class Response
    {
        public required List<Achievement> Achievements { get; set; }
    }
}