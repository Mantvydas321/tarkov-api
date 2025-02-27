using NCrontab;
using Tarkov.API.Application.Models;
using Tarkov.API.Application.Tasks;

namespace Tarkov.API.Database.Entities;

public class TaskEntity : IMutableEntity
{
    public Guid Id { get; protected set; }
    public string Name { get; protected set; }
    public CrontabSchedule CronExpression { get; protected set; }
    public DateTime NextScheduledRun { get; protected set; }
    public DateTime? LastRun { get; protected set; }
    public bool LastRunSuccessful { get; protected set; }

    public DateTime? CreatedDate { get; set; }
    public DateTime? ModifiedDate { get; set; }

    public List<TaskExecutionEntity> Executions { get; set; } = new();

    // EF Core constructor
#pragma warning disable CS8618, CS9264
    protected TaskEntity()
    {
    }
#pragma warning restore CS8618, CS9264

    private TaskEntity(Guid id, string name, string cronExpression)
    {
        Id = id;
        Name = name;
        CronExpression = CrontabSchedule.Parse(cronExpression, new CrontabSchedule.ParseOptions() { IncludingSeconds = false });
        NextScheduledRun = CronExpression.GetNextOccurrence(DateTime.UtcNow);
    }

    public static TaskEntity Create<T>(Guid id, string cronExpression) where T : ISyncTask
    {
        return new TaskEntity(id, typeof(T).Name, cronExpression);
    }

    public void UpdateCronExpression(CrontabSchedule cronExpression)
    {
        if (CronExpression == cronExpression)
            return;

        CronExpression = cronExpression;
        NextScheduledRun = CronExpression.GetNextOccurrence(DateTime.UtcNow);
    }

    public void UpdateLastRun(DateTime lastRun, bool lastRunSuccessful)
    {
        LastRun = lastRun;
        LastRunSuccessful = lastRunSuccessful;
    }

    public void UpdateNextScheduledRun(DateTime nextScheduledRun)
    {
        NextScheduledRun = nextScheduledRun;
    }

    public void ScheduleNextRun()
    {
        NextScheduledRun = CronExpression.GetNextOccurrence(DateTime.UtcNow);
    }
    
    public TaskData AsData()
    {
        return new TaskData
        {
            Id = Id,
            Name = Name,
            CronExpression = CronExpression.ToString(),
            LastRun = LastRun,
            LastRunSuccessful = LastRunSuccessful
        };
    }
}