using Core.Domain.Enum;
using Core.Domain.Exceptions;
using CrossCutting.EmailService;
using CrossCutting.Helpers.ResponseClasses;
using System.Net;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ApiForums.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
        {
            _env = env;
            _logger = logger;
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                GenericApiResponse<string> genericApiResponse = new GenericApiResponse<string>()
                {
                    Service = context.Request.Path.Value,
                    Data = null,
                    ResponseCode = ResponseCodeEnum.NoHandleException.ToString()
                };
                
                if(_env.IsDevelopment())
                    genericApiResponse.ResponseMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                else
                    genericApiResponse.ResponseMessage = "Internal Server Error";

                var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

                var json = JsonSerializer.Serialize(genericApiResponse, options);

                await context.Response.WriteAsync(json);
            }
        }
    }
}
