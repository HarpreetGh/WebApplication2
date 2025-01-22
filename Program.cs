using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.Use(async (context, next) => {
    await next.Invoke();
});

app.MapGet("/", () => "HELLO!");

app.MapPost("/auto", (Person p) => p);
app.MapPost("/json", async (HttpContext content) => {
    Person? p = await content.Request.ReadFromJsonAsync<Person>();
    if (p != null) { p.UserAge *= 2; }
    return p;
});

app.MapPost("/custom-options", async (HttpContext content) => {
    var options = new JsonSerializerOptions {
        UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow
    };

    Person? p = await content.Request.ReadFromJsonAsync<Person>(options);
    if (p != null) { p.UserAge *= 2; }
    return p;
});

app.MapPost("/xml", async (HttpContext context) => {
    var sr = new StreamReader(context.Request.Body);
    var body = await sr.ReadToEndAsync();
    // var body2 = context.Request.Body.ReadAsync();

    var xmlSeralizer = new XmlSerializer(typeof(Person));
    var stringReader = new StringReader(body);
    var p = xmlSeralizer.Deserialize(stringReader) as Person;
});

app.Run();

public class Person {
    required public string UserName { get; set; }
    required public int UserAge { get; set; }
}