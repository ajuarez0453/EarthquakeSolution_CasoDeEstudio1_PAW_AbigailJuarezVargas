using Microsoft.AspNetCore.Mvc;
using EarthquakeData.Repositories;
using EarthquakeAPI.Services;
using EarthquakeDTO;

namespace EarthquakeAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class EarthquakesController : ControllerBase
	{
		private readonly IEarthquakeRepository _repository;
		private readonly RestCountriesService _countryService;

		public EarthquakesController(
			IEarthquakeRepository repository,
			RestCountriesService countryService)
		{
			_repository = repository;
			_countryService = countryService;
		}

		[HttpGet("{countryName}")]
		public async Task<IActionResult> GetByCountry(string countryName)
		{
			if (string.IsNullOrWhiteSpace(countryName))
			{
				return BadRequest("Debe ingresar un país.");
			}

			CountryDto? country = await _countryService.GetCountryAsync(countryName);

			if (country == null)
			{
				return NotFound("No se encontró el país.");
			}

			var earthquakes = await _repository
				.GetEarthquakesByCountryAsync(country.Name);

			List<EarthquakeDto> earthquakeDtos = earthquakes.Select(e =>
				new EarthquakeDto
				{
					Id = e.Id,
					Magnitude = e.Magnitude,
					Place = e.Place,
					Time = e.Time,
					Url = e.Url,
					Longitude = e.Longitude,
					Latitude = e.Latitude,
					Depth = e.Depth
				}).ToList();

			CountryEarthquakesDto result = new CountryEarthquakesDto
			{
				Country = country,
				Earthquakes = earthquakeDtos,
				TotalEarthquakes = earthquakeDtos.Count
			};

			return Ok(result);
		}
	}
}