using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace AuthExtensions
{
    public static class AuthExtensions
    {
        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, string issuer, string audience, string key)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
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
                        ClockSkew = TimeSpan.Zero // o 0
                    };

                    
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            // Opción 1: Header estándar
                            if (context.Request.Headers.ContainsKey("Authorization"))
                            {
                                var authHeader = context.Request.Headers["Authorization"].ToString();
                                if (authHeader.StartsWith("Bearer "))
                                {
                                    context.Token = authHeader.Substring("Bearer ".Length).Trim();
                                    return Task.CompletedTask;
                                }
                            }

                           
                            if (context.Request.Headers.ContainsKey("X-JWT-KWY"))
                            {
                                context.Token = context.Request.Headers["X-JWT-KWY"].ToString();
                                return Task.CompletedTask;
                            }

                            return Task.CompletedTask;
                        }
                    };

                });

            return services;
        }
    }
}
