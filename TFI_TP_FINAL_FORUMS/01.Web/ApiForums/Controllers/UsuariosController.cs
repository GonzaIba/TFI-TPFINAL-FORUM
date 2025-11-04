using AutoMapper;
using Core.Contracts.Services;
using Core.Domain.Exceptions.BaseException;
using Core.Domain.GenericEntityClass;
using Core.Domain.IdentityModels;
using Core.Domain.Request;
using Core.Domain.Response;
using CrossCutting.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ApiForums.Controllers
{
    [Produces("application/json")]
    [ApiController]
    [Route("v1/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUsersService _usuarioService;

        public UsuariosController(
            IUsersService usuarioService,
            IMapper mapper
            )
        {
            _usuarioService = usuarioService;
            _mapper = mapper;
        }


        [HttpGet]
        [Route("ObtenerTopUsuariosSemana")]
        [AllowAnonymous]
        public async Task<IActionResult> ObtenerTopUsuariosSemana()
        {
            var result = await _usuarioService.GetTopLastWeekAsync();

            var mappedUsers = result.Select(r => new UsersForumPreviewResponse
            {
                CompleteName = r.Key.FirstName + " " + r.Key.LastName,
                Initials = r.Key.FirstName.Substring(0, 1) + r.Key.LastName?.Substring(0, 1) ?? "",
                ShortDescription = r.Key.UsersForum?.ShortDescriptionForum,
                LongDescription = r.Key.UsersForum?.LongDescriptionForum,
                Image = r.Key.UsersForum?.ImageForum,
                DateFrom = "Desde " + r.Key.CreatedDate.Year.ToString(),
                Score = r.Value,
                LastTimeOnline = r.Key.UsersForum?.LastTimeConnectedForum,
                Email = r.Key.Email
            });

            return Ok(mappedUsers);
        }


        [HttpGet]
        [Route("ObtenerUsuariosBuscador")]
        [AllowAnonymous]
        public async Task<IActionResult> ObtenerUsuariosBuscador()
        {
            var result = await _usuarioService.GetTopLastWeekAsync();

            var mappedUsers = result.Select(r => new UsersForumPreviewResponse
            {
                Email = r.Key.Email,
                CompleteName = r.Key.FirstName + " " + r.Key.LastName,
                Initials = r.Key.FirstName.Substring(0, 1) + r.Key.LastName?.Substring(0, 1) ?? "",
                DateFrom = "Desde " + r.Key.CreatedDate.Year.ToString(),
                Score = r.Value,
                LastTimeOnline = r.Key.UsersForum?.LastTimeConnectedForum,
                Image = r.Key.UsersForum?.ImageForum,
                LongDescription = r.Key.UsersForum?.LongDescriptionForum,
                ShortDescription = r.Key.UsersForum?.ShortDescriptionForum,
            });

            return Ok(mappedUsers);
        }


        [HttpGet]
        [Route("ObtenerUsuariosForos")]
        [AllowAnonymous]
        public async Task<IActionResult> ObtenerUsuariosForos([FromQuery] string userId, [FromQuery] int pageIndex = 1, [FromQuery] int pageCount = 10)
        {
            var result = await _usuarioService.GetUsersForumAsync(pageIndex, pageCount, userId);

            var mappedUsers = result.Item2.Select(r => new UserForumResponse
            {
                Name = r.Key.FirstName + " " + r.Key.LastName,
                Score = r.Value,
                Email = r.Key.Email,
                CreatedDate =  r.Key.CreatedDate,
            });

            var response = _mapper.Map<List<UserForumResponse>>(
                mappedUsers.ToList(),
                opt => opt.Items["UserId"] = userId
            );

            PaginatedList<UserForumResponse> paginatedList = new(response, pageIndex, pageCount, result.Item1.TotalCount, result.Item1.PageCount);
            return Ok(paginatedList);
        }


        [HttpGet]
        [Route("ObtenerDetalleUsuarioForos")]
        [AllowAnonymous]
        public async Task<IActionResult> ObtenerDetalleUsuarioForos([FromQuery] string userEmail)
        {
            var result = await _usuarioService.GetDetailUserAsync(userEmail);
            var userForum = _mapper.Map<DetailsUserForumResponse>(result);
            return Ok(userForum);
        }

        [HttpGet]
        [Route("ObtenerNotificacionesForo")]
        [AllowAnonymous]
        public async Task<IActionResult> GetNotificationForum([FromQuery] string userId)
        {
            var result = await _usuarioService.GetNotificationsAsync(userId);
            var userForum = _mapper.Map<List<NotificationsResponse>>(result);
            return Ok(userForum);
        }

        [HttpGet]
        [Route("Alertas")]
        public async Task<IActionResult> GetAlerts([FromQuery] string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return BadRequest("Debe indicar el usuario.");

            var alerts = await _usuarioService.GetAlertsAsync(userId.Trim());
            return Ok(alerts);
        }

        [HttpPost]
        [Route("MarcarNotificacionleida")]
        public async Task<IActionResult> MarkNotificationAsRead([FromBody] MarkNotificationAsReadRequest request)
        {
            var result = await _usuarioService.MarkNotificationAsReadAsync(request);
            var successfulResponse = _mapper.Map<SuccessfulResponse>(result);
            return Ok(successfulResponse);
        }
    }
}
