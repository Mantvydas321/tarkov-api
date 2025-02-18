using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tarkov.API.Database.Entities;

namespace Tarkov.API.Database.EntityTypeConfigurations;

public class AchievementEntityTypeConfiguration : IEntityTypeConfiguration<AchievementEntity>
{
    public void Configure(EntityTypeBuilder<AchievementEntity> builder)
    {
        builder.HasKey(x => x.Id);

        builder
            .Property(x => x.Id)
            .HasMaxLength(AchievementEntity.MaxIdLength)
            .ValueGeneratedNever();

        builder
            .Property(x => x.Hidden)
            .IsRequired();

        builder
            .Property(x => x.Side)
            .HasMaxLength(10)
            .IsRequired();

        builder
            .Property(x => x.Rarity)
            .HasMaxLength(10)
            .IsRequired();

        builder
            .Property(x => x.PlayersCompletedPercentage)
            .IsRequired();

        builder
            .Property(x => x.AdjustedPlayersCompletedPercentage)
            .IsRequired();

        builder
            .Property(x => x.CreatedDate)
            .IsRequired();

        builder
            .Property(x => x.ModifiedDate)
            .IsRequired();

        builder
            .HasMany(x => x.Translations)
            .WithOne(x => x.Achievement)
            .HasForeignKey(x => x.AchievementId);
    }
}