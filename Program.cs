using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
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
    public int Age { get; set; }
}