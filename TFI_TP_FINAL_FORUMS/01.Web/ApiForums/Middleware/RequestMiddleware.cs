using Core.Domain.Enum;
using Core.Domain.Exceptions.BaseException;
using Core.Domain.Response.BaseResponse;
using Newtonsoft.Json;
using System.Net;
using System.Text.Json;

namespace ApiForums.Middleware
{
    public class RequestMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public RequestMiddleware(RequestDelegate next, ILogger<RequestMiddleware> logger, IHostEnvironment env)
        {
            _env = env;
            _logger = logger;
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                var originalBodyStream = context.Response.Body;
                using (var responseBody = new MemoryStream())
                {
                    context.Response.Body = responseBody;

                    try
                    {
                        await _next(context);
                        responseBody.Seek(0, SeekOrigin.Begin);

                        // Analizar el código de estado HTTP y establecer ResponseMessage en consecuencia
                        var responseCode = context.Response.StatusCode switch
                        {
                            200 => ResponseCodeEnum.Success.ToString(),
                            400 => ResponseCodeEnum.NotProcessed.ToString(),
                            404 => ResponseCodeEnum.NotAllowed.ToString(),
                            // Otros códigos de estado según sea necesario
                            _ => ResponseCodeEnum.Unknown.ToString(),
                        };

                        var serializer = new Newtonsoft.Json.JsonSerializer();
                        await using (var writer = new StreamWriter(originalBodyStream))
                        await using (var jsonWriter = new JsonTextWriter(writer))
                        {
                            var apiResponse = new GenericApiResponse<object>
                            {
                                Meta = 
                                {
                                    Service = context.Request.Path,
                                    ResponseCode = responseCode,
                                    ResponseMessage = "asd",
                                },
                                Data = JsonConvert.DeserializeObject<object>(await new StreamReader(responseBody).ReadToEndAsync())
                            };

                            serializer.Serialize(jsonWriter, apiResponse);
                        }
                    }
                    catch (Exception ex)
                    {
                        // Manejo de excepciones si es necesario
                    }
                    finally
                    {
                        context.Response.Body = originalBodyStream;
                    }
                }

            }
            catch(ExceptionBase ex)
            {
                //EN GATEWAY VALIDAR SI EXISTE EL EXCEPTION PERSONALIZADO
                _logger.LogError(ex, ex.Message);
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                GenericApiResponse<string> genericApiResponse = new GenericApiResponse<string>()
                {
                    Meta =
                    {
                        Method = context.Request.Method,
                        Service = context.Request.Path.Value,
                        ResponseCode = ResponseCodeEnum.NoHandleException.ToString()
                    },
                    Data = null,

                };

                if (_env.IsDevelopment())
                    genericApiResponse.Meta.ResponseMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                else
                    genericApiResponse.Meta.ResponseMessage = "Internal Server Error";

                var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
                var json = System.Text.Json.JsonSerializer.Serialize(genericApiResponse, options);
                await context.Response.WriteAsync(json);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                GenericApiResponse<string> genericApiResponse = new GenericApiResponse<string>()
                {
                    Meta = 
                    {
                        Method = context.Request.Method,
                        Service = context.Request.Path.Value,
                        ResponseCode = ResponseCodeEnum.NoHandleException.ToString()
                    },
                    Data = null,

                };
                
                if(_env.IsDevelopment())
                    genericApiResponse.Meta.ResponseMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                else
                    genericApiResponse.Meta.ResponseMessage = "Internal Server Error";

                var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
                var json = System.Text.Json.JsonSerializer.Serialize(genericApiResponse, options);
                await context.Response.WriteAsync(json);
            }
        }
    }
}
