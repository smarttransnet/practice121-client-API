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

            var needsReseed = !await context.Districts.AnyAsync() || !await context.Places.AnyAsync(p => p.Address != null);
            if (needsReseed)
            {
                logger.LogInformation("Seeding / Re-seeding Locations...");

                if (await context.Districts.AnyAsync())
                {
                    logger.LogInformation("Clearing legacy locations data...");
                    context.PracticeCentres.RemoveRange(await context.PracticeCentres.ToListAsync());
                    context.Places.RemoveRange(await context.Places.ToListAsync());
                    context.MohAreas.RemoveRange(await context.MohAreas.ToListAsync());
                    context.Districts.RemoveRange(await context.Districts.ToListAsync());
                    await context.SaveChangesAsync();
                }

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

                                var seenPlaceNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                                foreach (var pDto in mDto.Places)
                                {
                                    var cleanName = pDto.Name.Trim();
                                    if (!seenPlaceNames.Add(cleanName))
                                    {
                                        continue;
                                    }

                                    var place = Place.Create(
                                        Guid.NewGuid(), 
                                        mohArea.Id, 
                                        cleanName, 
                                        isVerified: true, 
                                        address: string.IsNullOrWhiteSpace(pDto.Address) ? null : pDto.Address, 
                                        registrationNumber: string.IsNullOrWhiteSpace(pDto.Registration_Number) ? null : pDto.Registration_Number);
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

#pragma warning disable S3459, S1144
    private sealed class DistrictDto
    {
        public string Name { get; set; } = string.Empty;
        public List<MohAreaDto> Moh_areas { get; set; } = [];
    }

    private sealed class MohAreaDto
    {
        public string Name { get; set; } = string.Empty;
        public List<PlaceDto> Places { get; set; } = [];
    }

    private sealed class PlaceDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? Registration_Number { get; set; }
    }
#pragma warning restore S3459, S1144
}
