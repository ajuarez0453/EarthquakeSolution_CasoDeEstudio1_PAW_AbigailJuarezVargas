namespace EarthquakeData
{
	public class Earthquake
	{
		public string Id { get; set; } = string.Empty;
		public double? Magnitude { get; set; }
		public string? Place { get; set; }
		public DateTime Time { get; set; }
		public string? Url { get; set; }
		public double Longitude { get; set; }
		public double Latitude { get; set; }
		public double Depth { get; set; }
	}
}