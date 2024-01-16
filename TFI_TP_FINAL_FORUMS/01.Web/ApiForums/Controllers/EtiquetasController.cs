using AutoMapper;
using Core.Contracts.Services;
using Core.Domain.Exceptions.BaseException;
using Core.Domain.Response;
using Microsoft.AspNetCore.Mvc;

namespace ApiForums.Controllers
{
    [Produces("application/json")]
    [ApiController]
    [Route("v1/[controller]")]
    public class EtiquetasController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IPublicacionService _publicacionService;
        private readonly IEtiquetaService _etiquetaService;
        
        public EtiquetasController(
            IPublicacionService publicacionService,
            IEtiquetaService etiquetaService,
            IMapper mapper
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
            //var publicacionModel = _mapper.Map<PublicacionModel>(publicacion);
            await _etiquetaService.CrearEtiqueta(Etiquetas.FirstOrDefault());
            return Ok();
        }

        [HttpGet]
        [Route("ObtenerEtiquetas")]
        public async Task<IActionResult> ObtenerEtiquetas()
        {
            var etiquetas = await _etiquetaService.ObtenerEtiquetasDetalle();
            var etiquetasResponse = _mapper.Map<IEnumerable<EtiquetasResponse>>(etiquetas);
            etiquetasResponse.ToList().ForEach(x => x.EtiquetasPublicaciones.ToList().ForEach(y => y.Etiqueta = null));
                    
            return Ok(etiquetasResponse);
        }
    }
}
