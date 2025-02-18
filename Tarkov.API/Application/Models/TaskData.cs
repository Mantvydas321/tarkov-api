namespace Tarkov.API.Application.Models;

public class TaskData
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required string CronExpression { get; set; }
    public DateTime? LastRun { get; set; }
    public bool LastRunSuccessful { get; set; }
}