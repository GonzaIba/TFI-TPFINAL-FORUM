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
                NombreCompleto = r.Key.Nombre + " " + r.Key.Apellido,
                Iniciales = r.Key.Nombre.Substring(0, 1) + r.Key.Apellido?.Substring(0, 1) ?? "",
                DescripcionCorta = r.Key.UsersForum?.ShortDescriptionForum,
                DescripcionLarga = r.Key.UsersForum?.LongDescriptionForum,
                Image = r.Key.UsersForum?.ImageForum,
                FechaDesde = "Desde " + r.Key.FechaCreado.Year.ToString(),
                Puntaje = r.Value,
                UltimaVezConectado = r.Key.UsersForum?.LastTimeConnectedForum ?? r.Key.FechaCreado
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
                NombreCompleto = r.Key.Nombre + " " + r.Key.Apellido,
                Iniciales = r.Key.Nombre.Substring(0, 1) + r.Key.Apellido?.Substring(0, 1) ?? "",
                FechaDesde = "Desde " + r.Key.FechaCreado.Year.ToString(),
                Puntaje = r.Value,
            });

            return Ok(mappedUsers);
        }


        [HttpGet]
        [Route("ObtenerUsuariosForos")]
        [AllowAnonymous]
        public async Task<IActionResult> ObtenerUsuariosForos([FromQuery] string userId)
        {
            var result = await _usuarioService.GetUsersForumAsync(userId);
            var usersForum = _mapper.Map<IEnumerable<UsersForumResponse>>(result);
            return Ok(usersForum);
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
