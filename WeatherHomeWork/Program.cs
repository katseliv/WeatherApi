namespace DefaultNamespace
{
    public class Program
    {
        public static async Task<int> Main(string[] argv)
        {
            const string lat = "33.20";
            const string lon = "-80.563";
            const string apiKey = "7b6be55ecfc023f52792505653e8e278";

            #region 1
            // https://api.openweathermap.org/data/2.5/onecall?lat={lat}&lon={lon}&exclude={part}&appid={API key}
            var builder = new UriBuilder("https://api.openweathermap.org/data/2.5/onecall");
            var queryParameters = HttpUtility.ParseQueryString(builder.Query);
            queryParameters["lat"] = lat;
            queryParameters["lon"] = lat;
            queryParameters["appid"] = apiKey;
            builder.Query = queryParameters.ToString();
            
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
                .GetProperty("dt")
                .GetInt32();

            // DateTimeOffset offsetUtc = DateTimeOffset.FromUnixTimeSeconds(dateUnixTimeSeconds);
            // DateTimeOffset offsetLocal = offsetUtc.ToLocalTime();
            //
            Console.WriteLine("Temp in celsius: {0}, date: {1}", KelvinToCelsius(kelvinDegrees), 1);

            return 0;
        }

        private static double KelvinToCelsius(double kelvinDegrees)
        {
            return kelvinDegrees - 273.15;
        }
    }
}