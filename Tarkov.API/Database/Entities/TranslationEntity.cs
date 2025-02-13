using Tarkov.API.Database.Enumeration;

namespace Tarkov.API.Database.Entities;

public class TranslationEntity
{
    public required string Key { get; set; }
    public TranslationKeyEntity? KeyEntity { get; set; }
    
    public required LanguageCode Language { get; set; }
    public required string Value { get; set; }
}