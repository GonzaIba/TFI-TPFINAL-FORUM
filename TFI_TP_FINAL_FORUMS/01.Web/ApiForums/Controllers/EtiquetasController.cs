using AutoMapper;
using Core.Contracts.Services;
using Core.Domain.Enum;
using Core.Domain.Exceptions.BaseException;
using Core.Domain.GenericEntityClass;
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
        private readonly IEtiquetaService _labelService;
        
        public EtiquetasController(
            IPublicacionService publicacionService,
            IEtiquetaService labelService,
            IMapper mapper
            )
        {
            _publicacionService = publicacionService;
            _labelService = labelService;
            _mapper = mapper;
        }

        [HttpPost]
        [Route("CrearEtiqueta")]
        public async Task<IActionResult> CreateLabel(List<string> Etiquetas)
        {
            //var publicacionModel = _mapper.Map<PublicacionModel>(publicacion);
            await _labelService.CreateLabel(Etiquetas.FirstOrDefault());
            return Ok();
        }

        [HttpGet]
        [Route("ObtenerEtiquetasPorNombre")]
        public async Task<IActionResult> GetLabelsByName([FromQuery] string rawQuery, [FromQuery] int pageIndex = 1, [FromQuery] int pageCount = 10)
        {
            var etiquetas = await _labelService.GetLabelsByName(rawQuery, pageIndex, pageCount);
            var etiquetasResponse = _mapper.Map<PaginatedList<LabelResponse>>(etiquetas);
            return Ok(etiquetasResponse);
        }

        [HttpGet]
        [Route("ObtenerEtiquetasPorFiltro")]
        public async Task<IActionResult> GetLabelsByFilter([FromQuery] LabelFiltersEnum filterEnum, [FromQuery] int pageIndex = 1, [FromQuery] int pageCount = 10)
        {
            var etiquetas = await _labelService.GetLabelsByFilter(filterEnum, pageIndex, pageCount);
            var etiquetasResponse = _mapper.Map<PaginatedList<LabelResponse>>(etiquetas);
            return Ok(etiquetasResponse);
        }
    }
}
