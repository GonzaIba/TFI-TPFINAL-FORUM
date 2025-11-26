using Core.Domain.Enum;
using Core.Domain.Exceptions.BaseException;
using Core.Domain.Response.BaseResponse;
using Newtonsoft.Json;
using Npgsql;
using System.Net;
using System.Text;
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
            // No tocar las rutas MCP
            if (_.IsMcp(context))
            {
                await _next(context);
                return;
            }

            var originalBodyStream = context.Response.Body;

            await using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            try
            {
                // Ejecuta el pipeline normal
                await _next(context);

                // Si es un endpoint de controlador, envolvemos la respuesta en GenericApiResponse
                if (_.IsControllerPath(context.Request.Path))
                {
                    await WrapSuccessResponseAsync(context, originalBodyStream, responseBody);
                }
                else
                {
                    // Para rutas que no queremos tocar, devolvemos tal cual
                    responseBody.Seek(0, SeekOrigin.Begin);
                    await responseBody.CopyToAsync(originalBodyStream);
                }
            }
            catch (PostgresException ex)
            {
                _logger.LogError(ex, ex.Message);

                if (!context.Response.HasStarted)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.ContentType = "application/json; charset=utf-8";
                }

                await WrapErrorResponseAsync(context, originalBodyStream, responseBody, null);
            }
            catch (ExceptionBase exBase)
            {
                _logger.LogError(exBase, exBase.Message);

                if (!context.Response.HasStarted)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.ContentType = "application/json; charset=utf-8";
                }

                await WrapErrorResponseAsync(context, originalBodyStream, responseBody, exBase);

                // Si querés que el middleware “se coma” el error, sacá este throw
                //throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);

                if (!context.Response.HasStarted)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.ContentType = "application/json; charset=utf-8";
                }

                await WrapErrorResponseAsync(context, originalBodyStream, responseBody, null);
            }
            finally
            {
                context.Response.Body = originalBodyStream;
            }
        }

        #region Helpers

        private async Task WrapSuccessResponseAsync(
            HttpContext context,
            Stream originalBodyStream,
            Stream responseBody)
        {
            responseBody.Seek(0, SeekOrigin.Begin);
            var bodyText = await new StreamReader(responseBody).ReadToEndAsync();

            var apiResponse = new GenericApiResponse<object>
            {
                Meta =
                {
                    Method = context.Request.Method,
                    Service = context.Request.Path,
                    ResponseCode = _.GetHttpStatusMessage(context.Response.StatusCode)
                },
                Data = string.IsNullOrWhiteSpace(bodyText)
                    ? null
                    : JsonConvert.DeserializeObject<object>(bodyText)
            };

            var json = JsonConvert.SerializeObject(apiResponse);
            var buffer = Encoding.UTF8.GetBytes(json);

            context.Response.Body = originalBodyStream;
            context.Response.ContentType ??= "application/json; charset=utf-8";

            await originalBodyStream.WriteAsync(buffer, 0, buffer.Length);
            await originalBodyStream.FlushAsync();
        }

        private async Task WrapErrorResponseAsync(
            HttpContext context,
            Stream originalBodyStream,
            Stream responseBody,
            ExceptionBase? exceptionBase)
        {
            // Intentamos leer el body que haya quedado (si hay algo)
            responseBody.Seek(0, SeekOrigin.Begin);
            var bodyText = await new StreamReader(responseBody).ReadToEndAsync();

            var responseCode = _.GetHttpStatusMessage(context.Response.StatusCode);

            var apiResponse = new GenericApiResponse<object>
            {
                Meta =
                {
                    Method = context.Request.Method,
                    Service = context.Request.Path,
                    ResponseCode = responseCode
                },
                Data = (!string.IsNullOrWhiteSpace(bodyText) && exceptionBase == null)
                    ? JsonConvert.DeserializeObject<object>(bodyText)
                    : null
            };

            if (exceptionBase != null)
            {
                apiResponse.Errors.ErrorsList.Add(exceptionBase);
            }

            var json = JsonConvert.SerializeObject(apiResponse);
            var buffer = Encoding.UTF8.GetBytes(json);

            context.Response.Body = originalBodyStream;
            context.Response.ContentType ??= "application/json; charset=utf-8";

            await originalBodyStream.WriteAsync(buffer, 0, buffer.Length);
            await originalBodyStream.FlushAsync();
        }

        #endregion
    }
}
