using AutoMapper;
using Core.Contracts.Services;
using Core.Domain.Exceptions;
using Core.Domain.Models;
using Core.Domain.Request;
using Core.Domain.Response;
using CrossCutting.Helpers.ResponseClasses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections;

namespace ApiForums.Controllers
{
    [Produces("application/json")]
    [ApiController]
    [Route("[controller]")]
    public class PublicacionesController : BaseApiController<PublicacionesController>
    {
        private readonly IMapper _mapper;
        private readonly IPublicacionService _publicacionService;
        public PublicacionesController(
            IPublicacionService publicacionService,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper,
            ILogger<PublicacionesController> logger
            )
            : base(httpContextAccessor, logger)
        {
            _publicacionService = publicacionService;
            _mapper = mapper;
        }

        [HttpPost]
        [Route("CrearPublicacion")]
        public async Task<IActionResult> CrearPublicacion([FromQuery] string userId, CrearPublicacionRequest publicacion)
        {
            try
            {
                var publicacionModel = _mapper.Map<PublicacionModel>(publicacion);
                await _publicacionService.CrearPublicacion(userId, publicacionModel);
                
                return Ok<string>();
            }
            catch (ApiForumException ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet]
        [Route("ObtenerPublicaciones")]
        public async Task<IActionResult> ObtenerPublicaciones()
        {
            try
            {
                var publicaciones = await _publicacionService.ObtenerPublicaciones();
                var publicacionesResponse = _mapper.Map<IEnumerable<PublicacionesResponse>>(publicaciones);
                
                return Ok(publicacionesResponse);
            }
            catch (ApiForumException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("PredecirEtiquetaPublicacion")]
        public async Task<IActionResult> ObtenerPublicaciones([FromBody]string texto)
        {
            try
            {
                var publicaciones = await _publicacionService.ObtenerPublicaciones();
                var publicacionModel = _mapper.Map<IEnumerable<PublicacionesResponse>>(publicaciones);

                return Ok(publicacionModel);
            }
            catch (ApiForumException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
