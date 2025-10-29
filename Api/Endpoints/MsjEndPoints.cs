using Api.Models;
using Api.TokenService;
using Microsoft.AspNetCore.Mvc;


namespace Api.Endpoints
{
    public static class MsjEndPoints
    {
        public static void MapMsjEndPoints(this IEndpointRouteBuilder app)
        {
            app.MapPost("/DevOps",
                [ApiVersion("1.0")] 
            (MessageRequest request) =>
            {
                return Results.Ok(new MessageResponse { Message = $"Hello {request.To} your message will be send" });
            })
            .WithName("SendMessage")
            .WithTags("Messages")
            .RequireAuthorization();


            var others = new[] { "GET", "PUT", "DELETE", "PATCH", "HEAD", "OPTIONS" };
            app.MapMethods("/DevOps", others, () =>
            {
                return Results.Text("ERROR", "text/plain");
            });
            



            app.MapGet("/health", () =>
            {
                return Results.Ok(new
                {
                    status = "Healthy",
                    timestamp = DateTime.UtcNow,
                    instance = Environment.GetEnvironmentVariable("INSTANCE_NAME")
                });
            })
            .WithName("HealthCheck")
            .WithTags("Monitoring");


            app.MapGet("/DevOpsToken",
            [ApiVersion("1.0")]
            (JwtTokenService TokenService_) =>  
            {
                var token = TokenService_.GenerateToken();
                return Results.Ok(new
                {
                    token = token,
                    message = "Use this token in X-JWT-KWY header"
                });
            })
            .WithName("DevOpsToken")
            .WithTags("DevOpsToken");



        } 
    }
}
