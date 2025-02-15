using Microsoft.EntityFrameworkCore;
using Tarkov.API.Database.Entities;
using Tarkov.API.Database.EntityTypeConfigurations;

namespace Tarkov.API.Database;

public class DatabaseContext : DbContext
{
    public DbSet<AchievementEntity> Achievements { get; set; }
    public DbSet<AchievementTranslationEntity> AchievementTranslations { get; set; }
    public DbSet<TaskEntity> Tasks { get; set; }
    public DbSet<TaskExecutionEntity> TaskExecutions { get; set; }

    public int EntitiesUpdated { get; private set; } = 0;
    public int EntitiesCreated { get; private set; } = 0;
    public int EntitiesDeleted { get; private set; } = 0;

    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
    {
        ChangeTracker.DetectChanges();

        foreach (var entry in ChangeTracker.Entries<IImmutableEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedDate = DateTime.UtcNow;
            }
        }

        foreach (var entry in ChangeTracker.Entries<IMutableEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedDate = DateTime.UtcNow;
                entry.Entity.ModifiedDate = DateTime.UtcNow;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.ModifiedDate = DateTime.UtcNow;
            }
        }

        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.State == EntityState.Modified)
            {
                EntitiesUpdated++;
            }
            else if (entry.State == EntityState.Added)
            {
                EntitiesCreated++;
            }
            else if (entry.State == EntityState.Deleted)
            {
                EntitiesDeleted++;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AchievementEntityTypeConfiguration).Assembly);
    }
}