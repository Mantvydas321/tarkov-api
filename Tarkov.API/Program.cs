using System.Text.Json.Serialization;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.SystemTextJson;
using Microsoft.EntityFrameworkCore;
using Tarkov.API.Application.Tasks;
using Tarkov.API.Database;
using Tarkov.API.Infrastructure;
using Tarkov.API.Infrastructure.Clients;

var builder = WebApplication.CreateBuilder(args);

// Add database context
builder.Services.AddDbContext<DatabaseContext>(options => { options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")); });
builder.Services.AddTransient<DatabaseContextSeed>();

// Add Controllers
builder.Services.AddControllers(options =>
{
    options.Filters.Add(typeof(HttpGlobalExceptionFilter));
    options.EnableEndpointRouting = true;
});
builder.Services.ConfigureHttpJsonOptions(options => { options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()); });
builder.Services.Configure<Microsoft.AspNetCore.Mvc.JsonOptions>(options => { options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()); });


// Add MediatR
builder.Services.AddMediatR(cfg => { cfg.RegisterServicesFromAssemblyContaining<Program>(); });

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add services
builder.Services.AddKeyedTransient<ISyncTask, AchievementsSyncTask>(nameof(AchievementsSyncTask));
builder.Services.AddKeyedTransient<ISyncTask, AchievementTranslationsSyncTask>(nameof(AchievementTranslationsSyncTask));

// Add GraphQL client
builder.Services.AddTransient<TarkovClient>();
builder.Services.AddSingleton<GraphQLHttpClient>(
    _ => new GraphQLHttpClient("https://api.tarkov.dev/graphql", new SystemTextJsonSerializer())
);

// Add background service
builder.Services.AddHostedService<TaskExecutorBackgroundService>();

var app = builder.Build();

// Create, migrate adn seed database
await using (var serviceScope = app.Services.CreateAsyncScope())
{
    var seed = serviceScope.ServiceProvider.GetRequiredService<DatabaseContextSeed>();
    await seed.SeedAsync();
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