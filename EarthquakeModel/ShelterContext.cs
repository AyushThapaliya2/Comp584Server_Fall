using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace EarthquakeModel;

public partial class ShelterContext : IdentityDbContext<ShelterUser>
{
    public ShelterContext()
    {
    }

    public ShelterContext(DbContextOptions<ShelterContext> options)
        : base(options)
    {
    }

    public virtual DbSet<EarthquakeEvent> EarthquakeEvents { get; set; }

    public virtual DbSet<ShelterLocation> ShelterLocations { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        IConfigurationBuilder configBuilder = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json").AddJsonFile("appsettings.Development.json", optional: true);
        IConfiguration config = configBuilder.Build();
        if (!optionsBuilder.IsConfigured) {
            optionsBuilder.UseSqlServer(config.GetConnectionString("DefaultConnection"));
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure custom table details for the earthquake and shelter domain.
        modelBuilder.Entity<EarthquakeEvent>(entity =>
        {
            entity.Property(e => e.Latitude).HasPrecision(9, 6);
            entity.Property(e => e.Longitude).HasPrecision(9, 6);
            entity.Property(e => e.Magnitude).HasPrecision(4, 2);
            entity.HasOne(e => e.ShelterLocation)
                .WithMany(s => s.EarthquakeEvents)
                .HasForeignKey(e => e.ShelterLocationId)
                .HasConstraintName("FK_earthquake_events_shelter_locations");
        });

        modelBuilder.Entity<ShelterLocation>(entity =>
        {
            entity.Property(e => e.Name).IsUnicode(false);
            entity.Property(e => e.Latitude).HasPrecision(9, 6);
            entity.Property(e => e.Longitude).HasPrecision(9, 6);
        });
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
