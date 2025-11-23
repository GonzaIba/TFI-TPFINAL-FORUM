using Core.Domain.Enum;
using Core.Domain.Exceptions.BaseException;
using Core.Domain.Response.BaseResponse;
using Newtonsoft.Json;
using Npgsql;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Text.Json;
using _ = ApiForums.Middleware.Helpers.RequestAnalize;

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
            //⛔ evitá interferir con rutas especiales como / mcp o / mcp / sse
            try
            {
                if (_.IsMcp(context))
                {
                    await _next(context); // dejá que mcp maneje la respuesta completa
                    return;
                }
            }
            catch (Exception ex)
            {

                throw;
            }


            using (var responseBody = new MemoryStream())
            {
                var originalBodyStream = context.Response.Body;
                context.Response.Body = responseBody;
                try
                {
                    if (_.IsControllerPath(context.Request.Path)) // Verificar si la solicitud está dirigida a un endpoint de un controlador
                    {
                        await _next(context);
                        responseBody.Seek(0, SeekOrigin.Begin);
                        var serializer = new Newtonsoft.Json.JsonSerializer();
                        await using (var writer = new StreamWriter(originalBodyStream))
                        await using (var jsonWriter = new JsonTextWriter(writer))
                        {
                            var apiResponse = new GenericApiResponse<object>
                            {
                                Meta =
                                {
                                    Service = context.Request.Path,
                                    ResponseCode =  _.GetHttpStatusMessage(context.Response.StatusCode),
                                },
                                Data = JsonConvert.DeserializeObject<object>(await new StreamReader(responseBody).ReadToEndAsync())
                            };
                            serializer.Serialize(jsonWriter, apiResponse);
                        }
                        context.Response.Body = originalBodyStream;
                    }
                    else
                    {
                        await _next(context);
                    }
                }
                catch (PostgresException ex)
                {
                    _logger.LogError(ex, ex.Message);
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.ContentType = "application/json";
                    await SerializeApiResponseAsync(originalBodyStream, context, responseBody);
                }
                catch (ExceptionBase exBase)
                {
                    _logger.LogError(exBase, exBase.Message);
                    context.Response.StatusCode = exBase.HttpCode;
                    context.Response.ContentType = "application/json";
                    await SerializeApiResponseAsync(originalBodyStream, context, responseBody, exBase);
                    throw;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.ContentType = "application/json";
                    await SerializeApiResponseAsync(originalBodyStream, context, responseBody);
                }
            }
        }


        #region Helpers

        private async Task SerializeApiResponseAsync(Stream originalBodyStream, HttpContext context, Stream responseBody)
        {
            responseBody.Seek(0, SeekOrigin.Begin);
            var serializer = new Newtonsoft.Json.JsonSerializer();
            await using (var jsonWriter = new JsonTextWriter(new StreamWriter(originalBodyStream, leaveOpen: true)))
            {
                var responseCode = _.GetHttpStatusMessage(context.Response.StatusCode);

                var apiResponse = new GenericApiResponse<object>
                {
                    Meta = { Method = context.Request.Method, Service = context.Request.Path, ResponseCode = responseCode },
                    Data = JsonConvert.DeserializeObject<object>(await new StreamReader(responseBody).ReadToEndAsync())
                };
                serializer.Serialize(jsonWriter, apiResponse);
            }
            await Task.CompletedTask;
        }

        private async Task SerializeApiResponseAsync(Stream originalBodyStream, HttpContext context, Stream responseBody, ExceptionBase exceptionBase)
        {
            responseBody.Seek(0, SeekOrigin.Begin);
            var serializer = new Newtonsoft.Json.JsonSerializer();
            await using (var jsonWriter = new JsonTextWriter(new StreamWriter(originalBodyStream, leaveOpen: true)))
            {
                var responseCode = _.GetHttpStatusMessage(context.Response.StatusCode);

                var apiResponse = new GenericApiResponse<object>
                {
                    Meta = { Method = context.Request.Method, Service = context.Request.Path, ResponseCode = responseCode },
                    Data = null,
                    Errors = { ErrorsList = { exceptionBase } }
                };
                serializer.Serialize(jsonWriter, apiResponse);
            }
            await Task.CompletedTask;
        }
        #endregion
    }
}
