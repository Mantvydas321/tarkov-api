using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tarkov.API.Database.Entities;

namespace Tarkov.API.Database.EntityTypeConfigurations;

public class AchievementTranslationEntityTypeConfiguration : IEntityTypeConfiguration<AchievementTranslationEntity>
{
    public void Configure(EntityTypeBuilder<AchievementTranslationEntity> builder)
    {
        builder.HasKey(x => new { x.AchievementId, x.Language, x.Field });

        builder
            .Property(x => x.AchievementId)
            .HasMaxLength(AchievementEntity.MaxIdLength)
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
            .HasMaxLength(500)
            .IsRequired();

        builder
            .Property(x => x.CreatedDate)
            .IsRequired();

        builder
            .Property(x => x.ModifiedDate)
            .IsRequired();

        builder
            .HasOne(x => x.Achievement)
            .WithMany(x => x.Translations)
            .HasForeignKey(x => x.AchievementId);
    }
}