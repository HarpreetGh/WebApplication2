var builder = WebApplication.CreateBuilder();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

builder.Services.AddHttpLogging(options => {
    options.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.All;
});

var app = builder.Build();

app.UseHttpLogging();
app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "api v1"));

app.MapControllers();

app.MapGet("/", () => "Hello World!");

// app.Run();

Task.Run(() => app.RunAsync());

await Task.Delay(3000);
Console.WriteLine("Delay passed");

var GENERATE_FILE = false;
var PORT = 5296; //this shoyuld be in env and is needed only in this app.

if (GENERATE_FILE) {
    ClientGenerator g = new ClientGenerator();
    await g.GenerateClient(PORT);
}
else {
    var httpClient = new HttpClient();
    var client = new CustomNamespace.CustomdApiClient($"http://localhost:{PORT}", httpClient);

    Console.WriteLine("Delay passed");
    var u = await client.GetUserAsync(30);

    Console.WriteLine($"u: {u.Name}");ClientGenerator g = new ClientGenerator();
}
