using Microsoft.EntityFrameworkCore;
using PhotoOrganizer.Application.Abstractions;
using PhotoOrganizer.Application.Services;
using PhotoOrganizer.Infrastructure.Data;
using PhotoOrganizer.Infrastructure.FileStorage;
using PhotoOrganizer.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("ConnectionStrings:DefaultConnection must be configured.");

builder.Services.AddDbContext<PhotoOrganizerDbContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IAlbumRepository, AlbumRepository>();
builder.Services.AddSingleton<ILocalFileStorage, LocalFileStorage>();
builder.Services.AddSingleton<IPhotoMetadataReader, PhotoMetadataReader>();
builder.Services.AddScoped<AlbumGroupingService>();
builder.Services.AddScoped<AlbumSummaryService>();
builder.Services.AddScoped<AlbumManagementService>();
builder.Services.AddScoped<PhotoLibraryService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    await using var scope = app.Services.CreateAsyncScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<PhotoOrganizerDbContext>();
    var databaseCreated = await dbContext.Database.EnsureCreatedAsync();
    if (databaseCreated)
    {
        scope.ServiceProvider.GetRequiredService<ILogger<Program>>()
            .LogInformation("Created the local PhotoOrganizer database and schema.");
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHttpsRedirection();
}
app.MapControllers();
app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.Run();

public partial class Program { }
