using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tarkov.API.Database.Entities;

namespace Tarkov.API.Database.EntityTypeConfigurations;

public class ItemTypeEntityTypeConfiguration : IEntityTypeConfiguration<ItemTypeEntity>
{
    public void Configure(EntityTypeBuilder<ItemTypeEntity> builder)
    {
        builder.HasKey(x => x.Name);

        builder
            .Property(x => x.Name)
            .ValueGeneratedNever()
            .HasMaxLength(ItemTypeEntity.MaxNameLength)
            .IsRequired();

        builder
            .Property(x => x.CreatedDate)
            .IsRequired();

        builder
            .HasMany(x => x.Items)
            .WithMany(x => x.Types);
    }
}