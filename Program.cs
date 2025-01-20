
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var blogs = new List<Blog>
{
    new Blog { Title = "A", Body = "Message A" },
    new Blog { Title = "B", Body = "Message B" }
};

app.MapGet("/", () => "HELLO!");
app.MapGet("/blogs", () => blogs);
app.MapGet("/blogs/{id:int:min(0)}", (int id) => blogs != null && id < blogs.Count? Results.Ok(blogs[id]) : Results.NotFound());
app.MapPost("/blogs", (Blog blog) => {
    blogs.Add(blog);
    return Results.Created($"/blogs/{blogs.Count - 1}", blog);
});
app.MapDelete("/blogs/{id:int:min(0)}", (int id)=> {
    if (id < blogs.Count) {
        blogs.Remove(blogs[id]);
        Results.NoContent();
    }
    else {
        Results.NotFound();
    }
});
app.MapPut("/blogs/{id:int:min(0)}", (int id, Blog blog)=> {
    if (id < blogs.Count) {
        blogs[id] = blog;
        Results.Ok(blog);
    }
    else {
        Results.NotFound();
    }
});
app.Run();

public class Blog {
    public required string Title {get; set;}
    public required string Body {get; set;}
}