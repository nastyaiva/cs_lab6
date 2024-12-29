using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Linq;
using System.Threading.Tasks;
public struct Weather
{
    public string Country { get; set; }
    public string Name { get; set; }
    public double Temp { get; set; }
    public string Description { get; set; }
}
public class WeatherResponse
{
    public Coord coord { get; set; }
    public List<WeatherInfo> weather { get; set; }
    public string @base { get; set; }
    public MainInfo main { get; set; }
    public int visibility { get; set; }
    public Wind wind { get; set; }
    public Clouds clouds { get; set; }
    public int dt { get; set; }
    public Sys sys { get; set; }
    public int timezone { get; set; }
    public int id { get; set; }
    public string name { get; set; }
    public int cod { get; set; }
}
public class Coord
{
    public double lon { get; set; }
    public double lat { get; set; }
}
public class WeatherInfo
{
    public int id { get; set; }
    public string main { get; set; }
    public string description { get; set; }
    public string icon { get; set; }
}
public class MainInfo
{
    public double temp { get; set; }
    public double feels_like { get; set; }
    public double temp_min { get; set; }
    public double temp_max { get; set; }
    public int pressure { get; set; }
    public int humidity { get; set; }
}
public class Wind
{
    public double speed { get; set; }
    public int deg { get; set; }
}
public class Clouds
{
    public int all { get; set; }
}
public class Sys
{
    public int type { get; set; }
    public int id { get; set; }
    public string country { get; set; }
    public int sunrise { get; set; }
    public int sunset { get; set; }
}
class Program
{
    private static readonly HttpClient client = new HttpClient();
    private const string ApiKey = "5e9b5527b130ec4b5440a7ed79989b42";
    static async Task Main(string[] args)
    {
        List<Weather> weatherData = new List<Weather>();
        Random random = new Random();
        while (weatherData.Count < 50)
        {
            double lat = random.NextDouble() * 180 - 90;
            double lon = random.NextDouble() * 360 - 180;
            string url = $"https://api.openweathermap.org/data/2.5/weather?lat={lat}&lon={lon}&appid={ApiKey}&units=metric";
            HttpResponseMessage response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                string jsonResponse = await response.Content.ReadAsStringAsync();
                var weatherInfo = JsonSerializer.Deserialize<WeatherResponse>(jsonResponse);
                if (weatherInfo.sys.country != null && weatherInfo.name != null)
                {
                    weatherData.Add(new Weather
                    {
                        Country = weatherInfo.sys.country,
                        Name = weatherInfo.name,
                        Temp = weatherInfo.main.temp,
                        Description = weatherInfo.weather[0].description
                    });
                }
            }
        }
        var maxTempCountry = weatherData.OrderByDescending(w => w.Temp).First();
        var minTempCountry = weatherData.OrderBy(w => w.Temp).First();
        var averageTemp = weatherData.Average(w => w.Temp);
        var countryCount = weatherData.Select(w => w.Country).Distinct().Count();
        var specificDescriptions = weatherData.FirstOrDefault(w => new[] { "clear sky", "rain", "few clouds" }.Contains(w.Description));
        Console.WriteLine($"Страна с максимальной температурой: {maxTempCountry.Country}, {maxTempCountry.Temp}°C");
        Console.WriteLine($"Страна с минимальной температурой: {minTempCountry.Country}, {minTempCountry.Temp}°C");
        Console.WriteLine($"Средняя температура в мире: {averageTemp}°C");
        Console.WriteLine($"Количество стран в коллекции: {countryCount}");
        if (!specificDescriptions.Equals(default(Weather)))
        {
            Console.WriteLine($"Первая найденная страна и название местности с описанием '{specificDescriptions.Description}': {specificDescriptions.Country}, {specificDescriptions.Name}");
        }
    }
}