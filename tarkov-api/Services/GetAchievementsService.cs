using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using Microsoft.EntityFrameworkCore;
using tarkov_api.Data;
using tarkov_api.Database;
using tarkov_api.Database.Entities;

namespace tarkov_api.Services;

public class GetAchievementsService : IGetAchievementsService
{
    private readonly GraphQLHttpClient _client;
    private readonly DatabaseContext _context;

    public GetAchievementsService(DatabaseContext context)
    {
        _context = context;
        _client = new GraphQLHttpClient("https://api.tarkov.dev/graphql", new NewtonsoftJsonSerializer());
    }

    public async Task SaveAchievementsToDatabase()
    {
        var achievements = await GetAchievements();

        var existingAchievements = await _context.Achievements.ToListAsync();
        foreach (var achievement in achievements)
        {
            foreach(var existingAchievement in existingAchievements)
            {
                if (achievement.Name != existingAchievement.Name)
                { 
                    var entity = new AchievementEntity
                    {
                        Id = Guid.NewGuid(),
                        Name = achievement.Name,
                        Description = achievement.Description,
                        Hidden = achievement.Hidden,
                        PlayersCompletedPercentage = achievement.PlayersCompletedPercentage,
                        Side = achievement.Side,
                        Rarity = achievement.Rarity
                    };

                    _context.Achievements.Add(entity);
                }
            }
        }

        await _context.SaveChangesAsync();
        
    }
    
    private async Task<List<AchievementDto>> GetAchievements()
    {
        var query = new GraphQLRequest
        {
            Query = @"
                query Achievements {
                    achievements {
                        name
                        description
                        hidden
                        playersCompletedPercent
                        side
                        rarity
                    }
                }"
        };

        var response = await _client.SendQueryAsync<dynamic>(query);

        var achievements = new List<AchievementDto>();
        
        foreach (var achievement in response.Data.achievements)
        {
            achievements.Add(new AchievementDto
            {
                Name = achievement.name,
                Description = achievement.description,
                Hidden = achievement.hidden,
                PlayersCompletedPercentage = achievement.playersCompletedPercent,
                Side = achievement.side,
                Rarity = achievement.rarity
            });
        }
        
        return achievements;
    }
}
