using System;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;
using System.Web;

namespace WeatherApiUsingDto
{
	public class Program
	{
		public static async Task<int> Main(string[] argv)
		{
			const string defaultCity = "Voronezh";
			const string apiKey = "7b6be55ecfc023f52792505653e8e278";

			string city;
			if (argv.Length == 1)
				city = argv[0];
			else
				city = defaultCity;

			// Create a URI (Uniform Resource Identifier)
			var builder = new UriBuilder("https://api.openweathermap.org/data/2.5/weather");
			var queryParameters = HttpUtility.ParseQueryString(builder.Query);
			queryParameters["q"] = city;
			queryParameters["appid"] = apiKey;
			builder.Query = queryParameters.ToString();

			// Perform a request
			Uri uri = builder.Uri;
			var request = new HttpRequestMessage(HttpMethod.Get, uri);
			var client = new HttpClient();
			HttpResponseMessage result = await client.SendAsync(request);

			string jsonContent = await result.Content.ReadAsStringAsync();
			JsonDocument jsonDocument = JsonDocument.Parse(jsonContent);
			var currentWeatherDto = jsonDocument.Deserialize<Root>();

			DateTimeOffset offsetUtc = DateTimeOffset.FromUnixTimeSeconds(currentWeatherDto.dt);
			DateTimeOffset offsetLocal = offsetUtc.ToLocalTime();

			Console.WriteLine("Temp in celsius: {0}, date: {1}", KelvinToCelsius(currentWeatherDto.main.temp), offsetLocal);

			return 0;
		}

		private static double KelvinToCelsius(double kelvinDegrees)
		{
			return kelvinDegrees - 273.15;
		}
	}
}