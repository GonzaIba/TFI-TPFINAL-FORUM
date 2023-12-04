using AutoMapper;
using Core.Contracts.Services;
using Core.Domain.Exceptions.BaseException;
using Core.Domain.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiForums.Controllers
{
    [Produces("application/json")]
    [ApiController]
    [Route("[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUsersService _usuarioService;
        
        public UsuariosController(
            IUsersService usuarioService,
            IMapper mapper,
            ILogger<UsuariosController> logger
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
            try
            {
                var result = await _usuarioService.GetTopLastWeek();

                var mappedUsers = result.Select(r => new UsuariosTopResponse
                {
                    NombreCompleto = r.Key.Nombre + " " + r.Key.Apellido, // Asumiendo que Users tiene una propiedad llamada Nombre
                    Iniciales = r.Key.Nombre.Substring(0, 1) + r.Key.Apellido?.Substring(0, 1) ?? "", // Asumiendo que Users tiene una propiedad llamada Nombre
                    DescripcionCorta = r.Key.DescripcionCortaForum,
                    DescripcionLarga = r.Key.DescripcionLargaForum,
                    Image = r.Key.ImageForum,
                    FechaDesde = "Desde " + r.Key.FechaCreado.Year.ToString(),
                    Puntaje = r.Value,
                    UltimaVezConectado = r.Key.UltimaVezConectadoForum // Asumiendo que Users tiene una propiedad llamada LastConnected
                });

                return Ok(mappedUsers);
            }
            catch (ApiForumException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
