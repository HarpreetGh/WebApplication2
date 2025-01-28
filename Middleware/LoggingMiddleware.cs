namespace Middleware
{
    public class LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
    {
        private readonly RequestDelegate _next = next;
        private readonly ILogger<LoggingMiddleware> _logger = logger;

        public async Task InvokeAsync(HttpContext context)
        {
            try {
                RequestLogger(context);
                await _next(context);
            }
            catch (Exception ex) {
                context.Response.StatusCode = 500;
                _logger.LogError($"Exception: {ex.Message}");
            }
            finally {
                LogLevel logLevel;
                switch (context.Response.StatusCode) {
                    case >= 500:
                        logLevel = LogLevel.Error;
                        await context.Response.WriteAsync("Internal Server Error");
                        break;
                    case >= 400:
                        logLevel = LogLevel.Warning;
                        break;
                    default:
                        logLevel = LogLevel.Information;
                        break;
                }

                ResponseLogger(context, logLevel);
            }
        }

        private void RequestLogger(HttpContext context) =>
            _logger.LogInformation($"{DateTime.UtcNow}, {context.Request.Method}, {context.Request.Path} {context.Request.QueryString}");
        
        private void ResponseLogger(HttpContext context, LogLevel level = LogLevel.Information) =>
            _logger.Log(level, $"{DateTime.UtcNow}, {context.Response.StatusCode}, {context.Request.Method}, {context.Request.Path}");
    }
}