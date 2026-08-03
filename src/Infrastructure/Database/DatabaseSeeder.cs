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

            logger.LogInformation("Syncing / Updating Locations data from locations.json...");

            string? json = null;
            var assembly = typeof(DatabaseSeeder).Assembly;
            var resourceName = assembly.GetManifestResourceNames()
                .FirstOrDefault(n => n.EndsWith("locations.json", StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrEmpty(resourceName))
            {
                using var stream = assembly.GetManifestResourceStream(resourceName);
                if (stream != null)
                {
                    using var reader = new StreamReader(stream);
                    json = await reader.ReadToEndAsync();
                }
            }

            if (string.IsNullOrEmpty(json))
            {
                var basePath = AppContext.BaseDirectory;
                var seedDataPath = Path.Combine(basePath, "SeedData", "locations.json");
                if (!File.Exists(seedDataPath))
                {
                    seedDataPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "Infrastructure", "Database", "SeedData", "locations.json");
                }
                if (File.Exists(seedDataPath))
                {
                    json = await File.ReadAllTextAsync(seedDataPath);
                }
            }

            if (!string.IsNullOrEmpty(json))
            {
                var districtsDto = JsonSerializer.Deserialize<List<DistrictDto>>(json, _jsonOptions);

                if (districtsDto != null)
                {
                    var existingDistricts = await context.Districts.Include(d => d.MohAreas).ThenInclude(m => m.Places).ToListAsync();
                    var districtMap = existingDistricts.ToDictionary(d => d.Name.Trim(), StringComparer.OrdinalIgnoreCase);

                    bool changes = false;

                    foreach (var dDto in districtsDto)
                    {
                        var cleanDistName = dDto.Name.Trim();
                        if (!districtMap.TryGetValue(cleanDistName, out var district))
                        {
                            district = District.Create(Guid.NewGuid(), cleanDistName);
                            context.Districts.Add(district);
                            districtMap[cleanDistName] = district;
                            changes = true;
                        }

                        var existingMohs = district.MohAreas ?? new List<MohArea>();
                        var mohMap = existingMohs.ToDictionary(m => m.Name.Trim(), StringComparer.OrdinalIgnoreCase);

                        foreach (var mDto in dDto.Moh_areas)
                        {
                            var cleanMohName = mDto.Name.Trim();
                            if (!mohMap.TryGetValue(cleanMohName, out var mohArea))
                            {
                                mohArea = MohArea.Create(Guid.NewGuid(), district.Id, cleanMohName);
                                context.MohAreas.Add(mohArea);
                                mohMap[cleanMohName] = mohArea;
                                changes = true;
                            }

                            var existingPlaces = mohArea.Places ?? new List<Place>();
                            var seenPlaceNames = new HashSet<string>(existingPlaces.Select(p => p.Name.Trim()), StringComparer.OrdinalIgnoreCase);

                            foreach (var pDto in mDto.Places)
                            {
                                var cleanPlaceName = pDto.Name.Trim();
                                if (seenPlaceNames.Add(cleanPlaceName))
                                {
                                    var place = Place.Create(
                                        Guid.NewGuid(),
                                        mohArea.Id,
                                        cleanPlaceName,
                                        isVerified: true,
                                        address: string.IsNullOrWhiteSpace(pDto.Address) ? null : pDto.Address,
                                        registrationNumber: string.IsNullOrWhiteSpace(pDto.Registration_Number) ? null : pDto.Registration_Number);
                                    context.Places.Add(place);
                                    changes = true;
                                }
                            }
                        }
                    }

                    if (changes)
                    {
                        await context.SaveChangesAsync();
                        logger.LogInformation("Successfully synced new Districts, MOH Areas, and Places to Database.");
                    }
                    else
                    {
                        logger.LogInformation("Locations data is up-to-date in Database.");
                    }
                }
            }
            else
            {
                logger.LogWarning("locations.json seed data could not be loaded.");
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
