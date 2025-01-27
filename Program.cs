using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "My API", Version = "v1" });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API v1"));
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.Use(async (context, next) => {
    try {
        await next.Invoke();
    }
    catch (Exception ex) {
        context.Response.StatusCode = 500;
        Console.WriteLine($"Exception: {ex.Message}");
    }
    finally {
        if (400 <= context.Response.StatusCode) {
            if (500 <= context.Response.StatusCode) {
                await context.Response.WriteAsync("Internal Server Error");
            }
            Logger(context);
        }
    }
});

static void Logger(HttpContext context) => Console.WriteLine($"{DateTime.UtcNow}, {context.Response.StatusCode}, {context.Request.Method}, {context.Request.Path}");

app.Use(async (context, next) => {
    if (context.Request.Method == "POST" || context.Request.Method == "PUT")
    {
        if (!context.Request.HasJsonContentType())
        {
            context.Response.StatusCode = 415;
            await context.Response.WriteAsync("Unsupported Media Type");
            return;
        }
        else {
            var user = await context.Request.ReadFromJsonAsync<User>();
            if (user == null || user.Id == 0 || string.IsNullOrEmpty(user.Name) || string.IsNullOrEmpty(user.Email))
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync("Bad Request");
                return;
            }
        }

    }
    await next.Invoke();
});

// app.Use(async (context, next) => {
//     if (context.Request.Headers["knownUser"] == "true") {
//         await next.Invoke();
//     }
//     else {
//         context.Response.StatusCode = 403;
//         await context.Response.WriteAsync("Forbidden");
//     }
// });

app.MapControllers();

var users = new Dictionary<int, User>();

app.MapGet("/", () =>
{
    return TypedResults.Ok("Hello World!");
});

app.MapGet("/users", () =>
{
    return TypedResults.Ok(users.Values);
});

app.MapGet("/users/{id}", Results<Ok<User>, NotFound>(int id) =>
{
    if (users.TryGetValue(id, out var user))
    {
        return TypedResults.Ok(user);
    }
    return TypedResults.NotFound();
});

app.MapPost("/users", Results<Conflict<string>, Created<User>>(User user) =>
{
    if (users.ContainsKey(user.Id))
    {
        return TypedResults.Conflict("User with this ID already exists.");
    }
    users[user.Id] = user;
    return TypedResults.Created($"/users/{user.Id}", user);
});

app.MapPut("/users/{id}", Results<Ok<User>, NotFound>(int id, User updatedUser) =>
{
    if (users.ContainsKey(id))
    {
        users[id] = updatedUser;
        return TypedResults.Ok(updatedUser);
    }
    return TypedResults.NotFound();
});

app.MapDelete("/users/{id}", Results<NoContent, NotFound>(int id) =>
{
    if (users.Remove(id))
    {
        return TypedResults.NoContent();
    }
    return TypedResults.NotFound();
});

app.Run();

public class User
{
    public int Id { get; set; }
    required public string Name { get; set; }
    required public string Email { get; set; }
    public int Age { get; set; }
}