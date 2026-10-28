using System.Net.Http.Headers;
using System.Text.Json;
using EarthquakeDTO;
using Microsoft.Extensions.Caching.Memory;

namespace EarthquakeAPI.Services
{
	public class RestCountriesService
	{
		private readonly HttpClient _httpClient;
		private readonly IConfiguration _configuration;
		private readonly IMemoryCache _cache;

		public RestCountriesService(
			HttpClient httpClient,
			IConfiguration configuration,
			IMemoryCache cache)
		{
			_httpClient = httpClient;
			_configuration = configuration;
			_cache = cache;
		}

		public async Task<CountryDto?> GetCountryAsync(string countryName)
		{
			string cacheKey = "country_" + countryName.Trim().ToLowerInvariant();

			if (_cache.TryGetValue(cacheKey, out CountryDto? cachedCountry))
			{
				return cachedCountry;
			}

			string baseUrl = _configuration["RestCountries:BaseUrl"]!;
			string apiKey = _configuration["RestCountries:ApiKey"]!;

			string url = baseUrl + "/countries/v5/names.common/" +
				Uri.EscapeDataString(countryName);

			using var request = new HttpRequestMessage(HttpMethod.Get, url);

			request.Headers.Authorization =
				new AuthenticationHeaderValue("Bearer", apiKey);

			using var response = await _httpClient.SendAsync(request);

			response.EnsureSuccessStatusCode();

			string json = await response.Content.ReadAsStringAsync();

			using JsonDocument document = JsonDocument.Parse(json);

			JsonElement countries = document.RootElement
				.GetProperty("data")
				.GetProperty("objects");

			if (countries.GetArrayLength() == 0)
			{
				return null;
			}

			JsonElement country = countries[0];

			CountryDto countryDto = new CountryDto();

			countryDto.Name = country.GetProperty("names")
				.GetProperty("common").GetString() ?? "";

			if (country.TryGetProperty("capitals", out JsonElement capitals) &&
				capitals.ValueKind == JsonValueKind.Array &&
				capitals.GetArrayLength() > 0)
			{
				countryDto.Capital = capitals[0]
					.GetProperty("name").GetString() ?? "";
			}

			if (country.TryGetProperty("region", out JsonElement region))
			{
				countryDto.Region = region.GetString() ?? "";
			}

			if (country.TryGetProperty("population", out JsonElement population))
			{
				countryDto.Population = population.GetInt64();
			}

			if (country.TryGetProperty("flag", out JsonElement flag) &&
				flag.TryGetProperty("url_png", out JsonElement flagUrl))
			{
				countryDto.Flag = flagUrl.GetString() ?? "";
			}

			_cache.Set(cacheKey, countryDto, TimeSpan.FromMinutes(30));

			return countryDto;
		}
	}
}