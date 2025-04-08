namespace Tarkov.API.Application.Models;

public class ItemData
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
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
    //public required string[] ConflictingSlotIds { get; set; }
    public required string BackgroundColor { get; set; }
    public required DateTime Updated { get; set; }
}