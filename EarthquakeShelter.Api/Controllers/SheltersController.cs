using EarthquakeShelter.Api.DTOs;
using EarthquakeModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EarthquakeShelter.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class SheltersController : ControllerBase
{
    private readonly ShelterContext context;

    public SheltersController(ShelterContext context)
    {
        this.context = context;
    }

    // GET: api/Shelters
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ShelterLocation>>> GetShelters()
    {
        return await context.ShelterLocations.ToListAsync();
    }

    // GET: api/Shelters/nearby?lat=...&lng=...&take=3
    [AllowAnonymous]
    [HttpGet("nearby")]
    public async Task<ActionResult<IEnumerable<ShelterDistanceDto>>> GetNearbyShelters([FromQuery] decimal lat, [FromQuery] decimal lng, [FromQuery] int take = 3)
    {
        List<ShelterLocation> shelters = await context.ShelterLocations.AsNoTracking().ToListAsync();
        if (shelters.Count == 0)
        {
            return NotFound("No shelters recorded.");
        }

        take = Math.Clamp(take, 1, 10);

        List<ShelterDistanceDto> ranked = shelters
            .Select(s => new ShelterDistanceDto
            {
                Id = s.Id,
                Name = s.Name,
                Latitude = s.Latitude,
                Longitude = s.Longitude,
                Capacity = s.Capacity,
                IsOpen = s.IsOpen,
                DistanceKm = CalculateDistanceKm((double)lat, (double)lng, (double)s.Latitude, (double)s.Longitude)
            })
            .OrderBy(s => s.DistanceKm)
            .Take(take)
            .ToList();

        return ranked;
    }

    // GET: api/Shelters/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ShelterLocation>> GetShelter(int id)
    {
        ShelterLocation? shelter = await context.ShelterLocations.FindAsync(id);

        if (shelter == null)
        {
            return NotFound();
        }

        return shelter;
    }

    // PUT: api/Shelters/5
    [HttpPut("{id:int}")]
    public async Task<IActionResult> PutShelter(int id, ShelterLocation shelter)
    {
        if (id != shelter.Id)
        {
            return BadRequest();
        }

        context.Entry(shelter).State = EntityState.Modified;

        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ShelterExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    // POST: api/Shelters
    [HttpPost]
    public async Task<ActionResult<ShelterLocation>> PostShelter(ShelterLocation shelter)
    {
        context.ShelterLocations.Add(shelter);
        await context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetShelter), new { id = shelter.Id }, shelter);
    }

    // DELETE: api/Shelters/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteShelter(int id)
    {
        ShelterLocation? shelter = await context.ShelterLocations.FindAsync(id);
        if (shelter == null)
        {
            return NotFound();
        }

        context.ShelterLocations.Remove(shelter);
        await context.SaveChangesAsync();

        return NoContent();
    }

    private bool ShelterExists(int id)
    {
        return context.ShelterLocations.Any(e => e.Id == id);
    }

    private static double CalculateDistanceKm(double lat1, double lon1, double lat2, double lon2)
    {
        const double EarthRadiusKm = 6371;
        double dLat = DegreesToRadians(lat2 - lat1);
        double dLon = DegreesToRadians(lon2 - lon1);
        double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                   Math.Cos(DegreesToRadians(lat1)) * Math.Cos(DegreesToRadians(lat2)) *
                   Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return EarthRadiusKm * c;
    }

    private static double DegreesToRadians(double degrees) => degrees * Math.PI / 180;
}
