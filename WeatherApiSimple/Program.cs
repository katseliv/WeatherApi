using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace WeatherApiSimple
{
	public class Program
	{
		public static async Task<int> Main(string[] argv)
		{
			const string city = "Voronezh";
			const string apiKey = "7b6be55ecfc023f52792505653e8e278";
			const string template = "https://api.openweathermap.org/data/2.5/weather?q={0}&appid={1}";

			var client = new HttpClient();
			string url = string.Format(template, city, apiKey);
			HttpResponseMessage result = await client.GetAsync(url);

			Console.WriteLine(await result.Content.ReadAsStringAsync());

			return 0;
		}
	}
}