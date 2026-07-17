using music_time_manager.Application.Services;
using music_time_manager.Infrastructure;
using music_time_manager.Infrastructure.Options;
using music_time_manager.Persistence;
using SneakerStore.FailureHandler;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(nameof(JwtOptions)));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Connecting db
builder.Services.AddPersistence(builder.Configuration);

// Services
builder.Services.AddScoped<IUserService, UserService>();

// Handlers
builder.Services.AddScoped<IFailureHandler, FailureHandler>();

builder.Services.AddScoped<IJwtProvider, JwtProvider>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();

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
