using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tarkov.API.Database.Entities;

namespace Tarkov.API.Database.EntityTypeConfigurations;

public class ItemTranslationEntityTypeConfiguration : IEntityTypeConfiguration<ItemTranslationEntity>
{
    public void Configure(EntityTypeBuilder<ItemTranslationEntity> builder)
    {
        builder.HasKey(x => new { x.ItemId, x.Language, x.Field });

        builder
            .Property(x => x.ItemId)
            .HasMaxLength(ItemEntity.MaxIdLength)
            .IsRequired();

        builder
            .Property(x => x.Language)
            .HasMaxLength(2)
            .IsRequired()
            .HasConversion<string>();

        builder
            .Property(x => x.Field)
            .IsRequired();

        builder
            .Property(x => x.Value)
            .HasMaxLength(1024)
            .IsRequired();

        builder
            .Property(x => x.CreatedDate)
            .IsRequired();

        builder
            .Property(x => x.ModifiedDate)
            .IsRequired();

        builder
            .HasOne(x => x.Item)
            .WithMany(x => x.Translations)
            .HasForeignKey(x => x.ItemId);
    }
}