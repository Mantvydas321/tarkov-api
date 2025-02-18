using System.Diagnostics.CodeAnalysis;
using GraphQL;
using GraphQL.Client.Http;
using MediatR;

namespace Tarkov.API.Application.Client.Queries;

public class ItemsClientRequest : IRequest<ItemsClientResponse>
{
    public required int Limit { get; set; }
    public required int Offset { get; set; }
}

[SuppressMessage("ReSharper", "InconsistentNaming")]
public class ItemClientData
{
    public required string Id { get; set; }
    public required string ShortName { get; set; }
    public required string NormalizedName { get; set; }
    public required string WikiLink { get; set; }
    public required string IconLink { get; set; }
    public required string IconLinkFallback { get; set; }
    public required string ImageLink { get; set; }
    public required string ImageLinkFallback { get; set; }
    public required string Image512pxLink { get; set; }
    public required string Image8xLink { get; set; }
    public required string BaseImageLink { get; set; }
    public required string GridImageLink { get; set; }
    public required string GridImageLinkFallback { get; set; }
    public required string InspectImageLink { get; set; }
    public required string Link { get; set; }
    public required int BasePrice { get; set; }
    public required string BsgCategoryId { get; set; }
    public required float Height { get; set; }
    public required float Width { get; set; }
    public required float Weight { get; set; }
    public float? AccuracyModifier { get; set; }
    public float? RecoilModifier { get; set; }
    public float? ErgonomicsModifier { get; set; }
    public float? Velocity { get; set; }
    public float? Loudness { get; set; }
    public bool? BlocksHeadphones { get; set; }
    public bool? HasGrid { get; set; }
    public required string[] Types { get; set; }
    public required string[] ConflictingSlotIds { get; set; }
    public required string BackgroundColor { get; set; }
    public required DateTime Updated { get; set; }
}

public class ItemsClientResponse
{
    public required List<ItemClientData> Items { get; set; }
}

public class ItemsClientQuery : IRequestHandler<ItemsClientRequest, ItemsClientResponse>
{
    private static readonly string QueryString = @"
        query Items($limit: Int, $offset: Int) {
            items(limit: $limit, offset: $offset) {
                id
                shortName
                normalizedName
                wikiLink
                iconLink
                iconLinkFallback
                imageLink
                imageLinkFallback
                image512pxLink
                image8xLink
                baseImageLink
                gridImageLink
                gridImageLinkFallback
                inspectImageLink
                link
                basePrice
                bsgCategoryId
                height
                width
                weight
                accuracyModifier
                recoilModifier
                ergonomicsModifier
                velocity
                loudness
                blocksHeadphones
                hasGrid
                types
                conflictingSlotIds
                backgroundColor
                updated
            }
        }
    ";

    private readonly GraphQLHttpClient _client;

    public ItemsClientQuery(GraphQLHttpClient client)
    {
        _client = client;
    }

    public async Task<ItemsClientResponse> Handle(ItemsClientRequest clientRequest, CancellationToken cancellationToken)
    {
        var query = new GraphQLRequest
        {
            Query = QueryString,
            OperationName = "Items",
            Variables = new
            {
                limit = clientRequest.Limit,
                offset = clientRequest.Offset
            }
        };

        var response = await _client.SendQueryAsync<ItemsClientResponse>(query, cancellationToken);
        if (response.Errors != null)
        {
            throw new Exception(string.Join(", ", response.Errors.Select(e => e.Message)));
        }

        return response.Data;
    }
}