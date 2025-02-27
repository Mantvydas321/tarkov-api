namespace Tarkov.API.Database.Entities;

public class TaskExecutionEntity : IImmutableEntity
{
    public const int MaxErrorMessageLength = 4096;

    public int? Id { get; protected set; }

    public Guid TaskId { get; protected set; }
    public TaskEntity? Task { get; protected set; }

    public bool Success { get; protected set; }
    public string? ErrorMessage { get; protected set; }

    public int EntitiesUpdated { get; protected set; }
    public int EntitiesCreated { get; protected set; }
    public int EntitiesDeleted { get; protected set; }

    public DateTime Start { get; protected set; }
    public DateTime End { get; protected set; }
    public TimeSpan Duration { get; protected set; }

    public DateTime? CreatedDate { get; set; }

    // EF Core constructor
#pragma warning disable CS8618, CS9264
    protected TaskExecutionEntity()
    {
    }
#pragma warning restore CS8618, CS9264

    public TaskExecutionEntity(
        Guid taskId,
        bool success,
        int entitiesUpdated,
        int entitiesCreated,
        int entitiesDeleted,
        DateTime start,
        DateTime end,
        TimeSpan duration,
        string? errorMessage
    )
    {
        TaskId = taskId;
        Success = success;
        EntitiesUpdated = entitiesUpdated;
        EntitiesCreated = entitiesCreated;
        EntitiesDeleted = entitiesDeleted;
        Start = start;
        End = end;
        Duration = duration;
        ErrorMessage = errorMessage;
    }
}