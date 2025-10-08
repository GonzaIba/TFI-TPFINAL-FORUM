using AutoMapper;
using Core.Contracts.Publishers;
using Core.Contracts.Services;
using Core.Domain.GenericEntityClass;
using Core.Domain.Models;
using Core.Domain.Request;
using Core.Domain.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

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

        [HttpPost("CrearSolicitudAyuda")]
        public async Task<IActionResult> CreateHelpRequest([FromBody] CreateHelpRequest request)
        {
            //Validaciones...
            var result = await _solicitudAyudaService.CreateHelpRequest(request);
            return Ok(new SuccessfulResponse(result));
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

        [HttpGet("ObtenerMisSolicitudesDeAyuda")]
        public async Task<IActionResult> GetMyRequestsHelp([FromQuery] string? userId = null)
        {
            var page = await _solicitudAyudaService.GetMyRequestsHelp(userId);
            var mapped = _mapper.Map<List<RequestHelpResponse>>(page);
            return Ok(mapped);
        }

        [HttpGet]
        [Route("{id:int}/ObtenerDetalleSolicitudAyuda")]
        public async Task<IActionResult> GetDetailRequestsHelp([FromRoute] int id, [FromQuery] string userId)
        {
            var requestAndChat = await _solicitudAyudaService.GetDetailRequestsHelp(id, userId);
            if (requestAndChat.Item1 == null) return NotFound();
            var dto = _mapper.Map<RequestHelpResponse>(requestAndChat.Item1, opt => { opt.Items["UserId"] = userId; });
            var response = new RequestHelpDetailResponse
            {
                RequestHelp = dto,
                CodeChat = requestAndChat.Item2,
                IsOwner = requestAndChat.Item1.IDUsuarioSolicitante.Equals(userId, StringComparison.OrdinalIgnoreCase)
            };
            return Ok(response);
        }

        [HttpGet("{id:int}/ObtenerChatsDeMiSolicitud")]
        public async Task<IActionResult> GetMyHelpRequestChats([FromRoute] int id, [FromQuery] string userId)
        {
            var chats = await _chatService.ListMyHelpRequestChatsAsync(userId, id);
            var mapped = _mapper.Map<IEnumerable<HelpRequestChatsResponse>>(chats, opt => opt.Items["UserId"] = userId);
            return Ok(mapped);
        }

        [HttpGet("{id:int}/Chat/Mensajes")]
        public async Task<IActionResult> GetChatMessages(
            [FromRoute] int id,
            [FromQuery] string userId,
            [FromQuery] int chatId)
        {
            var chat = await _chatService.GetRequestHelpChatAsync(id, userId, chatId);

            if (chat == null) return NotFound();

            // Validación de participación (por seguridad, aunque el service ya valida)
            //var isParticipant = chat.Participantes.Any(p =>
            //    p.IDUsuario.Equals(userId, StringComparison.OrdinalIgnoreCase));
            //if (!isParticipant) return Forbid(); //Acá vamos a devolver una excepcion de que no tiene permisos para ver este chat

            var dto = _mapper.Map<HelpRequestChatDetailResponse>(chat, opt => { opt.Items["UserId"] = userId;});

            return Ok(dto);
        }

        [HttpPost("{id:int}/Chat/Crear")]
        public async Task<IActionResult> CreateChat(
            [FromRoute] int id,
            [FromBody] CreateRHChatRequest request)
        {
            var chat = await _chatService.CreateChatAsync(id, request.UserId);
            return Ok(new CreateChatResponse { CodeChat = chat });
        }

        [HttpPost("{id:int}/Chat/EnviarMensaje")]
        public async Task<IActionResult> SendChatMessage([FromRoute] int id, [FromBody] SendChatMessageRequest request)
        {
            var normalizedUser = request.UserId.Trim();
            var message = request.Message.TrimStart().TrimEnd();
            if (message.Length == 0)
                return BadRequest("Mensaje vacío");

            var chat = await _chatService.GetRequestHelpChatAsync(id, normalizedUser, request.CodeChat);
            var entity = new SolicitudAyudaChatMensajeModel
            {
                IDChat = chat.IDChat,
                IDUsuario = normalizedUser,
                Mensaje = message
            };

            await _chatMsgService.CreateAsync(entity);

            entity.LeidoPorUsuarioActual = true;
            var response = _mapper.Map<ChatMessageResponse>(entity, opt => opt.Items["UserId"] = normalizedUser);
            response.Readed = false;
            response.SentByMe = true;
            return Ok(response);
        }

        [HttpPost("{id:int}/Chat/Leido")]
        public async Task<IActionResult> MarkChatAsRead([FromRoute] int id, [FromBody] MarkChatReadRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.UserId))
                return BadRequest("Datos inválidos");

            var normalizedUser = request.UserId.Trim();

            var chat = await _chatService.GetByRequestCodeAndChatCodeAsync(id, request.CodeChat);
            if (chat == null)
                return NotFound("Chat no encontrado");

            int affected = 0;

            if (request.UpToUtc.HasValue)
                affected = await _chatMsgService.MarkAllAsReadUpToAsync(chat.IDChat, normalizedUser, request.UpToUtc.Value);
            else if (request.MessageIds != null && request.MessageIds.Count > 0)
                affected = await _chatMsgService.MarkAsReadAsync(chat.IDChat, normalizedUser, request.MessageIds);

            return Ok(new SuccessfulResponse(affected >= 0));
        }

        [HttpGet("{id:int}/Chat/NoLeido")]
        public async Task<IActionResult> GetUnreadCount([FromRoute] int id, [FromQuery] string userId, [FromQuery] int codeChat)
        {
            var normalizedUser = userId.Trim();

            var chat = await _chatService.GetByRequestCodeAndChatCodeAsync(id, codeChat);
            if (chat == null)
                return NotFound("Chat no encontrado");

            var count = await _chatMsgService.CountUnreadAsync(chat.IDChat, normalizedUser);
            return Ok(new ChatUnreadCountResponse { Count = count });
        }
    }
}
