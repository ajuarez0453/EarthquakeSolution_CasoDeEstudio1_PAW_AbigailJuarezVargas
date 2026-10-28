using EarthquakeData;
using Microsoft.EntityFrameworkCore;
using EarthquakeData.Repositories;
using EarthquakeAPI.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<EarthquakeDbContext>(options =>
	options.UseSqlServer(
		builder.Configuration.GetConnectionString("EarthquakeConnection")
	)
);

builder.Services.AddScoped<IEarthquakeRepository, EarthquakeRepository>();
builder.Services.AddHttpClient<EarthquakeService>();
builder.Services.AddScoped<EarthquakeImportService>();
builder.Services.AddHttpClient<RestCountriesService>();
builder.Services.AddMemoryCache();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Iniciar la importacion cuando la API ya este funcionando
app.Lifetime.ApplicationStarted.Register(() =>
{
	_ = Task.Run(async () =>
	{
		try
		{
			using var scope = app.Services.CreateScope();

			var importService = scope.ServiceProvider
				.GetRequiredService<EarthquakeImportService>();

			await importService.ImportEarthquakesAsync();
		}
		catch (Exception ex)
		{
			app.Logger.LogError(ex, "Error al importar terremotos");
		}
	});
});

app.Run();