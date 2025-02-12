using Microsoft.EntityFrameworkCore;
using Tarkov.API.Database.Entities;
using Tarkov.API.Database.EntityConfig;

namespace Tarkov.API.Database;

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