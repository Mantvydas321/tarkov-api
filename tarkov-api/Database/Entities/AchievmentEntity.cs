namespace tarkov_api.Database.Entities;

public class AchievmentEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public bool Hidden { get; set; }
    public float PlayersCompletedPercentage { get; set; }
    public string Side { get; set; }
    public string Rarity { get; set; }
}