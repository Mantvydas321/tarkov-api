using Microsoft.EntityFrameworkCore;
using Tarkov.API.Database.Entities;
using Tarkov.API.Database.EntityTypeConfigurations;

namespace Tarkov.API.Database;

public class DatabaseContext : DbContext
{
    public DbSet<AchievementEntity> Achievements { get; set; }
    public DbSet<TranslationKeyEntity> TranslationKeys { get; set; }
    public DbSet<TranslationEntity> Translations { get; set; }

    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AchievementEntityTypeConfiguration).Assembly);
    }
}