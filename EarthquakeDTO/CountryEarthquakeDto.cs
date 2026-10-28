namespace EarthquakeDTO
{
	public class CountryEarthquakesDto
	{
		public CountryDto Country { get; set; } = new CountryDto();

		public List<EarthquakeDto> Earthquakes { get; set; } =
			new List<EarthquakeDto>();

		public int TotalEarthquakes { get; set; }
	}
}