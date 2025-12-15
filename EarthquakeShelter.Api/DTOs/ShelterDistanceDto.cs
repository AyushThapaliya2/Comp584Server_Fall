namespace EarthquakeShelter.Api.DTOs;

public class ShelterDistanceDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public int Capacity { get; set; }
    public bool IsOpen { get; set; }
    public double DistanceKm { get; set; }
}
