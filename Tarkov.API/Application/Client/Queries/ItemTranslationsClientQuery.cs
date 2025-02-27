using GraphQL;
using GraphQL.Client.Http;
using MediatR;
using Tarkov.API.Database.Enumeration;

namespace Tarkov.API.Application.Client.Queries;

public class ItemTranslationsClientRequest : IRequest<ItemTranslationsClientResponse>
{
    public required LanguageCode Lang { get; set; }
    public required int Limit { get; set; }
    public required int Offset { get; set; }
}

public class ItemTranslationClientData
{
    public required string Id { get; set; }
    public required string? Name { get; set; }
    public required string? Description { get; set; }
}

public class ItemTranslationsClientResponse
{
    public required List<ItemTranslationClientData> Items { get; set; }
}

public class ItemTranslationsClientQuery : IRequestHandler<ItemTranslationsClientRequest, ItemTranslationsClientResponse>
{
    private static readonly string QueryString = @"
        query Items($lang: LanguageCode, $limit: Int, $offset: Int) {
            items(lang: $lang, limit: $limit, offset: $offset) {
                id
                name
                description
            }
        }
    ";

    private readonly GraphQLHttpClient _client;

    public ItemTranslationsClientQuery(GraphQLHttpClient client)
    {
        _client = client;
    }

    public async Task<ItemTranslationsClientResponse> Handle(ItemTranslationsClientRequest clientRequest, CancellationToken cancellationToken)
    {
        var query = new GraphQLRequest
        {
            Query = QueryString,
            OperationName = "Items",
            Variables = new
            {
                limit = clientRequest.Limit,
                offset = clientRequest.Offset,
                lang = clientRequest.Lang.ToString().ToLower()
            }
        };

        var response = await _client.SendQueryAsync<ItemTranslationsClientResponse>(query, cancellationToken);
        if (response.Errors != null)
        {
            throw new Exception(string.Join(", ", response.Errors.Select(e => e.Message)));
        }

        return response.Data;
    }
}