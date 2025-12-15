namespace EarthquakeShelter.Api;

public class SeismicForecast
{
    public DateOnly Date { get; set; }

    public decimal ExpectedMagnitude { get; set; }

    public string? Summary { get; set; }
}
