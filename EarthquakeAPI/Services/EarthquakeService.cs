using System.Text.Json;
using EarthquakeAPI.Models;
using Microsoft.Extensions.Caching.Memory;

namespace EarthquakeAPI.Services
{
	public class EarthquakeService
	{
		private readonly HttpClient _httpClient;
		private readonly IMemoryCache _cache;

		public EarthquakeService(HttpClient httpClient, IMemoryCache cache)
		{
			_httpClient = httpClient;
			_cache = cache;
		}

		public async Task<List<UsgsFeature>> GetEarthquakesAsync(
			DateTime startDate, DateTime endDate)
		{
			string start = startDate.ToString("yyyy-MM-dd");
			string end = endDate.ToString("yyyy-MM-dd");

			string cacheKey = "earthquakes_" + start + "_" + end;

			if (_cache.TryGetValue(cacheKey, out List<UsgsFeature>? cachedData)
				&& cachedData != null)
			{
				return cachedData;
			}

			string url = "https://earthquake.usgs.gov/fdsnws/event/1/query" +
				"?format=geojson" +
				"&starttime=" + start +
				"&endtime=" + end +
				"&minmagnitude=3";

			for (int attempt = 1; attempt <= 3; attempt++)
			{
				try
				{
					string json = await _httpClient.GetStringAsync(url);

					UsgsResponse? response =
						JsonSerializer.Deserialize<UsgsResponse>(json);

					if (response == null)
					{
						throw new JsonException("Respuesta JSON no válida.");
					}

					_cache.Set(cacheKey, response.Features,
						TimeSpan.FromMinutes(30));

					return response.Features;
				}
				catch (HttpRequestException ex) when (
					attempt < 3 &&
					(ex.StatusCode == System.Net.HttpStatusCode.GatewayTimeout ||
					 ex.StatusCode == System.Net.HttpStatusCode.BadGateway ||
					 ex.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable))
				{
					Console.WriteLine("Error temporal de USGS. Intento " + attempt);
					await Task.Delay(3000);
				}
			}

			throw new Exception("No se pudieron obtener los terremotos.");
		}
	}
}