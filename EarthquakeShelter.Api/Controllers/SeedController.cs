using CsvHelper;
using CsvHelper.Configuration;
using EarthquakeShelter.Api.Data;
using EarthquakeModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.IO;

namespace EarthquakeShelter.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[AllowAnonymous]
public class SeedController(ShelterContext context, IHostEnvironment environment) : ControllerBase
{
    private readonly string pathName = Path.Combine(environment.ContentRootPath, "Data", "earthquake_locations.csv");

    [HttpPost("earthquakes")]
    public async Task<ActionResult> SeedEarthquakes()
    {
        if (!System.IO.File.Exists(pathName))
        {
            return NotFound("Seed file not found.");
        }

        bool hasExisting = await context.EarthquakeEvents.AnyAsync();
        CsvConfiguration config = new(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            HeaderValidated = null
        };

        using StreamReader reader = new(pathName);
        using CsvReader csv = new(reader, config);
        List<EarthquakeCsvRecord> data = csv.GetRecords<EarthquakeCsvRecord>().ToList();

        foreach (EarthquakeCsvRecord record in data)
        {
            EarthquakeEvent quake = new()
            {
                Latitude = record.lat,
                Longitude = record.lng,
                Magnitude = record.magnitude,
                RecordedAt = DateTime.UtcNow
            };
            context.EarthquakeEvents.Add(quake);
        }

        await context.SaveChangesAsync();
        return Ok(new { created = data.Count, previouslySeeded = hasExisting });
    }

    [HttpPost("shelters")]
    public async Task<ActionResult> SeedShelters()
    {
        List<ShelterLocation> templates = new()
        {
            new() { Name = "Westwood Recreation Shelter", Latitude = 34.046700m, Longitude = -118.445200m, Capacity = 300 },
            new() { Name = "San Fernando Valley Hub", Latitude = 34.230500m, Longitude = -118.536900m, Capacity = 250 },
            new() { Name = "Downtown LA Convention Shelter", Latitude = 34.040700m, Longitude = -118.269000m, Capacity = 800 },
            new() { Name = "Hollywood High School Shelter", Latitude = 34.101600m, Longitude = -118.326900m, Capacity = 500 },
            new() { Name = "Long Beach Community Shelter", Latitude = 33.770100m, Longitude = -118.193700m, Capacity = 400 }
        };

        Dictionary<string, ShelterLocation> existing = await context.ShelterLocations
            .AsNoTracking()
            .ToDictionaryAsync(s => s.Name, StringComparer.OrdinalIgnoreCase);

        int created = 0;
        foreach (ShelterLocation shelter in templates)
        {
            if (existing.ContainsKey(shelter.Name))
            {
                continue;
            }

            await context.ShelterLocations.AddAsync(shelter);
            created++;
        }

        if (created == 0)
        {
            return Ok(new { created, message = "Shelters already present." });
        }

        await context.SaveChangesAsync();
        return Ok(new { created });
    }
}
