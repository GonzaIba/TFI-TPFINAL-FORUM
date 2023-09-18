using Core.Domain.Enum;
using CrossCutting.Helpers.ResponseClasses;
using Microsoft.AspNetCore.Mvc;

namespace ApiForums.Controllers
{
    //[ServiceFilter(typeof(LogUserActivity))]
    [Route("api/[controller]")]
    [ApiController]
    public class BaseApiController<T> : ControllerBase where T : BaseApiController<T>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<T> _logger;
        public BaseApiController(
            IHttpContextAccessor httpContextAccessor,
            ILogger<T> logger
            )
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        [NonAction]
        public OkObjectResult Ok<K>(IEnumerable<K> data = default(IEnumerable<K>), string Message = "Ok")
        {
            GenericApiResponse<IEnumerable<K>> genericApiResponse = new GenericApiResponse<IEnumerable<K>>()
            {
                Service = _httpContextAccessor.HttpContext.Request.Path.Value,
                Data = data,
                ResponseMessage = Message,
                ResponseCode = ResponseCodeEnum.Success.ToString()
            };
            _logger.LogInformation("OK");
            return base.Ok(genericApiResponse);
        }

        [NonAction]
        public BadRequestObjectResult BadRequest(string Message = "BadRequest")
        {
            GenericApiResponse<string> genericApiResponse = new GenericApiResponse<string>()
            {
                Service = _httpContextAccessor.HttpContext.Request.Path.Value,
                Data = null,
                ResponseMessage = Message,
                ResponseCode = ResponseCodeEnum.NotProcessed.ToString()
            };
            _logger.LogInformation("BadRequest");
            return base.BadRequest(genericApiResponse);
        }
    }
}
