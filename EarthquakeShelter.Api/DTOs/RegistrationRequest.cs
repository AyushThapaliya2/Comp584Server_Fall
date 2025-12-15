namespace EarthquakeShelter.Api.DTOs;

public class RegistrationRequest
{
    public required string UserName { get; set; }
    public required string Password { get; set; }
    public string? Email { get; set; }
}
