using Microsoft.EntityFrameworkCore;

namespace EarthquakeData
{
	public class EarthquakeDbContext : DbContext
	{
		public EarthquakeDbContext(DbContextOptions<EarthquakeDbContext> options)
			: base(options)
		{
		}

		public DbSet<Earthquake> Earthquakes { get; set; }
		public DbSet<ImportedDate> ImportedDates { get; set; }
	}
}