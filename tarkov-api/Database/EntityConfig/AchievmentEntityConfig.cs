using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using tarkov_api.Database.Entities;

namespace tarkov_api.Database.EntityConfig;

public class AchievmentEntityConfig : IEntityTypeConfiguration<AchievmentEntity>
{
    public void Configure(EntityTypeBuilder<AchievmentEntity> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Id).ValueGeneratedNever();
        
        builder.Property(x => x.Name).IsRequired();
        
        builder.Property(x => x.Description).IsRequired();
        
        builder.Property(x )
    }
}