using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tarkov.API.Database.Entities;
using Tarkov.API.Database.ValueConverters;

namespace Tarkov.API.Database.EntityTypeConfigurations;

public class TaskEntityTypeConfiguration : IEntityTypeConfiguration<TaskEntity>
{
    public void Configure(EntityTypeBuilder<TaskEntity> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.CronExpression)
            .IsRequired()
            .HasConversion(new CrontabScheduleConverter())
            .HasMaxLength(20);

        builder.Property(x => x.NextScheduledRun)
            .IsRequired();

        builder.Property(x => x.LastRunSuccessful)
            .IsRequired();

        builder.Property(x => x.CreatedDate)
            .IsRequired();

        builder.Property(x => x.ModifiedDate)
            .IsRequired();

        builder.HasMany(x => x.Executions)
            .WithOne(x => x.Task)
            .HasForeignKey(x => x.TaskId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasIndex(x => x.Name)
            .IsUnique();
    }
}