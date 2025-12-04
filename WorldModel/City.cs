using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WorldModel;

[Table("cities")]
public partial class City
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("countryID")]
    public int CountryId { get; set; }

    [Column("name")]
    [StringLength(50)]
    [Unicode(false)]
    public string Name { get; set; } = null!;

    [Column("population")]
    public int Population { get; set; }

    [Column("lat")]
    public decimal Lat { get; set; }

    [Column("lng")]
    public decimal Lng { get; set; }

    [ForeignKey("CountryId")]
    [InverseProperty("Cities")]
    public virtual Country Country { get; set; } = null!;
}
