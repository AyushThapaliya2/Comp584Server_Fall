using EarthquakeModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EarthquakeShelter.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class EarthquakesController(ShelterContext context) : ControllerBase
{
    // GET: api/Earthquakes
    [HttpGet]
    public async Task<ActionResult<IEnumerable<EarthquakeEvent>>> GetEarthquakes()
    {
        return await context.EarthquakeEvents
            .OrderByDescending(e => e.RecordedAt)
            .ToListAsync();
    }

    // GET: api/Earthquakes/alerts
    [HttpGet("alerts")]
    public async Task<ActionResult<IEnumerable<EarthquakeEvent>>> GetActiveEarthquakes()
    {
        return await context.EarthquakeEvents
            .Where(e => e.Magnitude > 0)
            .OrderByDescending(e => e.Magnitude)
            .ToListAsync();
    }

    // GET: api/Earthquakes/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<EarthquakeEvent>> GetEarthquake(int id)
    {
        EarthquakeEvent? quake = await context.EarthquakeEvents.FindAsync(id);

        if (quake == null)
        {
            return NotFound();
        }

        return quake;
    }

    // PUT: api/Earthquakes/5
    [HttpPut("{id:int}")]
    public async Task<IActionResult> PutEarthquake(int id, EarthquakeEvent quake)
    {
        if (id != quake.Id)
        {
            return BadRequest();
        }

        context.Entry(quake).State = EntityState.Modified;

        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!EarthquakeExists(id))
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

    // POST: api/Earthquakes
    [HttpPost]
    public async Task<ActionResult<EarthquakeEvent>> PostEarthquake(EarthquakeEvent quake)
    {
        context.EarthquakeEvents.Add(quake);
        await context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetEarthquake), new { id = quake.Id }, quake);
    }

    // DELETE: api/Earthquakes/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteEarthquake(int id)
    {
        EarthquakeEvent? quake = await context.EarthquakeEvents.FindAsync(id);
        if (quake == null)
        {
            return NotFound();
        }

        context.EarthquakeEvents.Remove(quake);
        await context.SaveChangesAsync();

        return NoContent();
    }

    private bool EarthquakeExists(int id)
    {
        return context.EarthquakeEvents.Any(e => e.Id == id);
    }
}
