using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Middleware;
using Models;
using Services;
using System.IdentityModel.Tokens.Jwt;
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

builder.Services.AddSingleton<ITokenInterface, TokenService>();
builder.Services.AddSingleton<IUserInterface, UserService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseMiddleware<LoggingMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<ContentValidationMiddleware>();

app.MapControllers();

app.MapGet("/", () => TypedResults.Ok("Hello World!"));

app.MapPost("/token", Results<BadRequest<string>, Ok<JwtPayload>>(User user, ITokenInterface tokenService) =>
{
    if (user == null || string.IsNullOrEmpty(user.Name) || string.IsNullOrEmpty(user.Email))
    {
        return TypedResults.BadRequest("Invalid user data.");
    }

    return TypedResults.Ok(tokenService.GenerateToken(user));
});

app.Run();