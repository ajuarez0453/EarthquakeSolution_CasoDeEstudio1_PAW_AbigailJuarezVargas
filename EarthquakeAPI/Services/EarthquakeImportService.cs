using EarthquakeData;
using EarthquakeData.Repositories;

namespace EarthquakeAPI.Services
{
	public class EarthquakeImportService
	{
		private readonly EarthquakeService _earthquakeService;
		private readonly IEarthquakeRepository _repository;

		public EarthquakeImportService(
			EarthquakeService earthquakeService,
			IEarthquakeRepository repository)
		{
			_earthquakeService = earthquakeService;
			_repository = repository;
		}

		public async Task ImportEarthquakesAsync()
		{
			DateTime startDate = new DateTime(2020, 1, 1);
			DateTime finalDate = new DateTime(2026, 10, 10);

			while (startDate <= finalDate)
			{
				bool dateImported = await _repository.DateImportedAsync(startDate);

				if (!dateImported)
				{
					DateTime endDate = startDate.AddDays(1);

					var features = await _earthquakeService
						.GetEarthquakesAsync(startDate, endDate);

					List<Earthquake> earthquakes = new List<Earthquake>();

					foreach (var feature in features)
					{
						if (feature.Geometry.Coordinates.Count < 3)
						{
							continue;
						}

						bool exists = await _repository
							.EarthquakeExistsAsync(feature.Id);

						if (exists)
						{
							continue;
						}

						Earthquake earthquake = new Earthquake
						{
							Id = feature.Id,
							Magnitude = feature.Properties.Magnitude,
							Place = feature.Properties.Place,
							Time = DateTimeOffset
								.FromUnixTimeMilliseconds(feature.Properties.Time)
								.UtcDateTime,
							Url = feature.Properties.Url,
							Longitude = feature.Geometry.Coordinates[0],
							Latitude = feature.Geometry.Coordinates[1],
							Depth = feature.Geometry.Coordinates[2]
						};

						earthquakes.Add(earthquake);
					}

					if (earthquakes.Count > 0)
					{
						await _repository.AddEarthquakesAsync(earthquakes);
					}

					await _repository.SaveImportedDateAsync(startDate);

					Console.WriteLine("Fecha importada: " +
						startDate.ToString("yyyy-MM-dd"));
				}

				await Task.Delay(1000);
				startDate = startDate.AddDays(1);
			}
		}
	}
}