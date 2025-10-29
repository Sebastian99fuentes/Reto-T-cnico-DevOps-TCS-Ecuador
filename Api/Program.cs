using Api.Endpoints;
using Api.TokenService;
using AuthExtensions;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "AllowPostman",
        policy =>
        {
            policy.WithOrigins("*") // 🔁 Cambia al puerto del frontend
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});



//TODO
//Problema actual: Llave hardcodeada en appsettings.json
//Solución: Usar Azure Key Vault o Kubernetes Secrets
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = jwtSettings["Key"]
    ?? throw new InvalidOperationException("Jwt:Key no está configurado");
var issuer = jwtSettings["Issuer"]
    ?? throw new InvalidOperationException("Jwt:Issuer no está configurado");
var audience = jwtSettings["Audience"]
    ?? throw new InvalidOperationException("Jwt:Audience no está configurado");
// Autorización
builder.Services.AddJwtAuthentication(
   issuer,
    audience,
    key
);

//  TOKEN SERVICE
builder.Services.AddScoped<JwtTokenService>();

builder.Services.AddAuthorization();


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowPostman");

app.UseAuthentication(); // JWT
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


app.UseAuthorization();


app.MapMsjEndPoints();

app.MapControllers();



app.Run();



// Necesario para los tests
public partial class Program { }
