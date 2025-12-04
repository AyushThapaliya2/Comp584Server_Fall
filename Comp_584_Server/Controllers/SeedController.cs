using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CsvHelper;
using CsvHelper.Configuration;
using WorldModel;
using Comp_584_Server.Data;
using System.Globalization;

namespace Comp_584_Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeedController(WeatherContext context, IHostEnvironment environment) : ControllerBase
    {
        string _pathName = Path.Combine(environment.ContentRootPath, "Data/worldcities.csv");

        [HttpPost]
        [Route("/countries")]
        public async Task<ActionResult> PostCountries()
        {
            Dictionary<string, Country> countries = await context.Countries.AsNoTracking().
                ToDictionaryAsync(c => c.Name, StringComparer.OrdinalIgnoreCase);
            CsvConfiguration config = new(CultureInfo.InvariantCulture) { 
                HasHeaderRecord = true,
                HeaderValidated = null
            };
            using StreamReader reader = new(_pathName);
            using CsvReader csv = new(reader, config);
            List<WorldData> data = csv.GetRecords<WorldData>().ToList();
            foreach (WorldData d in data) {
                if (!countries.ContainsKey(d.country)) {
                    Country country = new() {
                        Name = d.country,
                        Iso2 = d.iso2,
                        Iso3 = d.iso3,
                    };
                    countries.Add(country.Name, country);
                    await context.Countries.AddAsync(country);
                }
            }

            await context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost]
        [Route("/cities")]
        public async Task<ActionResult> PostCities()
        {
            Dictionary<string, Country> countries = await context.Countries.AsNoTracking().
                ToDictionaryAsync(c => c.Name,
                StringComparer.OrdinalIgnoreCase);
            CsvConfiguration config = new(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                HeaderValidated = null
            };
            using StreamReader reader = new(_pathName);
            using CsvReader csv = new(reader, config);
            List<WorldData> data = csv.GetRecords<WorldData>().ToList();

            int cityCount = 0;
            foreach (WorldData record in data)
            {
                if (record.population.HasValue && record.population.Value > 0 && countries.ContainsKey(record.country))
                {
                    City city = new()
                    {
                        Name = record.city,
                        Lat = record.lat,
                        Lng = record.lng,
                        Population = (int)record.population.Value,
                        CountryId = countries[record.country].Id
                    };
                    await context.Cities.AddAsync(city);
                    cityCount++;
                }
            }
            await context.SaveChangesAsync();

            return new JsonResult(cityCount);
        }
    }
}
