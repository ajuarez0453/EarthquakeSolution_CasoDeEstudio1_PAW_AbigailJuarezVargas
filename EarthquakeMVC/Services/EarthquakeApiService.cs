using System.Net;
using System.Net.Http.Json;
using EarthquakeDTO;

namespace EarthquakeMVC.Services
{
	public class EarthquakeApiService
	{
		private readonly HttpClient _httpClient;

		public EarthquakeApiService(HttpClient httpClient)
		{
			_httpClient = httpClient;
		}

		public async Task<CountryEarthquakesDto?> GetEarthquakesByCountryAsync(
			string countryName)
		{
			string url = "api/Earthquakes/" +
				Uri.EscapeDataString(countryName);

			using var response = await _httpClient.GetAsync(url);

			if (response.StatusCode == HttpStatusCode.NotFound)
			{
				return null;
			}

			response.EnsureSuccessStatusCode();

			return await response.Content
				.ReadFromJsonAsync<CountryEarthquakesDto>();
		}
	}
}