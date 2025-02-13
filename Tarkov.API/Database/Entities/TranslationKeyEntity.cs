namespace Tarkov.API.Database.Entities;

public class TranslationKeyEntity : IComparable
{
    public static readonly int MaxKeyLength = 50;

    public required string Key { get; init; } = null!;

    public List<TranslationEntity> Translations { get; set; } = new();

    public AchievementEntity? AchievementName { get; set; }
    public AchievementEntity? AchievementDescription { get; set; }

    public override string ToString() => Key;

    public int CompareTo(object? obj)
    {
        if (obj is TranslationKeyEntity other)
        {
            return String.Compare(Key, other.Key, StringComparison.Ordinal);
        }

        return -1;
    }
}

public static class TranslationKey
{
    public static class Achievement
    {
        public static string Name(string id) => $"Achievement.{id}.Name";
        public static string Description(string id) => $"Achievement.{id}.Description";
    }
}