using Microsoft.EntityFrameworkCore;
using tarkov_api.Database.Entities;
using tarkov_api.Database.EntityConfig;

namespace tarkov_api.Database;

public class DatabaseContext : DbContext
{
    public DbSet<AchievementEntity> Achievements { get; set; }

    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AchievementEntityConfig).Assembly);
    }
}