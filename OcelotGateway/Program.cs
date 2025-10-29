
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using OcelotGateway.Middleware;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = jwtSettings["Key"];
var issuer = jwtSettings["Issuer"];
var audience = jwtSettings["Audience"];

builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

//Add JWT auth to ocelot
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("JwtKey", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            ClockSkew = TimeSpan.Zero 
        };
    });
builder.Services.AddAuthorization();
builder.Services.AddOcelot();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi



builder.Services.AddSwaggerGen();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

}



app.Use(async (context, next) =>
{
    // Si existe X-JWT-KWY, lo convertimos a Authorization: Bearer
    if (context.Request.Headers.TryGetValue("X-JWT-KWY", out var jwtValue))
    {
        var token = jwtValue.ToString();
if (!string.IsNullOrEmpty(token) && !token.StartsWith("Bearer "))
{
    context.Request.Headers["Authorization"] = $"Bearer {token}";
}
    }
    
    await next();
});

//  MIDDLEWARE PARA VALIDAR X-Parse-REST-API-Key
app.Use(async (context, next) =>
{
    if (context.Request.Path.StartsWithSegments("/DevOps"))
    {
        if (!context.Request.Headers.ContainsKey("X-Parse-REST-API-Key") ||
            context.Request.Headers["X-Parse-REST-API-Key"] != "2f5ae96c-b558-4c7b-a590-a501ae1c3f6c")
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Invalid API Key");
            return;
        }
    }

    await next();
});


app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<CorrelationIDMidleware>();

await app.UseOcelot();

app.Run();
