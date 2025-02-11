using Microsoft.EntityFrameworkCore;
using tarkov_api.Database;
using tarkov_api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add database context
builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();

builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add services
builder.Services.AddTransient<IAchievementsService, AchievementsService>();
builder.Services.AddTransient<ITarkovClient, TarkovClient>();


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
    app.MapOpenApi();

    // Serve the Swagger JSON at /swagger/v1/swagger.json
    app.UseSwagger();

    // Serve Swagger UI at the root (http://localhost:5258/)
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = string.Empty;  // Makes Swagger UI available at the root
    });
}

app.UseAuthorization();

app.MapControllers();

app.Run();