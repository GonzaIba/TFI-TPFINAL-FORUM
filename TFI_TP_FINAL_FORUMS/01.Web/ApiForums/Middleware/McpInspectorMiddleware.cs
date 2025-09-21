using ModelContextProtocol;

namespace ApiForums.Middleware
{
    public class McpInspectorMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public McpInspectorMiddleware(RequestDelegate next, ILogger<RequestMiddleware> logger, IHostEnvironment env)
        {
            _env = env;
            _logger = logger;
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            var endpoint = context.GetEndpoint();
            var isMcpRequest = endpoint?.DisplayName?.Contains("MCP") == true;

            if (isMcpRequest)
            {
                await _next(context);
                return;
            }

            await _next(context);
        }
    }
}
