
var builder = WebApplication.CreateBuilder(args);
// builder.Services.AddControllers();
var app = builder.Build();

app.MapGet("/", () => "GET!");
// app.MapPut("/", () => "PUT!");
// app.MapGet("/test", () => "TEST GET!");
// app.MapPost("/test", () => "TEST POST!");
// app.MapDelete("/test", () => "TEST DELETE!");
// app.MapControllers();
// app.MapControllerRoute("/weather", "{controller=weatherController}");
// app.MapControllers("WeatherForecast");
app.MapGet("/product/{id:int:min(6)}", (int id) => $"product {id}");
app.MapGet("/user/{id:int}/name/{name:alpha}", (int id, char name) => $"User {id}, Name {name}");
app.MapGet("/report/{year?}", (int year = 2016) => $"Report {year}");
app.MapGet("file/{*path}", (string path) => $"File {path}");
app.MapGet("/search", (string queary, string lang) => $"Search {queary} in language {lang}");

app.MapGet("/stock/{category}/{id:int?}/{*extra}",
    (string category, bool inStock, int? id = 0, string? extra = "") => $"Stock {category}, ID {id}, Extra {extra}, In Stock {inStock}");

app.Run();