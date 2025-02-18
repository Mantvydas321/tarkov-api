namespace Tarkov.API.Application.Models;

public class AchievementData
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required bool Hidden { get; set; }
    public required string Side { get; set; }
    public required string Rarity { get; set; }
    public required float PlayersCompletedPercentage { get; set; }
    public required float AdjustedPlayersCompletedPercentage { get; set; }
}