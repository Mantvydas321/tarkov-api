namespace Tarkov.API.Database.Entities;

public class TranslationKeyEntity : IImmutableEntity, IComparable
{
    public static readonly int MaxKeyLength = 50;

    public string Key { get; protected set; }

    public DateTime? CreatedDate { get; set; }

    public List<TranslationEntity> Translations { get; protected set; } = new();

    public AchievementEntity? AchievementName { get; protected set; }
    public AchievementEntity? AchievementDescription { get; protected set; }

    // EF Core constructor
#pragma warning disable CS8618, CS9264
    protected TranslationKeyEntity()
    {
    }
#pragma warning restore CS8618, CS9264

    public TranslationKeyEntity(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentException("Key cannot be null or empty", nameof(key));
        }

        if (key.Length > MaxKeyLength)
        {
            throw new ArgumentException($"Key cannot be longer than {MaxKeyLength} characters", nameof(key));
        }

        Key = key;
    }


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