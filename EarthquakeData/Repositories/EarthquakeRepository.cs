using Microsoft.EntityFrameworkCore;

namespace EarthquakeData.Repositories
{
	public class EarthquakeRepository : IEarthquakeRepository
	{
		private readonly EarthquakeDbContext _context;

		public EarthquakeRepository(EarthquakeDbContext context)
		{
			_context = context;
		}

		public async Task<bool> HasEarthquakesAsync()
		{
			return await _context.Earthquakes.AnyAsync();

		}
		public async Task<bool> EarthquakeExistsAsync(string id)
		{
			return await _context.Earthquakes.AnyAsync(e => e.Id == id);
		}

		public async Task AddEarthquakesAsync(List<Earthquake> earthquakes)
		{
			await _context.Earthquakes.AddRangeAsync(earthquakes);
			await _context.SaveChangesAsync();
		}

		public async Task<List<Earthquake>> GetEarthquakesAsync()
		{
			return await _context.Earthquakes.ToListAsync();
		}

		public async Task<bool> DateImportedAsync(DateTime date)
		{
			return await _context.ImportedDates
				.AnyAsync(d => d.Date == date.Date);
		}

		public async Task SaveImportedDateAsync(DateTime date)
		{
			ImportedDate importedDate = new ImportedDate
			{
				Date = date.Date
			};

			await _context.ImportedDates.AddAsync(importedDate);
			await _context.SaveChangesAsync();
		}
		public async Task<List<Earthquake>> GetEarthquakesByCountryAsync(string countryName)
		{
			return await _context.Earthquakes
				.Where(e => e.Place != null &&
							e.Place.Contains(countryName))
				.OrderByDescending(e => e.Time)
				.ToListAsync();
		}
	}
}