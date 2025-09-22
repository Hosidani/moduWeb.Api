using Microsoft.EntityFrameworkCore;
using ModuWeb.Api.Models;
using Hangfire;
using Hangfire.MemoryStorage;

var builder = WebApplication.CreateBuilder(args);

// Register ReminderService
builder.Services.AddSingleton<ModuWeb.Api.Services.IReminderService, ModuWeb.Api.Services.ReminderService>();

// Add services to the container.
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy => policy
            .WithOrigins("https://localhost:5173", "https://hosidani.dev")
            .AllowAnyHeader()
            .AllowAnyMethod());
});

// Register SmtpEmailService with config from appsettings
builder.Services.AddSingleton(provider =>
    new ModuWeb.Api.Services.SmtpEmailService(
        smtpHost: builder.Configuration["Smtp:Host"] ?? "smtp.example.com",
        smtpPort: int.TryParse(builder.Configuration["Smtp:Port"], out var port) ? port : 587,
        smtpUser: builder.Configuration["Smtp:User"] ?? "user@example.com",
        smtpPass: builder.Configuration["Smtp:Pass"] ?? "password",
        fromAddress: builder.Configuration["Smtp:From"] ?? "noreply@example.com"
    ));

// Add Hangfire services
builder.Services.AddHangfire(config =>
    config.UseMemoryStorage());
builder.Services.AddHangfireServer();

var app = builder.Build();

app.UseCors("AllowFrontend");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseHangfireDashboard();
}

app.UseHttpsRedirection();
app.MapControllers();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.Run();

public record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
