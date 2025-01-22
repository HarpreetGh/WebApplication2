using System.Text.Json;
using System.Xml.Serialization;

var builder = WebApplication.CreateBuilder(args);
builder.Services.ConfigureHttpJsonOptions(option => {
    option.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});

var app = builder.Build();

var samplePerson = new Person { UserName = "Alice", UserAge= 20 };

app.Use(async (context, next) => {
    await next.Invoke();
});

app.MapGet("/", () => "HELLO!");
app.MapGet("/manual-json", () => {
    string jsonStr = JsonSerializer.Serialize(samplePerson);
    return TypedResults.Text(jsonStr, "application/json");
});

app.MapGet("/custom-serializer", () => {
    var options = new JsonSerializerOptions {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseUpper
    };
    string customJsonStr = JsonSerializer.Serialize(samplePerson, options);
    return TypedResults.Text(customJsonStr, "application/json");
});

app.MapGet("/json", () => TypedResults.Json(samplePerson));

app.MapGet("/auto", () => samplePerson);

app.MapGet("/xml", () => {
    var xmlSerializer = new XmlSerializer(typeof(Person));
    var strinWriter = new StringWriter();
    xmlSerializer.Serialize(strinWriter, samplePerson);
    var xmlOutput = strinWriter.ToString();
    return TypedResults.Text(xmlOutput, "application/xml");
});


app.Run();

public class Person {
    required public string UserName { get; set; }
    required public int UserAge { get; set; }
}