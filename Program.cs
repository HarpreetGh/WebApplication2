var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "GET!");
app.MapPut("/", () => "PUT!");

app.Run();
