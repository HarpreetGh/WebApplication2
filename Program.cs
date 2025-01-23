using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

Blog defaultBlog = new Blog("First Blog", "I am happy!");
List<Blog> blogs= [defaultBlog];

app.Use(async (context, next) => {
    var startTime = DateTime.Now;
    await next.Invoke();
    Console.WriteLine($"Path Called: {context.Request.Path} in Duration: {DateTime.Now - startTime}");
});

app.UseWhen(context => context.Request.Path == "/reset",
    app => app.Use(async (context, next) => {
    if (context.Request.Headers["pass"] == "P@ssword!") {
        await next.Invoke();
    }
    else {
        context.Response.StatusCode = 403;
        await context.Response.WriteAsync("Access Denied");
        Console.WriteLine("Access Denied");
    }
}));

app.MapGet("/", () => "Hello World!");
app.MapGet("/blogs", () => blogs);

app.MapPost("/newBlog/{title:minlength(3)}", (string title, string? body) => { blogs.Add(new Blog(title, body)); return Results.Ok(blogs.Last()); });

app.MapPost("/newBlog/", (Blog b) => { blogs.Add(b); return Results.Ok(b); });

app.MapPost("/newBlogHash/", (string bStr) => {
    // not tested but idea
    string hash = Convert.ToBase64String(Encoding.UTF8.GetBytes(bStr));
    if(hash != hash) {
        return Results.Problem("Hash doesn't match");
    }
    Blog b = JsonSerializer.Deserialize<Blog>(bStr);
    blogs.Add(b);
    return Results.Ok(b);
});

app.MapPost("/reset", () => { blogs.Clear(); blogs = [defaultBlog]; return Results.Accepted("RESET!"); });

app.Run();

public class Blog(string title, string body)
{
    public string Title { get; set; } = title;
    public string Body { get; set; } = body;
}