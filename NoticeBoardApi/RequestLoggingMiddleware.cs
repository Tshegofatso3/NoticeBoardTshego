namespace NoticeBoardApi.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(
            RequestDelegate next,
            ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var method = context.Request.Method;
            var path = context.Request.Path;
            var start = DateTime.UtcNow;

            _logger.LogInformation("[REQUEST]  {Method} {Path} → started at {Time}",
                method, path, start.ToString("HH:mm:ss"));

            await _next(context);   // pass to next middleware

            var duration = (DateTime.UtcNow - start).TotalMilliseconds;
            var status = context.Response.StatusCode;

            _logger.LogInformation("[RESPONSE] {Method} {Path} → {Status} in {Duration}ms",
                method, path, status, duration);
        }
    }
}

