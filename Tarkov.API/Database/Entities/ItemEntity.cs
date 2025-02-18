using System.Diagnostics.CodeAnalysis;
using Tarkov.API.Application.Client.Queries;

namespace Tarkov.API.Database.Entities;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public class ItemEntity : IMutableEntity
{
    public const int MaxIdLength = 30;
    public const int UrlMaxLength = 256;

    public string Id { get; protected set; }
    public string ShortName { get; protected set; }
    public string NormalizedName { get; protected set; }
    public string WikiLink { get; protected set; }
    public string IconLink { get; protected set; }
    public string IconLinkFallback { get; protected set; }
    public string ImageLink { get; protected set; }
    public string ImageLinkFallback { get; protected set; }
    public string Image512pxLink { get; protected set; }
    public string Image8xLink { get; protected set; }
    public string BaseImageLink { get; protected set; }
    public string GridImageLink { get; protected set; }
    public string GridImageLinkFallback { get; protected set; }
    public string InspectImageLink { get; protected set; }
    public string Link { get; protected set; }
    public int BasePrice { get; protected set; }
    public string BsgCategoryId { get; protected set; }
    public float Height { get; protected set; }
    public float Width { get; protected set; }
    public float Weight { get; protected set; }
    public float? AccuracyModifier { get; protected set; }
    public float? RecoilModifier { get; protected set; }
    public float? ErgonomicsModifier { get; protected set; }
    public float? Velocity { get; protected set; }
    public float? Loudness { get; protected set; }
    public bool? BlocksHeadphones { get; protected set; }
    public bool? HasGrid { get; protected set; }
    public string BackgroundColor { get; protected set; }
    public DateTime Updated { get; protected set; }

    public DateTime? CreatedDate { get; set; }
    public DateTime? ModifiedDate { get; set; }

    public List<ItemTypeEntity> Types { get; protected set; } = new();

    public List<ItemTranslationEntity> Translations { get; set; } = new();

    // EF Core constructor
#pragma warning disable CS8618, CS9264
    protected ItemEntity()
    {
    }
#pragma warning restore CS8618, CS9264

    public ItemEntity(ItemClientData data)
    {
        Id = data.Id;
        ShortName = data.ShortName;
        NormalizedName = data.NormalizedName;
        WikiLink = data.WikiLink;
        IconLink = data.IconLink;
        IconLinkFallback = data.IconLinkFallback;
        ImageLink = data.ImageLink;
        ImageLinkFallback = data.ImageLinkFallback;
        Image512pxLink = data.Image512pxLink;
        Image8xLink = data.Image8xLink;
        BaseImageLink = data.BaseImageLink;
        GridImageLink = data.GridImageLink;
        GridImageLinkFallback = data.GridImageLinkFallback;
        InspectImageLink = data.InspectImageLink;
        Link = data.Link;
        BasePrice = data.BasePrice;
        BsgCategoryId = data.BsgCategoryId;
        Height = data.Height;
        Width = data.Width;
        Weight = data.Weight;
        AccuracyModifier = data.AccuracyModifier;
        RecoilModifier = data.RecoilModifier;
        ErgonomicsModifier = data.ErgonomicsModifier;
        Velocity = data.Velocity;
        Loudness = data.Loudness;
        BlocksHeadphones = data.BlocksHeadphones;
        HasGrid = data.HasGrid;
        BackgroundColor = data.BackgroundColor;
        Updated = data.Updated;
    }

    public void Update(ItemClientData data)
    {
        if (ShortName != data.ShortName)
            ShortName = data.ShortName;

        if (NormalizedName != data.NormalizedName)
            NormalizedName = data.NormalizedName;

        if (WikiLink != data.WikiLink)
            WikiLink = data.WikiLink;

        if (IconLink != data.IconLink)
            IconLink = data.IconLink;

        if (IconLinkFallback != data.IconLinkFallback)
            IconLinkFallback = data.IconLinkFallback;

        if (ImageLink != data.ImageLink)
            ImageLink = data.ImageLink;

        if (ImageLinkFallback != data.ImageLinkFallback)
            ImageLinkFallback = data.ImageLinkFallback;

        if (Image512pxLink != data.Image512pxLink)
            Image512pxLink = data.Image512pxLink;

        if (Image8xLink != data.Image8xLink)
            Image8xLink = data.Image8xLink;

        if (BaseImageLink != data.BaseImageLink)
            BaseImageLink = data.BaseImageLink;

        if (GridImageLink != data.GridImageLink)
            GridImageLink = data.GridImageLink;

        if (GridImageLinkFallback != data.GridImageLinkFallback)
            GridImageLinkFallback = data.GridImageLinkFallback;

        if (InspectImageLink != data.InspectImageLink)
            InspectImageLink = data.InspectImageLink;

        if (Link != data.Link)
            Link = data.Link;

        if (BasePrice != data.BasePrice)
            BasePrice = data.BasePrice;

        if (BsgCategoryId != data.BsgCategoryId)
            BsgCategoryId = data.BsgCategoryId;

        if (Math.Abs(Height - data.Height) > 0.0001)
            Height = data.Height;

        if (Math.Abs(Width - data.Width) > 0.0001)
            Width = data.Width;

        if (Math.Abs(Weight - data.Weight) > 0.0001)
            Weight = data.Weight;

        if ((AccuracyModifier != null && data.AccuracyModifier != null && Math.Abs(AccuracyModifier.Value - data.AccuracyModifier.Value) > 0.0001) ||
            (AccuracyModifier == null && data.AccuracyModifier != null) ||
            (AccuracyModifier != null && data.AccuracyModifier == null))
            AccuracyModifier = data.AccuracyModifier;

        if ((RecoilModifier != null && data.RecoilModifier != null && Math.Abs(RecoilModifier.Value - data.RecoilModifier.Value) > 0.0001) ||
            (RecoilModifier == null && data.RecoilModifier != null) ||
            (RecoilModifier != null && data.RecoilModifier == null))
            RecoilModifier = data.RecoilModifier;

        if ((ErgonomicsModifier != null && data.ErgonomicsModifier != null && Math.Abs(ErgonomicsModifier.Value - data.ErgonomicsModifier.Value) > 0.0001) ||
            (ErgonomicsModifier == null && data.ErgonomicsModifier != null) ||
            (ErgonomicsModifier != null && data.ErgonomicsModifier == null))
            ErgonomicsModifier = data.ErgonomicsModifier;

        if ((Velocity != null && data.Velocity != null && Math.Abs(Velocity.Value - data.Velocity.Value) > 0.0001) ||
            (Velocity == null && data.Velocity != null) ||
            (Velocity != null && data.Velocity == null))
            Velocity = data.Velocity;

        if ((Loudness != null && data.Loudness != null && Math.Abs(Loudness.Value - data.Loudness.Value) > 0.0001) ||
            (Loudness == null && data.Loudness != null) ||
            (Loudness != null && data.Loudness == null))
            Loudness = data.Loudness;

        if (BlocksHeadphones != data.BlocksHeadphones)
            BlocksHeadphones = data.BlocksHeadphones;

        if (HasGrid != data.HasGrid)
            HasGrid = data.HasGrid;

        if (BackgroundColor != data.BackgroundColor)
            BackgroundColor = data.BackgroundColor;

        if (Updated != data.Updated)
            Updated = data.Updated;
    }
}