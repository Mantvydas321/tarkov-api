using Tarkov.API.Application.Tasks;
using Tarkov.API.Database;
using Tarkov.API.Database.Entities;

namespace Tarkov.API.Infrastructure;

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
            await TryExecuteNextTask(cancellationToken);
            var endTime = DateTime.UtcNow;

            var timeToWait = CheckInterval - (endTime - startTime);
            if (timeToWait > TimeSpan.FromMilliseconds(100))
            {
                await Task.Delay(timeToWait, cancellationToken);
            }
        }

        throw new NotImplementedException();
    }

    private async Task TryExecuteNextTask(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Checking for tasks to run");

        await using var scope = _serviceProvider.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();

        var taskEntity = context.Tasks
            .OrderBy(t => t.NextScheduledRun)
            .FirstOrDefault();
        if (taskEntity == null || taskEntity.NextScheduledRun > DateTime.UtcNow)
        {
            _logger.LogInformation($"Found no tasks to run");
            return;
        }

        _logger.LogInformation("Running task {TaskName}", taskEntity.Name);

        var start = DateTime.UtcNow;
        string? errorMessage = null;
        try
        {
            var task = scope.ServiceProvider.GetRequiredKeyedService<ISyncTask>(taskEntity.Name);
            await task.Run(cancellationToken);
        }
        catch (Exception e)
        {
            errorMessage = e.Message.Substring(0, Math.Min(e.Message.Length, TaskExecutionEntity.MaxErrorMessageLength));
            _logger.LogError(e, "Task {TaskName} failed with error: {ErrorMessage}", taskEntity.Name, errorMessage);
        }

        var end = DateTime.UtcNow;
        var duration = end - start;
        taskEntity.Executions.Add(new TaskExecutionEntity(
            taskEntity.Id,
            errorMessage == null,
            context.EntitiesUpdated,
            context.EntitiesCreated,
            context.EntitiesDeleted,
            start,
            end,
            duration,
            errorMessage
        ));

        taskEntity.UpdateLastRun(end, errorMessage == null);
        taskEntity.ScheduleNextRun();

        await context.SaveChangesAsync();

        _logger.LogInformation("Task {TaskName} finished in {Duration}", taskEntity.Name, duration);
    }
}