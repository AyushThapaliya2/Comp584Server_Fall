using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace EarthquakeModel;

/// <summary>
/// Provides a design-time factory so dotnet-ef can create the DbContext without running the web host.
/// </summary>
public class DesignTimeShelterContextFactory : IDesignTimeDbContextFactory<ShelterContext>
{
    public ShelterContext CreateDbContext(string[] args)
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .Build();

        DbContextOptionsBuilder<ShelterContext> options = new();
        string connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        options.UseSqlServer(connectionString);

        return new ShelterContext(options.Options);
    }
}
