namespace Middleware
{
    public class ContentValidationMiddleware(RequestDelegate next)
    {
        private readonly RequestDelegate _next = next;

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Method == "POST" || context.Request.Method == "PUT")
            {
                if (!context.Request.HasJsonContentType())
                {
                    context.Response.StatusCode = 415;
                    await context.Response.WriteAsync("Unsupported Media Type");
                    return;
                }
            }
            await _next(context);            
        }
    }
}