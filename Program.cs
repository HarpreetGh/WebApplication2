
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpLogging((o)=>{});
var app = builder.Build();

app.UseHttpLogging();
app.Use(async (context, next) => {
    Console.WriteLine("Before");
    await next.Invoke();
    Console.WriteLine("After");
});

app.MapGet("/", () => "HELLO!");
app.MapGet("/test", () => "TEST!");
app.Run();