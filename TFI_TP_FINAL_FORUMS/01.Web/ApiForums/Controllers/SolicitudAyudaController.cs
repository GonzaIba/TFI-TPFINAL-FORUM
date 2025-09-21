using AutoMapper;
using Core.Contracts.Publishers;
using Core.Contracts.Services;
using Core.Domain.GenericEntityClass;
using Core.Domain.Request;
using Core.Domain.Response;
using Microsoft.AspNetCore.Mvc;

namespace ApiForums.Controllers
{
    [Produces("application/json")]
    [ApiController]
    [Route("v1/[controller]")]
    public class SolicitudAyudaController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILogger<SolicitudAyudaController> _logger;
        private readonly ISolicitudAyudaService _solicitudAyudaService;

        public SolicitudAyudaController(
            ILogger<SolicitudAyudaController> logger,
            ISolicitudAyudaService solicitudAyudaService,
            IMapper mapper
            )
        {
            _logger = logger;
            _solicitudAyudaService = solicitudAyudaService;
            _mapper = mapper;
        }

        [HttpGet("ObtenerSolicitudesDeAyuda")]
        public async Task<IActionResult> GetRequestsHelp(
            [FromQuery] int limit = 8,
            [FromQuery] string? after = null,
            [FromQuery] DateTime? anchorUtc = null,
            [FromQuery] string? userId = null,
            [FromQuery] string? search = null)
        {
            var anchor = anchorUtc ?? DateTime.UtcNow;

            (DateTime createdAt, int id)? cursor = null;
            if (!string.IsNullOrWhiteSpace(after))
            {
                var parts = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(after)).Split(':');
                if (parts.Length == 2 &&
                    long.TryParse(parts[0], out var ticks) &&
                    int.TryParse(parts[1], out var lastId))
                {
                    cursor = (new DateTime(ticks, DateTimeKind.Utc), lastId);
                }
            }

            // Normalización de búsqueda
            search = string.IsNullOrWhiteSpace(search) ? null : search.Trim();
            if (search is { Length: > 100 }) search = search.Substring(0, 100); // anti-abuso

            var page = await _solicitudAyudaService.GetRequestsHelp(limit, anchor, cursor, userId, search);

            var mapped = _mapper.Map<CursorPage<RequestHelpResponse>>(page);
            mapped.AnchorUtc = anchor;
            return Ok(mapped);
        }


    }
}
