using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Tarkov.API.Database.Entities;

namespace Tarkov.API.Database.EntityTypeConfigurations;

public class TaskExecutionEntityTypeConfiguration : IEntityTypeConfiguration<TaskExecutionEntity>
{
    public void Configure(EntityTypeBuilder<TaskExecutionEntity> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd()
            .UseIdentityColumn();

        builder.Property(x => x.TaskId)
            .IsRequired();

        builder.Property(x => x.EntitiesUpdated)
            .IsRequired();

        builder.Property(x => x.EntitiesCreated)
            .IsRequired();

        builder.Property(x => x.Success)
            .IsRequired();

        builder.Property(x => x.ErrorMessage)
            .HasMaxLength(TaskExecutionEntity.MaxErrorMessageLength);

        builder.Property(x => x.Start)
            .IsRequired();

        builder.Property(x => x.End)
            .IsRequired();

        builder.Property(x => x.Duration)
            .HasConversion(new TimeSpanToTicksConverter())
            .IsRequired();

        builder.Property(x => x.CreatedDate)
            .IsRequired();

        builder.HasOne(x => x.Task)
            .WithMany(x => x.Executions)
            .HasForeignKey(x => x.TaskId);
    }
}