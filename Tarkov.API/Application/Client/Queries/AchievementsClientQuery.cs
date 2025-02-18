using GraphQL;
using GraphQL.Client.Http;
using MediatR;

namespace Tarkov.API.Application.Client.Queries;

public class AchievementClientRequest : IRequest<AchievementClientResponse>
{
    public required int Limit { get; set; }
    public required int Offset { get; set; }
}

public class AchievementClientData
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

public class AchievementClientResponse
{
    public required List<AchievementClientData> Achievements { get; set; }
}

public class AchievementsClientQuery : IRequestHandler<AchievementClientRequest, AchievementClientResponse>
{
    private static readonly string QueryString = @"
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

    private readonly GraphQLHttpClient _client;

    public AchievementsClientQuery(GraphQLHttpClient client)
    {
        _client = client;
    }

    public async Task<AchievementClientResponse> Handle(AchievementClientRequest clientRequest, CancellationToken cancellationToken)
    {
        var query = new GraphQLRequest
        {
            Query = QueryString,
            OperationName = "Achievements",
            Variables = new
            {
                limit = clientRequest.Limit,
                offset = clientRequest.Offset
            }
        };

        var response = await _client.SendQueryAsync<AchievementClientResponse>(query, cancellationToken);
        if (response.Errors != null)
        {
            throw new Exception(string.Join(", ", response.Errors.Select(e => e.Message)));
        }

        return response.Data;
    }
}