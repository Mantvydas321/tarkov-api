using GraphQL;
using GraphQL.Client.Http;
using MediatR;
using Tarkov.API.Database.Enumeration;

namespace Tarkov.API.Application.Client.Queries;

public class AchievementTranslationsClientRequest : IRequest<AchievementTranslationsClientResponse>
{
    public required LanguageCode Lang { get; set; }
    public required int Limit { get; set; }
    public required int Offset { get; set; }
}

public class AchievementTranslationsClientData
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
}

public class AchievementTranslationsClientResponse
{
    public required List<AchievementTranslationsClientData> Achievements { get; set; }
}

public class AchievementTranslationsClientQuery : IRequestHandler<AchievementTranslationsClientRequest, AchievementTranslationsClientResponse>
{
    private static readonly string QueryString = @"
        query Achievements($lang: LanguageCode, $limit: Int, $offset: Int) {
            achievements(lang: $lang, limit: $limit, offset: $offset) {
                id
                name
                description
            }
        }
    ";

    private readonly GraphQLHttpClient _client;

    public AchievementTranslationsClientQuery(GraphQLHttpClient client)
    {
        _client = client;
    }

    public async Task<AchievementTranslationsClientResponse> Handle(AchievementTranslationsClientRequest clientRequest, CancellationToken cancellationToken)
    {
        var query = new GraphQLRequest
        {
            Query = QueryString,
            OperationName = "Achievements",
            Variables = new
            {
                limit = clientRequest.Limit,
                offset = clientRequest.Offset,
                lang = clientRequest.Lang.ToString().ToLower()
            }
        };

        var response = await _client.SendQueryAsync<AchievementTranslationsClientResponse>(query, cancellationToken);
        if (response.Errors != null)
        {
            throw new Exception(string.Join(", ", response.Errors.Select(e => e.Message)));
        }

        return response.Data;
    }
}