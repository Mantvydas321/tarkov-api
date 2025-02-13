using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tarkov.API.Database.Entities;

namespace Tarkov.API.Database.EntityTypeConfigurations;

public class TranslationKeyEntityTypeConfiguration : IEntityTypeConfiguration<TranslationKeyEntity>
{
    public void Configure(EntityTypeBuilder<TranslationKeyEntity> builder)
    {
        builder.HasKey(x => x.Key);

        builder.Property(x => x.Key)
            .HasMaxLength(TranslationKeyEntity.MaxKeyLength)
            .ValueGeneratedNever();

        builder.HasMany(x => x.Translations)
            .WithOne(x => x.KeyEntity)
            .HasForeignKey(x => x.Key)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(x => x.AchievementName)
            .WithOne(x => x.NameTranslationKeyEntity)
            .HasForeignKey<AchievementEntity>(x => x.NameTranslationKey)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.AchievementDescription)
            .WithOne(x => x.DescriptionTranslationKeyEntity)
            .HasForeignKey<AchievementEntity>(x => x.DescriptionTranslationKey)
            .OnDelete(DeleteBehavior.Restrict);
    }
}