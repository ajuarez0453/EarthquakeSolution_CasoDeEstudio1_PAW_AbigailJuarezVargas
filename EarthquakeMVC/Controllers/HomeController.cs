using Microsoft.AspNetCore.Mvc;
using EarthquakeMVC.Services;
using EarthquakeDTO;
using System.Text.RegularExpressions;

namespace EarthquakeMVC.Controllers
{
	public class HomeController : Controller
	{
		private readonly EarthquakeApiService _earthquakeService;

		public HomeController(EarthquakeApiService earthquakeService)
		{
			_earthquakeService = earthquakeService;
		}

		public IActionResult Index()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Search(string? countryName)
		{
			try
			{
				if (string.IsNullOrWhiteSpace(countryName))
				{
					TempData["Error"] = "Debe escribir el nombre de un país.";
					return RedirectToAction("Index");
				}

				countryName = countryName.Trim();

				if (countryName.Length > 100)
				{
					TempData["Error"] = "El nombre del país no puede superar los 100 caracteres.";
					return RedirectToAction("Index");
				}

				if (!Regex.IsMatch(countryName,
					@"^[a-zA-ZáéíóúÁÉÍÓÚñÑüÜ\s]+$"))
				{
					TempData["Error"] = "Solo se permiten letras y espacios.";
					return RedirectToAction("Index");
				}

				CountryEarthquakesDto? result =
					await _earthquakeService.GetEarthquakesByCountryAsync(countryName);

				if (result == null)
				{
					TempData["Error"] = "El país ingresado no existe.";
					return RedirectToAction("Index");
				}

				return View("Index", result);
			}
			catch (Exception)
			{
				TempData["Error"] =
					"No se pudo completar la búsqueda. Verifique el país o intente nuevamente.";

				return RedirectToAction("Index");
			}
		}
	}
}