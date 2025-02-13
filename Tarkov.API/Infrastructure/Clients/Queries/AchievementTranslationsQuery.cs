using GraphQL;
using GraphQL.Client.Http;
using Tarkov.API.Database.Enumeration;

namespace Tarkov.API.Infrastructure.Clients.Queries;

public static class AchievementTranslationsQuery
{
    private static readonly string Query = @"
        query Achievements($lang: LanguageCode, $limit: Int, $offset: Int) {
            achievements(lang: $lang, limit: $limit, offset: $offset) {
                id
                name
                description
            }
        }
    ";

    public static async Task<List<AchievementTranslation>> QueryAchievementTranslationsAsync(this GraphQLHttpClient client, LanguageCode lang, int limit, int offset)
    {
        var query = new GraphQLRequest
        {
            Query = Query,
            OperationName = "Achievements",
            Variables = new
            {
                lang = lang.ToString().ToLower(),
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

    public class AchievementTranslation
    {
        public required string Id { get; set; }
        public required string Name  { get; set; }
        public required string Description { get; set; }
    }

    private class Response
    {
        public required List<AchievementTranslation> Achievements { get; set; }
    }
}