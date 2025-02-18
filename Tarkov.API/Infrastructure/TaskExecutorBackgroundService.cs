using Microsoft.EntityFrameworkCore;
using Tarkov.API.Application.Tasks;
using Tarkov.API.Database;
using Tarkov.API.Database.Entities;

namespace Tarkov.API.Infrastructure;

public class ExecutionResult
{
    public Guid TaskId { get; private set; }
    public bool Success { get; private set; } = true;
    public string? ErrorMessage { get; private set; }
    public int EntitiesUpdated { get; private set; }
    public int EntitiesCreated { get; private set; }
    public int EntitiesDeleted { get; private set; }

    public DateTime StartTime { get; private set; }
    public DateTime EndTime { get; private set; }
    public TimeSpan Duration => StartTime - EndTime;

    public ExecutionResult(Guid taskId)
    {
        TaskId = taskId;
    }

    public void SetError(string errorMessage)
    {
        var message = errorMessage.Substring(0, Math.Min(errorMessage.Length, TaskExecutionEntity.MaxErrorMessageLength));
        Success = false;
        ErrorMessage = message;
    }

    public void Start()
    {
        StartTime = DateTime.UtcNow;
    }

    public void End()
    {
        EndTime = DateTime.UtcNow;
    }

    public void AddEntitiesUpdated(DatabaseContext context)
    {
        EntitiesUpdated = context.EntitiesUpdated;
        EntitiesCreated = context.EntitiesCreated;
        EntitiesDeleted = context.EntitiesDeleted;
    }
}

public class TaskExecutorBackgroundService : BackgroundService
{
    private static readonly TimeSpan CheckInterval = TimeSpan.FromSeconds(10);

    private readonly ILogger<TaskExecutorBackgroundService> _logger;
    private readonly IServiceProvider _serviceProvider;

    public TaskExecutorBackgroundService(ILogger<TaskExecutorBackgroundService> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }


    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Starting");
        cancellationToken.Register(() => _logger.LogInformation("Stopping"));

        while (!cancellationToken.IsCancellationRequested)
        {
            var startTime = DateTime.UtcNow;
            await TryExecuteTaskTask(cancellationToken);
            var endTime = DateTime.UtcNow;

            var timeToWait = CheckInterval - (endTime - startTime);
            if (timeToWait > TimeSpan.FromMilliseconds(100))
            {
                await Task.Delay(timeToWait, cancellationToken);
            }
        }
    }

    private async Task TryExecuteTaskTask(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Checking for tasks to run");
        var taskEntity = await GetNextTask(cancellationToken);
        if (taskEntity == null)
        {
            _logger.LogInformation("No tasks to run");
            return;
        }

        _logger.LogInformation("Running task {TaskName}", taskEntity.Name);

        var result = new ExecutionResult(taskEntity.Id);
        {
            await using var scope = _serviceProvider.CreateAsyncScope();
            var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
            var task = scope.ServiceProvider.GetRequiredKeyedService<ISyncTask>(taskEntity.Name);
            result.Start();
            try
            {
                await task.Run(cancellationToken);
            }
            catch (Exception e)
            {
                result.SetError(e.Message);
            }
            finally
            {
                result.End();
            }

            if (!result.Success)
            {
                _logger.LogError("Task {TaskName} failed with error: {ErrorMessage}", taskEntity.Name, result.ErrorMessage);
            }

            result.AddEntitiesUpdated(context);
        }

        await UpdateTask(result, cancellationToken);
    }

    private async Task UpdateTask(ExecutionResult result, CancellationToken cancellationToken = default)
    {
        await using var scope = _serviceProvider.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();

        var taskEntity = await context.Tasks
            .FirstOrDefaultAsync(t => t.Id == result.TaskId, cancellationToken: cancellationToken);

        if (taskEntity == null)
        {
            throw new Exception("Task not found");
        }

        taskEntity.Executions.Add(new TaskExecutionEntity(
            taskEntity.Id,
            result.Success,
            result.EntitiesUpdated,
            result.EntitiesCreated,
            result.EntitiesDeleted,
            result.StartTime,
            result.EndTime,
            result.Duration,
            result.ErrorMessage
        ));

        taskEntity.UpdateLastRun(result.EndTime, result.Success);
        taskEntity.ScheduleNextRun();

        await context.SaveChangesAsync(cancellationToken);
    }

    private async Task<TaskEntity?> GetNextTask(CancellationToken cancellationToken = default)
    {
        await using var scope = _serviceProvider.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();

        var taskEntity = await context.Tasks
            .AsNoTracking()
            .OrderBy(t => t.NextScheduledRun)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);

        if (taskEntity == null || taskEntity.NextScheduledRun > DateTime.UtcNow)
        {
            return null;
        }

        return taskEntity;
    }
}