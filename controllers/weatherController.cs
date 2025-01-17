// using Microsoft.AspNetCore.Mvc;

// [ApiController]
// [Route("[controller]")]

// class WeatherForcast {
//     public DateTime Date { get; set; }
//     public int TemperatureC { get; set; }
//     public string Summary { get; set; }

//     public WeatherForcast(DateTime date, int temperatureC, string summary) {
//         Date = date;
//         TemperatureC = temperatureC;
//         Summary = summary;
//     }
// }

// class WeatherForecastController : ControllerBase {
//     private List<string> Summaries = new List<string> {
//     "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
// };

//     [HttpGet(Name = "GetWeatherForcast")]
//     public IEnumerable<WeatherForcast> Get() {
//         var rng = new Random();
//         return Enumerable.Range(1, 5).Select(index => new WeatherForcast(
//             DateTime.Now.AddDays(index), rng.Next(-20, 55), Summaries[rng.Next(Summaries.Count)]
//         )).ToArray();
//     }

//     [HttpPost]
//     public IActionResult Post([FromBody] WeatherForcast weatherForcast) {
//         return Ok(weatherForcast);
//     }

//     [HttpPut("{id}")]
//     public  IActionResult Put(int id, [FromBody] WeatherForcast weatherForcast) {
//         return NoContent();
//     }

//     [HttpDelete("{id}")]
//     public IActionResult Delete(int id) {
//         return NoContent();
//     }
// }