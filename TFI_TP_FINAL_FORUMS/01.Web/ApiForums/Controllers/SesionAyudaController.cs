using AutoMapper;
using Core.Contracts.Services;
using Core.Domain.IdentityModels;
using Core.Domain.Request;
using Core.Domain.Response;
using Microsoft.AspNetCore.Mvc;

namespace ApiForums.Controllers
{
    [Produces("application/json")]
    [ApiController]
    [Route("v1/[controller]")]
    public class SesionAyudaController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILogger<SesionAyudaController> _logger;
        private readonly ISesionAyudaService _sesionAyudaService;

        public SesionAyudaController(
            ILogger<SesionAyudaController> logger,
            ISesionAyudaService sesionAyudaService,
            IMapper mapper
            )
        {
            _logger = logger;
            _sesionAyudaService = sesionAyudaService;
            _mapper = mapper;
        }

        [HttpPost("AceptarTerminosCondiciones")]
        public async Task<IActionResult> AcceptTyC([FromBody] AcceptTyCRequest request)
        {
            var result = await _sesionAyudaService.AcceptTyC(request);
            return Ok(new SuccessfulResponse(result));
        }

        [HttpGet("ObtenerTerminosCondiciones")]
        public async Task<IActionResult> GetTyC()
        {
            var result = await _sesionAyudaService.GetTyC();
            var tyc = _mapper.Map<TerminosCondicionesResponse>(result);
            return Ok(tyc);
        }

        [HttpGet("ObtenerSesion")]
        public async Task<IActionResult> GetSession([FromQuery] int codeRequestHelp, [FromQuery] string userId)
        {
            var result = await _sesionAyudaService.GetSession(codeRequestHelp, userId);
            var session = _mapper.Map<SessionResponse>(result, opt => { opt.Items["UserId"] = userId; } );
            return Ok(session);
        }

        [HttpPost("IngresarSesion")]
        public async Task<IActionResult> EnterSession([FromBody] EnterSessionRequest enterSession)
        {
            var result = await _sesionAyudaService.EnterSession(enterSession);
            var session = _mapper.Map<SessionResponse>(result, opt => { opt.Items["UserId"] = enterSession.UserId; } );
            return Ok(session);
        }
    }
}
