using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tarkov.API.Database.Entities;

namespace Tarkov.API.Database.EntityTypeConfigurations;

public class ItemEntityTypeConfiguration : IEntityTypeConfiguration<ItemEntity>
{
    public void Configure(EntityTypeBuilder<ItemEntity> builder)
    {
        builder.HasKey(x => x.Id);

        builder
            .Property(x => x.Id)
            .HasMaxLength(ItemEntity.MaxIdLength)
            .ValueGeneratedNever();

        builder
            .Property(x => x.ShortName)
            .HasMaxLength(50)
            .IsRequired();

        builder
            .Property(x => x.NormalizedName)
            .HasMaxLength(100)
            .IsRequired();

        builder
            .Property(x => x.WikiLink)
            .HasMaxLength(ItemEntity.UrlMaxLength)
            .IsRequired();

        builder
            .Property(x => x.IconLink)
            .HasMaxLength(ItemEntity.UrlMaxLength)
            .IsRequired();

        builder
            .Property(x => x.IconLinkFallback)
            .HasMaxLength(ItemEntity.UrlMaxLength)
            .IsRequired();

        builder
            .Property(x => x.ImageLink)
            .HasMaxLength(ItemEntity.UrlMaxLength)
            .IsRequired();

        builder
            .Property(x => x.ImageLinkFallback)
            .HasMaxLength(ItemEntity.UrlMaxLength)
            .IsRequired();

        builder
            .Property(x => x.Image512pxLink)
            .HasMaxLength(ItemEntity.UrlMaxLength)
            .IsRequired();

        builder
            .Property(x => x.Image8xLink)
            .HasMaxLength(ItemEntity.UrlMaxLength)
            .IsRequired();

        builder
            .Property(x => x.BaseImageLink)
            .HasMaxLength(ItemEntity.UrlMaxLength)
            .IsRequired();

        builder
            .Property(x => x.GridImageLink)
            .HasMaxLength(ItemEntity.UrlMaxLength)
            .IsRequired();

        builder
            .Property(x => x.GridImageLinkFallback)
            .HasMaxLength(ItemEntity.UrlMaxLength)
            .IsRequired();

        builder
            .Property(x => x.InspectImageLink)
            .HasMaxLength(ItemEntity.UrlMaxLength)
            .IsRequired();

        builder
            .Property(x => x.Link)
            .HasMaxLength(ItemEntity.UrlMaxLength)
            .IsRequired();

        builder
            .Property(x => x.BasePrice)
            .IsRequired();

        builder
            .Property(x => x.BsgCategoryId)
            .HasMaxLength(ItemEntity.MaxIdLength)
            .IsRequired();

        builder
            .Property(x => x.Height)
            .IsRequired();

        builder
            .Property(x => x.Width)
            .IsRequired();

        builder
            .Property(x => x.Weight)
            .IsRequired();

        builder
            .Property(x => x.AccuracyModifier)
            .IsRequired(false);

        builder
            .Property(x => x.RecoilModifier)
            .IsRequired(false);

        builder
            .Property(x => x.ErgonomicsModifier)
            .IsRequired(false);

        builder
            .Property(x => x.Velocity)
            .IsRequired(false);

        builder
            .Property(x => x.Loudness)
            .IsRequired(false);

        builder
            .Property(x => x.BlocksHeadphones)
            .IsRequired(false);

        builder
            .Property(x => x.BackgroundColor)
            .HasMaxLength(20)
            .IsRequired();

        builder
            .Property(x => x.Updated)
            .IsRequired();

        builder
            .Property(x => x.CreatedDate)
            .IsRequired();

        builder
            .Property(x => x.ModifiedDate)
            .IsRequired();

        builder
            .HasMany(x => x.Types)
            .WithMany(x => x.Items);
    }
}