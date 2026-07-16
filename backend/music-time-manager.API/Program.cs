using music_time_manager.Application.Services;
using music_time_manager.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Connecting db
builder.Services.AddPersistence(builder.Configuration);

// Services
builder.Services.AddScoped<IUserService, UserService>();

var app = builder.Build();

var logger = app.Services.GetRequiredService<ILogger<Program>>();

using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;
    try
    {
        var dbContext = serviceProvider.GetRequiredService<MusicTimeManagerDbContext>();
        await DbInitializer.InitializeAsync(dbContext);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, ex.Message);
        throw;
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
