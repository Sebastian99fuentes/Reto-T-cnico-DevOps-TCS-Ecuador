namespace OcelotGateway.Middleware
{
    public class CorrelationIDMidleware
    {
        private const string CorrelationIdHeader = "X-Correlation-ID";
        private readonly RequestDelegate _next;
        private readonly ILogger<CorrelationIDMidleware> _logger;

        public CorrelationIDMidleware(RequestDelegate next, ILogger<CorrelationIDMidleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            // Generate if not present
            if (!context.Request.Headers.TryGetValue(CorrelationIdHeader, out var correlationId))
            {
                correlationId = Guid.NewGuid().ToString();
                context.Request.Headers[CorrelationIdHeader] = correlationId;
            }

            context.Response.Headers[CorrelationIdHeader] = correlationId;

            // Log for visibility - SIN Serilog
            _logger.LogInformation("Correlation Id set: {CorrelationId}", correlationId);

            await _next(context);
        }
    }
}