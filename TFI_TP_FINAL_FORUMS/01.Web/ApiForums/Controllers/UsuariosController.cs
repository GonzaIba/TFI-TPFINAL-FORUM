using AutoMapper;
using Core.Contracts.Services;
using Core.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace ApiForums.Controllers
{
    [Produces("application/json")]
    [ApiController]
    [Route("[controller]")]
    public class UsuariosController : BaseApiController<PublicacionesController>
    {
        private readonly IMapper _mapper;
        private readonly IUsersService _usuarioService;
        
        public UsuariosController(
            IUsersService usuarioService,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper,
            ILogger<PublicacionesController> logger
            )
            : base(httpContextAccessor, logger)
        {
            _usuarioService = usuarioService;
            _mapper = mapper;
        }

        [HttpPost]
        [Route("ObtenerTopUsuariosSemana")]
        public async Task<IActionResult> ObtenerTopUsuariosSemana()
        {
            try
            {

                return Ok<string>();
            }
            catch (ApiForumException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
