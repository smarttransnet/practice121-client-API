using System.Text.Json;
using Domain.Locations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Database;

public static class DatabaseSeeder
{
    private static readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("DatabaseSeeder");

        try
        {
            await context.Database.MigrateAsync();

            if (!await context.Districts.AnyAsync())
            {
                logger.LogInformation("Seeding Locations...");
                var basePath = AppContext.BaseDirectory;
                // Walk up to find Infrastructure/Database/SeedData if running locally, or use a specific path
                var seedDataPath = Path.Combine(basePath, "SeedData", "locations.json");
                
                // For development, it might be easier to just read from the source folder if BaseDirectory doesn't have it copied.
                if (!File.Exists(seedDataPath))
                {
                    // Fallback to project path assuming we are running via dotnet run in Web.Api
                    seedDataPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "Infrastructure", "Database", "SeedData", "locations.json");
                }

                if (File.Exists(seedDataPath))
                {
                    var json = await File.ReadAllTextAsync(seedDataPath);
                    var districtsDto = JsonSerializer.Deserialize<List<DistrictDto>>(json, _jsonOptions);
                    
                    if (districtsDto != null)
                    {
                        foreach (var dDto in districtsDto)
                        {
                            var district = District.Create(Guid.NewGuid(), dDto.Name);
                            context.Districts.Add(district);

                            foreach (var mDto in dDto.Moh_areas)
                            {
                                var mohArea = MohArea.Create(Guid.NewGuid(), district.Id, mDto.Name);
                                context.MohAreas.Add(mohArea);

                                foreach (var pName in mDto.Places)
                                {
                                    var place = Place.Create(Guid.NewGuid(), mohArea.Id, pName, isVerified: true);
                                    context.Places.Add(place);
                                }
                            }
                        }
                        await context.SaveChangesAsync();
                        logger.LogInformation("Successfully seeded Locations.");
                    }
                }
                else
                {
                    logger.LogWarning("locations.json not found at {Path}", seedDataPath);
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the database.");
        }
    }

    private sealed class DistrictDto
    {
        public string Name { get; set; } = string.Empty;
        public List<MohAreaDto> Moh_areas { get; set; } = new();
    }

    private sealed class MohAreaDto
    {
        public string Name { get; set; } = string.Empty;
        public List<string> Places { get; set; } = new();
    }
}
