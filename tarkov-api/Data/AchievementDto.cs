namespace tarkov_api.Data;

public class AchievementDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public bool Hidden { get; set; }
    public float PlayersCompletedPercentage { get; set; }
    public string Side { get; set; }
    public string Rarity { get; set; }
}

public class AchievementsApi {
    public List<AchievementDto>? Achievements { get; set; }
}