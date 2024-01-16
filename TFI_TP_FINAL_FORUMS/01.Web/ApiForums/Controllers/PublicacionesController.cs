using AutoMapper;
using Core.Contracts.Services;
using Core.Domain.Exceptions.BaseException;
using Core.Domain.Models;
using Core.Domain.Request;
using Core.Domain.Response;
using Microsoft.AspNetCore.Mvc;

namespace ApiForums.Controllers
{
    [Produces("application/json")]
    [ApiController]
    [Route("v1/[controller]")]
    public class PublicacionesController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IPublicacionService _publicacionService;
        public PublicacionesController(
            IPublicacionService publicacionService,
            IMapper mapper,
            ILogger<PublicacionesController> logger
            )
        {
            _publicacionService = publicacionService;
            _mapper = mapper;
        }

        [HttpPost]
        [Route("CrearPublicacion")]
        public async Task<IActionResult> CrearPublicacion([FromQuery] string userId, CrearPublicacionRequest publicacion)
        {
            var publicacionModel = _mapper.Map<PublicacionModel>(publicacion);
            await _publicacionService.CrearPublicacion(userId, publicacionModel);
                
            return Ok();
        }


        [HttpGet]
        [Route("ObtenerPublicaciones")]
        public async Task<IActionResult> ObtenerPublicaciones()
        {
            var publicaciones = await _publicacionService.ObtenerPublicaciones();
            var publicacionesResponse = _mapper.Map<IEnumerable<PublicacionesResponse>>(publicaciones);
                
            return Ok(publicacionesResponse);
        }

        [HttpGet]
        [Route("PredecirEtiquetasPorTexto")]
        public async Task<IActionResult> PredecirEtiquetasPorTexto([FromQuery] string texto)
        {
            var publicaciones = await _publicacionService.ObtenerPublicacionesPorFiltro(texto);
            return Ok(publicaciones);
        }
    }
}
