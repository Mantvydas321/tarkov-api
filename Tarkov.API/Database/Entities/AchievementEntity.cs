namespace Tarkov.API.Database.Entities;

public class AchievementEntity
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required bool Hidden { get; set; }
    public required float PlayersCompletedPercentage { get; set; }
    public required string Side { get; set; }
    public required string Rarity { get; set; }
}