using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EarthquakeModel;

[Table("earthquake_events")]
public class EarthquakeEvent
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("latitude")]
    public decimal Latitude { get; set; }

    [Column("longitude")]
    public decimal Longitude { get; set; }

    [Column("magnitude")]
    public decimal Magnitude { get; set; }

    [Column("recorded_at")]
    public DateTime RecordedAt { get; set; } = DateTime.UtcNow;

    [Column("shelter_location_id")]
    public int? ShelterLocationId { get; set; }

    [ForeignKey("ShelterLocationId")]
    [InverseProperty("EarthquakeEvents")]
    public ShelterLocation? ShelterLocation { get; set; }
}
