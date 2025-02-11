using Microsoft.EntityFrameworkCore;
using tarkov_api.Database.Entities;

namespace tarkov_api.Database;

public class DatabaseContext : DbContext
{
    public DbSet<AchievementEntity> Achievements { get; set; }
    
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
    }
}