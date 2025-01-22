var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<ITest, MyServiceClass>();

var app = builder.Build();

app.Use(async (context, next) => {
    var myService = context.RequestServices.GetService<ITest>();
    myService?.LogCreation("MiddleWear");
    await next.Invoke();
});

app.MapGet("/", () => "HELLO!");
app.MapGet("/test", (ITest ct) => { ct.LogCreation("Test"); return "Test"; });
app.Run();

public interface ITest {
    public void LogCreation(string message);
}

public class MyServiceClass: ITest {
    private readonly int _serviceId;

    public MyServiceClass() {
        _serviceId = new Random().Next(100, 999);
    }

    public void LogCreation(string message = "") {
        Console.WriteLine($"SERVER CREATED. serviceID: {_serviceId} {(message.Length > 0? $"Message: {message}" : "")}");
    }
}