using Core.Contracts.Repositories;
using Core.Contracts.Services;
using Core.Contracts.UoW;
using Core.Domain.Enum;
using Core.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Business.Services
{
    public class SolicitudAyudaChatService : GenericService<SolicitudAyudaChatModel>, ISolicitudAyudaChatService
    {
        private readonly ISolicitudAyudaChatRepository _chatRepo;
        private readonly ISolicitudAyudaRepository _solRepo;
        private readonly IUnitOfWorkGateway _unitOfWorkGateway;

        private static readonly StringComparison UserComparison = StringComparison.OrdinalIgnoreCase;

        public SolicitudAyudaChatService(
            IUnitOfWorkForum unitOfWork,
            IUnitOfWorkGateway unitOfWorkGateway
        ) : base(unitOfWork, unitOfWork.GetRepository<ISolicitudAyudaChatRepository>())
        {
            _chatRepo = unitOfWork.GetRepository<ISolicitudAyudaChatRepository>();
            _solRepo = unitOfWork.GetRepository<ISolicitudAyudaRepository>();
            _unitOfWorkGateway = unitOfWorkGateway;
        }

        public async Task<int> CreateChatAsync(int requestCode, string userId)
        {
            var requestHelp = (await _solRepo.Get(x => x.IDSolicitudAyuda == requestCode, tracking: false)).FirstOrDefault();
            if (requestHelp == null) throw new ArgumentException("No se encontró la solicitud de ayuda.", nameof(requestCode));

            // Evitar chat con uno mismo
            if (string.Equals(requestHelp.IDUsuarioSolicitante, userId, StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("El solicitante no puede iniciar un chat consigo mismo.", nameof(userId));

            // Idempotencia: si ya existe, devolvelo
            var existing = (await _chatRepo.Get(
                c => c.IDSolicitudAyuda == requestCode && c.IDUsuarioAyudante == userId,
                tracking: false)).FirstOrDefault();
            if (existing != null) return existing.IDChat;

            using var transaction = await _unitOfWork.BeginTransactionAsync();

            var chat = new SolicitudAyudaChatModel
            {
                IDSolicitudAyuda = requestCode,
                IDUsuarioAyudante = userId,
                Active = true, ///////////////////////////////////////
                CreateDate = DateTime.UtcNow
            };

            await _chatRepo.Insert(chat);
            await _unitOfWork.SaveChangesAsync();
            var participantes = new List<SolicitudAyudaChatParticipanteModel>
            {
                new SolicitudAyudaChatParticipanteModel
                {
                    IDChat = chat.IDChat,
                    IDUsuario = requestHelp.IDUsuarioSolicitante,
                    Active = true, ///////////////////////////////////////
                    CreateDate = DateTime.UtcNow,
                    Rol = (byte)ParticipantChatEnum.Solicitante
                },
                new SolicitudAyudaChatParticipanteModel
                {
                    IDChat = chat.IDChat,
                    IDUsuario = userId,
                    Active = true, ///////////////////////////////////////
                    CreateDate = DateTime.UtcNow,
                    Rol = (byte)ParticipantChatEnum.Ayudante
                }
            };

            var participantesRepo = _unitOfWork.GetRepository<ISolicitudAyudaChatParticipanteRepository>();
            await participantesRepo.Insert(participantes);
            await _unitOfWork.SaveChangesAsync();
            await transaction.CommitAsync();
            return chat.IDChat;
        }


        public async Task<SolicitudAyudaChatModel?> GetByRequestCodeAndChatCodeAsync(int codeRequest, int codeChat)
        {
            var chat = (await _chatRepo.Get(c => c.IDSolicitudAyuda == codeRequest && c.IDChat == codeChat, includeProperties: "Solicitud", tracking: false)).FirstOrDefault();
            return chat;
        }

        public async Task<SolicitudAyudaChatModel> GetRequestHelpChatAsync(int idSolicitud, string userId, int chatCode)
        {
            var normalizedUserId = NormalizeUserId(userId, nameof(userId));

            var chat = (await _chatRepo.Get(c => c.IDSolicitudAyuda == idSolicitud && c.IDChat == chatCode, includeProperties: "Solicitud,Participantes,Mensajes,Mensajes.Lecturas", tracking: false)).FirstOrDefault();
            if(chat is null)
                throw new ArgumentException("No se encontró el chat indicado para la solicitud de ayuda.", nameof(chatCode));

            chat.UsuarioAyudante = (await _unitOfWorkGateway.GetRepository<IUsersRepository>()
                .Get(u => u.Id == chat.IDUsuarioAyudante, includeProperties: "UsersForum")).FirstOrDefault();

            return chat;
        }

        public async Task<List<SolicitudAyudaChatModel>> ListMyHelpRequestChatsAsync(string userId, int idRequest)
        {
            var normalizedUser = NormalizeUserId(userId, nameof(userId));

            var chats = (await _chatRepo.Get(
                c => c.Solicitud.IDUsuarioSolicitante == normalizedUser && c.IDSolicitudAyuda == idRequest,
                orderBy: q => q.OrderByDescending(c => c.CreateDate).ThenByDescending(c => c.IDChat),
                includeProperties: "Solicitud,Mensajes,Mensajes.Lecturas,Participantes,Solicitud.SolicitudAyudaEstado",
                tracking: false)).ToList();

            // Batch lookup de ayudantes
            var helperIds = chats.Select(c => c.IDUsuarioAyudante).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
            var usersRepo = _unitOfWorkGateway.GetRepository<IUsersRepository>();
            var helpers = (await usersRepo.Get(u => helperIds.Contains(u.Id), includeProperties: "UsersForum", tracking: false))
                          .ToDictionary(u => u.Id, StringComparer.OrdinalIgnoreCase);

            foreach (var chat in chats)
                if (helpers.TryGetValue(chat.IDUsuarioAyudante, out var user))
                    chat.UsuarioAyudante = user;

            return chats;
        }

        private static string NormalizeUserId(string value, string paramName)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Valor requerido", paramName);
            }

            return value.Trim();
        }
    }
}
