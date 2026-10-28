using Microsoft.AspNetCore.Mvc;
using EarthquakeAPI.Services;
using EarthquakeDTO;

namespace EarthquakeAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CountriesController : ControllerBase
	{
		private readonly RestCountriesService _countryService;

		public CountriesController(RestCountriesService countryService)
		{
			_countryService = countryService;
		}

		[HttpGet("{countryName}")]
		public async Task<IActionResult> GetCountry(string countryName)
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

			return Ok(country);
		}
	}
}