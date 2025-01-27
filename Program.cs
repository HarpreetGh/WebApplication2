using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var secretKey = "e6b99e83db6f884a225874f1201cb7eeaa70c58fa12d3b4a23642657dcec72a6d8b52c7e009ea3def1aeaca86464f6c4dafdaf5d19d19e835c2cb8de7f78201eb99b5bb08c433fce32632e1f71fb2363da09fdac7bfa74e94527777d534d1716453fc3a365014773b9ffb7cf540603708db9a7aad5160de63a4081ab4aa81c23";

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "yourIssuer",
            ValidAudience = "yourAudience",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
        };
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "My API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"token"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "Bearer",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new string[] { }
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
    // app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API v1"));
}

app.UseHttpsRedirection();
app.UseRouting();

app.Use(async (context, next) => {
    try {
        await next.Invoke();
    }
    catch (Exception ex) {
        context.Response.StatusCode = 500;
        Console.WriteLine($"Exception: {ex.Message}");
    }
    finally {
        RequestLogger(context);
        if (500 <= context.Response.StatusCode) {
            await context.Response.WriteAsync("Internal Server Error");
        }
        ResponseLogger(context);
    }
});

static void RequestLogger(HttpContext context) =>
    Console.WriteLine($"{DateTime.UtcNow}, {context.Request.Method}, {context.Request.Path} {context.Request.QueryString}");
static void ResponseLogger(HttpContext context) =>
    Console.WriteLine($"{DateTime.UtcNow}, {context.Response.StatusCode}, {context.Request.Method}, {context.Request.Path}");

app.UseAuthentication();
app.UseAuthorization();

app.Use(async (context, next) => {
    if (context.Request.Method == "POST" || context.Request.Method == "PUT")
    {
        if (!context.Request.HasJsonContentType())
        {
            context.Response.StatusCode = 415;
            await context.Response.WriteAsync("Unsupported Media Type");
            return;
        }
    }
    await next.Invoke();
});

app.MapControllers();

var users = new Dictionary<int, User>();

app.MapGet("/", () =>
{
    return TypedResults.Ok("Hello World!");
});

app.MapGet("/users", () =>
{
    return TypedResults.Ok(users.Values);
}).RequireAuthorization();

app.MapGet("/users/{id}", Results<Ok<User>, NotFound>(int id) =>
{
    if (users.TryGetValue(id, out var user))
    {
        return TypedResults.Ok(user);
    }
    return TypedResults.NotFound();
}).RequireAuthorization();

app.MapPost("/users", Results<Conflict<string>, Created<User>>(User user) =>
{
    if (users.ContainsKey(user.Id))
    {
        return TypedResults.Conflict("User with this ID already exists.");
    }
    users[user.Id] = user;
    return TypedResults.Created($"/users/{user.Id}", user);
});//.RequireAuthorization();

app.MapPut("/users/{id}", Results<Ok<User>, NotFound>(int id, User updatedUser) =>
{
    if (users.ContainsKey(id))
    {
        users[id] = updatedUser;
        return TypedResults.Ok(updatedUser);
    }
    return TypedResults.NotFound();
}).RequireAuthorization();

app.MapDelete("/users/{id}", Results<NoContent, NotFound>(int id) =>
{
    if (users.Remove(id))
    {
        return TypedResults.NoContent();
    }
    return TypedResults.NotFound();
}).RequireAuthorization();

app.MapPost("/token", Results<BadRequest<string>, Ok<JwtPayload>>(User user) =>
{
    if (user == null || string.IsNullOrEmpty(user.Name) || string.IsNullOrEmpty(user.Email))
    {
        return TypedResults.BadRequest("Invalid user data.");
    }

    var claims = new[]
    {
        new Claim(JwtRegisteredClaimNames.Sub, user.Name),
        new Claim(JwtRegisteredClaimNames.Email, user.Email),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };

    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    var token = new JwtSecurityToken(
        issuer: "yourIssuer",
        audience: "yourAudience",
        claims: claims,
        expires: DateTime.Now.AddMinutes(30),
        signingCredentials: creds);

    return TypedResults.Ok(new JwtPayload
    {
        { "token", new JwtSecurityTokenHandler().WriteToken(token) },
        { "expiration", token.ValidTo }
    });
});

app.Run();

public class User
{
    public int Id { get; set; }
    required public string Name { get; set; }
    required public string Email { get; set; }
    public int Age { get; set; }
}