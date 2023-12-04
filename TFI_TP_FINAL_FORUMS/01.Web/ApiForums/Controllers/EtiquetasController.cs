using AutoMapper;
using Core.Contracts.Services;
using Core.Domain.Exceptions.BaseException;
using Core.Domain.Exceptions.BusinessExceptions.Auth;
using Core.Domain.Models;
using Core.Domain.Request;
using Core.Domain.Response;
using Microsoft.AspNetCore.Mvc;

namespace ApiForums.Controllers
{
    [Produces("application/json")]
    [ApiController]
    [Route("[controller]")]
    public class EtiquetasController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IPublicacionService _publicacionService;
        private readonly IEtiquetaService _etiquetaService;
        
        public EtiquetasController(
            IPublicacionService publicacionService,
            IEtiquetaService etiquetaService,
            IMapper mapper,
            ILogger<EtiquetasController> logger
            )
        {
            _publicacionService = publicacionService;
            _etiquetaService = etiquetaService;
            _mapper = mapper;
        }

        [HttpPost]
        [Route("CrearEtiqueta")]
        public async Task<IActionResult> CrearEtiqueta(List<string> Etiquetas)
        {
            try
            {
                //var publicacionModel = _mapper.Map<PublicacionModel>(publicacion);
                //await _publicacionService.CrearPublicacion(userId, publicacionModel);
                throw new Exception("Error");
                return Ok();
            }
            catch (ApiForumException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("ObtenerEtiquetas")]
        public async Task<IActionResult> ObtenerEtiquetas()
        {
            try
            {
                var etiquetas = await _etiquetaService.ObtenerEtiquetasDetalle();
                var etiquetasResponse = _mapper.Map<IEnumerable<EtiquetasResponse>>(etiquetas);
                etiquetasResponse.ToList().ForEach(x => x.EtiquetasPublicaciones.ToList().ForEach(y => y.Etiqueta = null));
                    
                return Ok(etiquetasResponse);
            }
            catch (ApiForumException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
