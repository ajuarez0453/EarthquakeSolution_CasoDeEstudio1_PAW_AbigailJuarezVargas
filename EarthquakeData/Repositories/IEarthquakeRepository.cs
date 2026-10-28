namespace EarthquakeData.Repositories
{
	public interface IEarthquakeRepository
	{
		Task<bool> HasEarthquakesAsync();
		Task<bool> EarthquakeExistsAsync(string id);
		Task<bool> DateImportedAsync(DateTime date);
		Task SaveImportedDateAsync(DateTime date);
		Task<List<Earthquake>> GetEarthquakesByCountryAsync(string countryName);

		Task AddEarthquakesAsync(List<Earthquake> earthquakes);

		Task<List<Earthquake>> GetEarthquakesAsync();
	}

}
