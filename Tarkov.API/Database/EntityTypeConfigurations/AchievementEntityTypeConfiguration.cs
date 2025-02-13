using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tarkov.API.Database.Entities;

namespace Tarkov.API.Database.EntityTypeConfigurations;

public class AchievementEntityTypeConfiguration : IEntityTypeConfiguration<AchievementEntity>
{
    public void Configure(EntityTypeBuilder<AchievementEntity> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasMaxLength(30)
            .ValueGeneratedNever();

        builder.Property(x => x.NameTranslationKey)
            .HasMaxLength(TranslationKeyEntity.MaxKeyLength)
            .IsRequired();

        builder.Property(x => x.DescriptionTranslationKey)
            .HasMaxLength(TranslationKeyEntity.MaxKeyLength)
            .IsRequired();

        builder.Property(x => x.Hidden)
            .IsRequired();

        builder.Property(x => x.Side)
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(x => x.Rarity)
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(x => x.PlayersCompletedPercentage)
            .IsRequired();

        builder.Property(x => x.AdjustedPlayersCompletedPercentage)
            .IsRequired();

        builder
            .HasOne(x => x.NameTranslationKeyEntity)
            .WithOne(x => x.AchievementName)
            .HasForeignKey<AchievementEntity>(x => x.NameTranslationKey);

        builder
            .HasOne(x => x.DescriptionTranslationKeyEntity)
            .WithOne(x => x.AchievementDescription)
            .HasForeignKey<AchievementEntity>(x => x.DescriptionTranslationKey);
    }
}