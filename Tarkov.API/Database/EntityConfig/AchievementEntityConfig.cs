using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tarkov.API.Database.Entities;

namespace Tarkov.API.Database.EntityConfig;

public class AchievementEntityConfig : IEntityTypeConfiguration<AchievementEntity>
{
    public void Configure(EntityTypeBuilder<AchievementEntity> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.Name)
            .IsRequired();

        builder.Property(x => x.Description)
            .IsRequired();

        builder.Property(x => x.Hidden)
            .IsRequired();

        builder.Property(x => x.PlayersCompletedPercentage)
            .IsRequired();

        builder.Property(x => x.Side)
            .IsRequired();

        builder.Property(x => x.Rarity)
            .IsRequired();
    }
}