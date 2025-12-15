using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EarthquakeModel;

[Table("shelter_locations")]
public class ShelterLocation
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [Column("name")]
    [StringLength(120)]
    [Unicode(false)]
    public string Name { get; set; } = string.Empty;

    [Column("latitude")]
    public decimal Latitude { get; set; }

    [Column("longitude")]
    public decimal Longitude { get; set; }

    [Column("capacity")]
    public int Capacity { get; set; }

    [Column("is_open")]
    public bool IsOpen { get; set; } = true;

    [InverseProperty("ShelterLocation")]
    public ICollection<EarthquakeEvent> EarthquakeEvents { get; set; } = new List<EarthquakeEvent>();
}
