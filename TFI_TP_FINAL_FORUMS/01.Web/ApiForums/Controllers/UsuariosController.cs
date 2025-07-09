using AutoMapper;
using Core.Contracts.Services;
using Core.Domain.Exceptions.BaseException;
using Core.Domain.Response;
using CrossCutting.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
                CompleteName = r.Key.Nombre + " " + r.Key.Apellido,
                Initials = r.Key.Nombre.Substring(0, 1) + r.Key.Apellido?.Substring(0, 1) ?? "",
                ShortDescription = r.Key.UsersForum?.ShortDescriptionForum,
                LongDescription = r.Key.UsersForum?.LongDescriptionForum,
                Image = r.Key.UsersForum?.ImageForum,
                DateFrom = "Desde " + r.Key.FechaCreado.Year.ToString(),
                Score = r.Value,
                LastTimeOnline = r.Key.UsersForum?.LastTimeConnectedForum ?? r.Key.FechaCreado
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
                CompleteName = r.Key.Nombre + " " + r.Key.Apellido,
                Initials = r.Key.Nombre.Substring(0, 1) + r.Key.Apellido?.Substring(0, 1) ?? "",
                DateFrom = "Desde " + r.Key.FechaCreado.Year.ToString(),
                Score = r.Value,
            });

            return Ok(mappedUsers);
        }


        [HttpGet]
        [Route("ObtenerUsuariosForos")]
        [AllowAnonymous]
        public async Task<IActionResult> ObtenerUsuariosForos([FromQuery] string userId)
        {
            var result = await _usuarioService.GetUsersForumAsync(userId);

            var mappedUsers = result.Select(r => new UserForumResponse
            {
                Name = r.Key.Nombre + " " + r.Key.Apellido,
                Score = r.Value,
                Email = r.Key.Email,
                CreatedDate =  r.Key.FechaCreado,
            });

            //var usersForum = _mapper.Map<IEnumerable<UserForumResponse>>(result);
            return Ok(mappedUsers);
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
    }
}
