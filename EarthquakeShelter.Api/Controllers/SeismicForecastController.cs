using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EarthquakeShelter.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SeismicForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Calm",
        "Minor surface tremors",
        "Noticeable shaking possible",
        "Strong shaking possible",
        "Prepare to evacuate"
    };

    private readonly ILogger<SeismicForecastController> logger;

    public SeismicForecastController(ILogger<SeismicForecastController> logger)
    {
        this.logger = logger;
    }

    [Authorize]
    [HttpGet]
    public IEnumerable<SeismicForecast> Get()
    {
        logger.LogInformation("Forecast requested");
        return Enumerable.Range(1, 5).Select(index => new SeismicForecast
        {
            Date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(index)),
            ExpectedMagnitude = Math.Round((decimal)Random.Shared.NextDouble() * 4.5m, 1),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        });
    }
}
