using System;
using System.Globalization;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

namespace WeatherApiUsingQuery
{
	public class Program
	{
		public static async Task<int> Main(string[] argv)
		{
			
			double latitude = 33.20;
			double longitude = -80.563;
			string city = "Voronezh";
			const string apiKey = "7b6be55ecfc023f52792505653e8e278";

			Console.Write("Введите широту: ");
			latitude = Convert.ToDouble(Console.ReadLine());
			Console.Write("Введите долготу: ");
			longitude = Convert.ToDouble(Console.ReadLine());
			await PrintCurrentTemperatureAndDateByCoordinate(latitude, longitude, apiKey);
			
			Console.Write("Введите город: ");
			city = Console.ReadLine();
			Console.WriteLine("City: {0} \n", city);
			await PrintCurrentTemperatureAndDateByCity(city, apiKey);

			return 0;
		}

		private static async Task<int> PrintCurrentTemperatureAndDate(string apiKey)
		{
			string city = "Voronezh";
			#region 1
			var builder = new UriBuilder("https://api.openweathermap.org/data/2.5/weather");
			var queryParameters = HttpUtility.ParseQueryString(builder.Query);
			queryParameters["q"] = city;
			queryParameters["appid"] = apiKey;
			builder.Query = queryParameters.ToString()!;
			Uri uri = builder.Uri;
			var request = new HttpRequestMessage(HttpMethod.Get, uri);
			var client = new HttpClient();
			HttpResponseMessage result = await client.SendAsync(request);
			result.EnsureSuccessStatusCode();
			#endregion
			
			string jsonContent = await result.Content.ReadAsStringAsync();
			JsonDocument jsonDocument = JsonDocument.Parse(jsonContent);
			double kelvinDegrees = jsonDocument.RootElement
				.GetProperty("main")
				.GetProperty("temp")
				.GetDouble();
			
			int dateUnixTimeSeconds = jsonDocument.RootElement
				.GetProperty("dt")
				.GetInt32();
			
			DateTimeOffset offsetUtc = DateTimeOffset.FromUnixTimeSeconds(dateUnixTimeSeconds);
			DateTimeOffset offsetLocal = offsetUtc.ToLocalTime();
			
			Console.WriteLine("Temp in celsius: {0}, Date: {1}", KelvinToCelsius(kelvinDegrees), offsetLocal);

			return 0;
		}
		
		//Home Work by Ekaterina Selivanova (Modification 2)
		private static async Task<int> PrintCurrentTemperatureAndDateByCity(string city, string apiKey)
		{
			var builder = new UriBuilder("https://api.openweathermap.org/data/2.5/weather");
			var queryParameters = HttpUtility.ParseQueryString(builder.Query);
			queryParameters["q"] = city;
			queryParameters["appid"] = apiKey;
			builder.Query = queryParameters.ToString()!;
			Uri uri = builder.Uri;
			
			var request = new HttpRequestMessage(HttpMethod.Get, uri);
			var client = new HttpClient();
			HttpResponseMessage result = await client.SendAsync(request);
			result.EnsureSuccessStatusCode();
			
			string jsonContent = await result.Content.ReadAsStringAsync();
			JsonDocument jsonDocument = JsonDocument.Parse(jsonContent);
			double lat = jsonDocument.RootElement
				.GetProperty("coord")
				.GetProperty("lat")
				.GetDouble();
			double lon = jsonDocument.RootElement
				.GetProperty("coord")
				.GetProperty("lon")
				.GetDouble();

			await PrintCurrentTemperatureAndDateByCoordinate(lat, lon, apiKey);
			
			return 0;
		}
		
		//Home Work by Ekaterina Selivanova (Modification 1)
		private static async Task<int> PrintCurrentTemperatureAndDateByCoordinate(double lat, double lon, string apiKey)
		{
			#region 1
			// https://api.openweathermap.org/data/2.5/onecall?lat={lat}&lon={lon}&exclude={part}&appid={API key}
			var builder = new UriBuilder("https://api.openweathermap.org/data/2.5/onecall");
			var queryParameters = HttpUtility.ParseQueryString(builder.Query);
			queryParameters["lat"] = Convert.ToString(lat, CultureInfo.CurrentCulture);
			queryParameters["lon"] = Convert.ToString(lon, CultureInfo.CurrentCulture);
			queryParameters["appid"] = apiKey;
			builder.Query = queryParameters.ToString()!;
            
			Uri uri = builder.Uri;
			var request = new HttpRequestMessage(HttpMethod.Get, uri);
			var client = new HttpClient();
			HttpResponseMessage result = await client.SendAsync(request);
			result.EnsureSuccessStatusCode();
			#endregion

			string jsonContent = await result.Content.ReadAsStringAsync();
			JsonDocument jsonDocument = JsonDocument.Parse(jsonContent);
			double kelvinDegrees = jsonDocument.RootElement
				.GetProperty("current")
				.GetProperty("temp")
				.GetDouble();

			int dateUnixTimeSeconds = jsonDocument.RootElement
				.GetProperty("current")
				.GetProperty("dt")
				.GetInt32();

			DateTimeOffset offsetUtc = DateTimeOffset.FromUnixTimeSeconds(dateUnixTimeSeconds);
			DateTimeOffset offsetLocal = offsetUtc.ToLocalTime();
			
			Console.WriteLine("Latitude: {0}, Longitude: {1}, Temp in celsius: {2}, Date: {3}", 
				               lat, lon, KelvinToCelsius(kelvinDegrees).ToString("0,0", CultureInfo.InvariantCulture), 
				               offsetLocal);

			return 0;
		}

		private static double KelvinToCelsius(double kelvinDegrees)
		{
			return kelvinDegrees - 273.15;
		}
	}
}