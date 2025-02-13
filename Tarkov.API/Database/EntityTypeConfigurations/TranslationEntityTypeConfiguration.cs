using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tarkov.API.Database.Entities;

namespace Tarkov.API.Database.EntityTypeConfigurations;

public class TranslationEntityTypeConfiguration : IEntityTypeConfiguration<TranslationEntity>
{
    public void Configure(EntityTypeBuilder<TranslationEntity> builder)
    {
        builder.HasKey(x => new { x.Key, x.Language });
        
        builder.Property(x => x.Key)
            .HasMaxLength(TranslationKeyEntity.MaxKeyLength)
            .IsRequired();

        builder.Property(x => x.Language)
            .HasMaxLength(2)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(x => x.Value)
            .HasMaxLength(500)
            .IsRequired();

        builder.HasOne(x => x.KeyEntity)
            .WithMany(x => x.Translations)
            .HasForeignKey(x => x.Key);
    }
}