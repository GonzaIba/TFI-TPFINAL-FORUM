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
        private readonly ILogger<PublicacionesController> _logger;
        public PublicacionesController(
            IPublicacionService publicacionService,
            IMapper mapper,
            ILogger<PublicacionesController> logger
            )
        {
            _publicacionService = publicacionService;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpPost]
        [Route("CrearPublicacion")]
        public async Task<IActionResult> CreatePublication([FromQuery] string userId, CreatePublicationRequest publication)
        {
            var publicacionModel = _mapper.Map<PublicacionModel>(publication);
            var result = await _publicacionService.CreatePublication(userId, publicacionModel);            
            return Ok(new SuccessfulResponse(result));
        }

        [HttpPost]
        [Route("AgregarRespuesta")]
        public async Task<IActionResult> AddAnswer([FromBody] AddAnswerRequest answerRequest)
        {
            var result = await _publicacionService.AddAnswer(answerRequest);
            return Ok(new SuccessfulResponse(result));
        }

        [HttpPost]
        [Route("GuardarPublicacion")]
        public async Task<IActionResult> SavePublication([FromBody] SavePublicationRequest publicationRequest)
        {
            var result = await _publicacionService.SavePublication(publicationRequest.UserId, publicationRequest.CodigoPublicacion);
            return Ok(new SuccessfulResponse(result));
        }

        [HttpDelete]
        [Route("EliminarPublicacionGuardada")]
        public async Task<IActionResult> DeleteSavedPublication([FromQuery] string userId, int codePublication)
        {
            var result = await _publicacionService.DeleteSavedPublication(userId, codePublication);
            return Ok(new SuccessfulResponse(result));
        }

        [HttpGet]
        [Route("ObtenerPublicaciones")]
        public async Task<IActionResult> GetPublications([FromQuery] string? userId)
        {
            var publicaciones = await _publicacionService.GetPublications();
            var publicacionesList = publicaciones.ToList(); // <-- importantísimo
            var publicacionesResponse = _mapper.Map<IEnumerable<PublicationResponse>>(
                publicacionesList,
                opt => opt.Items["UserId"] = userId
            );
            return Ok(publicacionesResponse);
        }

        [HttpGet]
        [Route("ObtenerDetallePublicacion")]
        public async Task<IActionResult> GetDetailPublication([FromQuery] int codePublication, [FromQuery] string? userId)
        {
            var publication = await _publicacionService.GetDetailPublication(codePublication);
            var publicationResponse = _mapper.Map<PublicationDetailResponse>(
                publication,
                opt => opt.Items["UserId"] = userId
            );
            return Ok(publicationResponse);
        }

        [HttpGet]
        [Route("ObtenerPublicacionesCreadasPorUsuario")]
        public async Task<IActionResult> GetCreatedPublicationsByUser([FromQuery] string userId)
        {
            var publications = await _publicacionService.GetCreatedPublicationByUser(userId); //////////////////////////////////////////////////////
            var publicationsResponse = _mapper.Map<IEnumerable<PublicationResponse>>(
                publications.ToList(),
                opt => opt.Items["UserId"] = userId
            );
            return Ok(publicationsResponse);
        }

        [HttpGet]
        [Route("ObtenerPublicacionesGuardadas")]
        public async Task<IActionResult> GetSavedPublications([FromQuery] string userId)
        {
            var publications = await _publicacionService.GetSavedPublications(userId); //////////////////////////////////////////////////////
            var publicationsResponse = _mapper.Map<IEnumerable<PublicationResponse>>(
                publications.ToList(),
                opt => opt.Items["UserId"] = userId
            );
            return Ok(publicationsResponse);
        }

        [HttpPost]
        [Route("VotarPublicacion")]
        public async Task<IActionResult> PublicationVote([FromBody] PublicationVoteRequest request)
        {
            var result = await _publicacionService.UserPublicationVote(request);
            return Ok(result);
        }

        [HttpPost]
        [Route("VotarRespuesta")]
        public async Task<IActionResult> AnswerVote([FromBody] AnswerVoteRequest request)
        {
            var result = await _publicacionService.UserAnswerVote(request);
            return Ok(result);
        }

        [HttpGet]
        [Route("PredecirEtiquetasPorTexto")]
        public async Task<IActionResult> GetPublicationByFilter([FromQuery] string texto)
        {
            var publicaciones = await _publicacionService.GetPublicationByFilter(texto);
            return Ok(publicaciones);
        }
    }
}
