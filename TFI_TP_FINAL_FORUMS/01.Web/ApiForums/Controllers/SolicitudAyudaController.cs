using AutoMapper;
using Core.Contracts.Publishers;
using Core.Contracts.Services;
using Core.Domain.GenericEntityClass;
using Core.Domain.Models;
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
        private readonly ISolicitudAyudaChatService _chatService;
        private readonly ISolicitudAyudaChatMensajeService _chatMsgService;

        public SolicitudAyudaController(
            ILogger<SolicitudAyudaController> logger,
            ISolicitudAyudaService solicitudAyudaService,
            ISolicitudAyudaChatService chatService,
            ISolicitudAyudaChatMensajeService chatMsgService,
            IMapper mapper
            )
        {
            _logger = logger;
            _solicitudAyudaService = solicitudAyudaService;
            _chatService = chatService;
            _chatMsgService = chatMsgService;
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

        [HttpPost("CrearSolicitudAyuda")]
        public async Task<IActionResult> CreateHelpRequest([FromBody] CreateHelpRequest request)
        {
            //Validaciones...
            var result = await _solicitudAyudaService.CreateHelpRequest(request);
            return Ok(new SuccessfulResponse(result));
        }

        //[HttpGet("{id:int}/Chat")] 
        //public async Task<IActionResult> GetOrCreateChat([FromRoute] int id)
        //{
        //    var chat = await _chatService.GetOrCreateBySolicitudAsync(id);
        //    var resp = new ChatResponse { CodeChat = chat.IDChat, CodeRequestHelp = id };
        //    return Ok(resp);
        //}

        [HttpGet("{id:int}/Chat/Mensajes")]
        public async Task<IActionResult> GetChatMessages(
            [FromRoute] int id,
            [FromQuery] string userId,
            [FromQuery] DateTime? afterUtc = null,
            [FromQuery] int take = 50,
            [FromQuery] bool asc = false)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return BadRequest("userId requerido");

            var chat = await _chatService.GetOrCreateBySolicitudAsync(id);
            var msgs = await _chatMsgService.GetMessagesAsync(chat.IDChat, afterUtc, take, asc, userId);
            var mapped = _mapper.Map<IEnumerable<ChatMessageResponse>>(msgs, opt => opt.Items["UserId"] = userId);
            return Ok(mapped);
        }

        [HttpPost("{id:int}/Chat/Mensajes")]
        public async Task<IActionResult> SendChatMessage([FromRoute] int id, [FromBody] SendChatMessageRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.UserId) || string.IsNullOrWhiteSpace(request.Message))
                return BadRequest("Datos inválidos");

            var chat = await _chatService.GetOrCreateBySolicitudAsync(id);
            var entity = new SolicitudAyudaChatMensajeModel
            {
                IDChat = chat.IDChat,
                IDUsuario = request.UserId,
                Mensaje = request.Message.Trim()
            };

            await _chatMsgService.CreateAsync(entity);

            // Flag de lectura para el emisor
            entity.LeidoPorUsuarioActual = true;

            var response = _mapper.Map<ChatMessageResponse>(entity, opt => opt.Items["UserId"] = request.UserId);
            response.SentByMe = true;
            return Ok(response);
        }

        [HttpPost("{id:int}/Chat/Leido")]
        public async Task<IActionResult> MarkChatAsRead([FromRoute] int id, [FromBody] MarkChatReadRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.UserId))
                return BadRequest("Datos inválidos");

            var chat = await _chatService.GetOrCreateBySolicitudAsync(id);
            int affected = 0;

            if (request.UpToUtc.HasValue)
            {
                affected = await _chatMsgService.MarkAllAsReadUpToAsync(chat.IDChat, request.UserId, request.UpToUtc.Value);
            }
            else if (request.MessageIds != null && request.MessageIds.Count > 0)
            {
                affected = await _chatMsgService.MarkAsReadAsync(chat.IDChat, request.UserId, request.MessageIds);
            }

            return Ok(new SuccessfulResponse(affected >= 0));
        }

        [HttpGet("{id:int}/Chat/NoLeido")]
        public async Task<IActionResult> GetUnreadCount([FromRoute] int id, [FromQuery] string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return BadRequest("userId requerido");

            var chat = await _chatService.GetOrCreateBySolicitudAsync(id);
            var count = await _chatMsgService.CountUnreadAsync(chat.IDChat, userId);
            return Ok(new ChatUnreadCountResponse { Count = count });
        }
    }
}
