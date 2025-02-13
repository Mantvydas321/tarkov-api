using GraphQL.Client.Http;
using GraphQL.Client.Serializer.SystemTextJson;
using Microsoft.EntityFrameworkCore;
using Tarkov.API.Database;
using Tarkov.API.Infrastructure.Clients;
using Tarkov.API.Infrastructure.Tasks;

var builder = WebApplication.CreateBuilder(args);

// Add database context
builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add services
builder.Services.AddTransient<TarkovClient>();
builder.Services.AddTransient<AchievementsSyncTask>();
builder.Services.AddTransient<AchievementTranslationsSyncTask>();

// Add GraphQL client
builder.Services.AddSingleton<GraphQLHttpClient>(
    _ => new GraphQLHttpClient("https://api.tarkov.dev/graphql", new SystemTextJsonSerializer())
);

var app = builder.Build();

// Create and migrate database
using (var serviceScope = app.Services.CreateScope())
{
    var context = serviceScope.ServiceProvider.GetRequiredService<DatabaseContext>();
    context.Database.Migrate();
    context.SaveChanges();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Serve the Swagger JSON at /swagger/v1/swagger.json
    app.UseSwagger();

    // Serve Swagger UI at the root (http://localhost:5258/)
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = string.Empty; // Makes Swagger UI available at the root
    });
}

app.UseAuthorization();

app.MapControllers();

app.Run();