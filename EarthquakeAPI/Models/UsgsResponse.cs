using System.Text.Json.Serialization;

namespace EarthquakeAPI.Models
{
	public class UsgsResponse
	{
		[JsonPropertyName("features")]
		public List<UsgsFeature> Features { get; set; } = new();
	}

	public class UsgsFeature
	{
		[JsonPropertyName("id")]
		public string Id { get; set; } = string.Empty;

		[JsonPropertyName("properties")]
		public UsgsProperties Properties { get; set; } = new();

		[JsonPropertyName("geometry")]
		public UsgsGeometry Geometry { get; set; } = new();
	}

	public class UsgsProperties
	{
		[JsonPropertyName("mag")]
		public double? Magnitude { get; set; }

		[JsonPropertyName("place")]
		public string? Place { get; set; }

		[JsonPropertyName("time")]
		public long Time { get; set; }

		[JsonPropertyName("url")]
		public string? Url { get; set; }
	}

	public class UsgsGeometry
	{
		[JsonPropertyName("coordinates")]
		public List<double> Coordinates { get; set; } = new();
	}
}