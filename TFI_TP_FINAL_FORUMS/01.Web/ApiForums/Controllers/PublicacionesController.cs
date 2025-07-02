using AutoMapper;
using Core.Contracts.Publishers;
using Core.Contracts.Services;
using Core.Domain.Exceptions.BaseException;
using Core.Domain.IdentityModels;
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
        private readonly IPublisherPublication _publisherService;

        public PublicacionesController(
            IPublicacionService publicacionService,
            IMapper mapper,
            ILogger<PublicacionesController> logger,
            IPublisherPublication publisherService
            )
        {
            _publicacionService = publicacionService;
            _mapper = mapper;
            _logger = logger;
            _publisherService = publisherService;
        }

        [HttpPost]
        [Route("CrearPublicacion")]
        public async Task<IActionResult> CreatePublication([FromBody] CreatePublicationRequest publication)
        {
            var result = await _publicacionService.CreatePublication(publication);            
            return Ok(new SuccessfulResponse(result));
        }

        [HttpPut]
        [Route("EditarPublicacion")]
        public async Task<IActionResult> EditPublication([FromQuery] string userId, EditPublicationRequest request)
        {
            var result = await _publicacionService.EditPublication(userId, request);
            return Ok(new SuccessfulResponse(result));
        }

        [HttpPut]
        [Route("EditarRespuesta")]
        public async Task<IActionResult> EditAnswer([FromQuery] string userId, EditAnswerRequest request)
        {
            var result = await _publicacionService.EditAnswer(userId, request);
            return Ok(new SuccessfulResponse(result));
        }

        [HttpPost]
        [Route("AgregarRespuesta")]
        public async Task<IActionResult> AddAnswer([FromBody] AddAnswerRequest answerRequest)
        {
            var result = await _publicacionService.AddAnswer(answerRequest);
            var answerResponse = _mapper.Map<AnswerResponse>(result, opt => opt.Items["UserId"] = answerRequest.UserId);
            await _publisherService.PublishAddAnswerAsync(answerResponse, answerRequest.ConnectionId, result.IDPublicacion);
            return Ok(answerResponse);
        }

        [HttpPost]
        [Route("GuardarPublicacion")]
        public async Task<IActionResult> SavePublication([FromBody] SavePublicationRequest publicationRequest)
        {
            var result = await _publicacionService.SavePublication(publicationRequest.UserId, publicationRequest.CodePublication);
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
            var publicacionesModel = await _publicacionService.GetPublications();
            var publicacionesResponse = _mapper.Map<IEnumerable<PublicationResponse>>(
                publicacionesModel,
                opt => opt.Items["UserId"] = userId
            );
            return Ok(publicacionesResponse);
        }

        [HttpGet]
        [Route("ObtenerTopPublicacionesSemana")]
        public async Task<IActionResult> GetTopPublications([FromQuery] string? userId)
        {
            var publicaciones = await _publicacionService.GetTopPublications();
            var publicacionesList = publicaciones.ToList(); // <-- importantísimo
            var publicacionesResponse = _mapper.Map<IEnumerable<PublicationResponse>>(
                publicacionesList,
                opt => opt.Items["UserId"] = userId
            );
            return Ok(publicacionesResponse);
        }

        [HttpGet]
        [Route("ObtenerPublicacionesRelacionadas")]
        public async Task<IActionResult> GetRelatedPublications([FromQuery] int codePublication)
        {
            var publicaciones = await _publicacionService.GetRelatedPublications(codePublication);
            var publicacionesList = publicaciones.ToList(); // <-- importantísimo
            var publicacionesResponse = _mapper.Map<IEnumerable<PublicationResponse>>(publicacionesList);
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

        [HttpPost]
        [Route("EliminarRespuestaPropia")]
        public async Task<IActionResult> DeleteAnswerByUser([FromBody] DeleteAnswerRequest request)
        {
            var result = await _publicacionService.DeleteAnswerByUser(request);
            return Ok(new SuccessfulResponse(result));
        }

        [HttpGet]
        [Route("PredecirEtiquetasPorTexto")]
        public async Task<IActionResult> GetPublicationByFilter([FromQuery] string texto)
        {
            var publicaciones = await _publicacionService.PredictLabel(texto);
            return Ok(publicaciones);
        }
    }
}
