var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();
builder.Services.AddHttpLogging(logging => {
    logging.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.All;
});

var app = builder.Build();

if (app.Environment.IsDevelopment()) {
    app.UseExceptionHandler("/error");
}
else { app.UseDeveloperExceptionPage(); }

app.UseAuthentication();
app.UseAuthorization();
app.UseHttpLogging();

app.Use(async (context, next) => {
    Console.WriteLine($"Path: {context.Request.Path}");
    await next.Invoke();
    Console.WriteLine($"Response Status Code: {context.Response.StatusCode}");
});

app.Use(async (context, next) => {
    var startTime = DateTime.Now;
    Console.WriteLine($"Start Time: {startTime}");
    await next.Invoke();
    var duration = DateTime.Now - startTime;
    Console.WriteLine($"Response Time: {duration.TotalMilliseconds} ms");
});

app.MapGet("/", () => "Hello World!");
app.MapGet("/test", () => { throw new Exception("Test"); });
app.MapGet("/error", () => "Oopsie an error occured.");


app.Run();
