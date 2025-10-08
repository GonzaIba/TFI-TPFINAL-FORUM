using Core.Contracts.Repositories;
using Core.Contracts.Services;
using Core.Contracts.UoW;
using Core.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Business.Services
{
    public class SolicitudAyudaChatMensajeService : GenericService<SolicitudAyudaChatMensajeModel>, ISolicitudAyudaChatMensajeService
    {
        private readonly ISolicitudAyudaChatMensajeRepository _msgRepo;
        private readonly ISolicitudAyudaChatMensajeLecturaRepository _readRepo;
        private readonly ISolicitudAyudaChatRepository _chatRepo;

        private static readonly StringComparison UserComparison = StringComparison.OrdinalIgnoreCase;

        public SolicitudAyudaChatMensajeService(
            IUnitOfWorkForum unitOfWork
        ) : base(unitOfWork, unitOfWork.GetRepository<ISolicitudAyudaChatMensajeRepository>())
        {
            _msgRepo = unitOfWork.GetRepository<ISolicitudAyudaChatMensajeRepository>();
            _readRepo = unitOfWork.GetRepository<ISolicitudAyudaChatMensajeLecturaRepository>();
            _chatRepo = unitOfWork.GetRepository<ISolicitudAyudaChatRepository>();
        }

        public override async Task CreateAsync(SolicitudAyudaChatMensajeModel entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            entity.IDUsuario = NormalizeUserId(entity.IDUsuario, nameof(entity.IDUsuario));
            entity.Mensaje = (entity.Mensaje ?? string.Empty).Trim();
            if (entity.Mensaje.Length == 0)
            {
                throw new ArgumentException("Mensaje requerido", nameof(entity.Mensaje));
            }

            var chat = await GetChatAsync(entity.IDChat);
            EnsureParticipant(chat, entity.IDUsuario);

            await base.CreateAsync(entity);
        }

        public async Task<IReadOnlyList<SolicitudAyudaChatMensajeModel>> GetMessagesAsync(int idChat, DateTime? afterUtc, int take, bool ascending, string currentUserId)
        {
            var normalizedUser = NormalizeUserId(currentUserId, nameof(currentUserId));
            var chat = await GetChatAsync(idChat);
            EnsureParticipant(chat, normalizedUser);

            if (take <= 0) take = 50;

            var q = _msgRepo.Query(m => m.IDChat == idChat, tracking: false);

            if (afterUtc.HasValue)
                q = q.Where(m => m.CreateDate > afterUtc.Value);

            q = ascending
                ? q.OrderBy(m => m.CreateDate).ThenBy(m => m.IDMensaje)
                : q.OrderByDescending(m => m.CreateDate).ThenByDescending(m => m.IDMensaje);

            var list = q.Take(take).ToList();

            var ids = list.Select(m => m.IDMensaje).ToList();
            if (ids.Count > 0)
            {
                var read = await _readRepo.Get(r => r.IDUsuario == normalizedUser && ids.Contains(r.IDMensaje), tracking: false);
                var readSet = read.Select(r => r.IDMensaje).ToHashSet();
                foreach (var m in list)
                    m.LeidoPorUsuarioActual = readSet.Contains(m.IDMensaje);
            }

            return list;
        }

        public async Task<int> MarkAsReadAsync(int idChat, string userId, IEnumerable<int> messageIds)
        {
            var normalizedUser = NormalizeUserId(userId, nameof(userId));
            var chat = await GetChatAsync(idChat);
            EnsureParticipant(chat, normalizedUser);

            return await MarkAsReadInternalAsync(chat, normalizedUser, messageIds);
        }

        public async Task<int> MarkAllAsReadUpToAsync(int idChat, string userId, DateTime upToUtc)
        {
            var normalizedUser = NormalizeUserId(userId, nameof(userId));
            var chat = await GetChatAsync(idChat);
            EnsureParticipant(chat, normalizedUser);

            var msgs = _msgRepo.Query(m => m.IDChat == idChat && m.IDUsuario != normalizedUser && m.CreateDate <= upToUtc, tracking: false)
                               .Select(m => m.IDMensaje)
                               .ToList();

            return await MarkAsReadInternalAsync(chat, normalizedUser, msgs);
        }

        public async Task<int> CountUnreadAsync(int idChat, string userId)
        {
            var normalizedUser = NormalizeUserId(userId, nameof(userId));
            var chat = await GetChatAsync(idChat);
            EnsureParticipant(chat, normalizedUser);

            // Count messages from others without a read receipt for this user
            var q = from m in _msgRepo.Query(m => m.IDChat == idChat && m.IDUsuario != normalizedUser, tracking: false)
                    join r in _readRepo.Query(r => r.IDUsuario == normalizedUser, tracking: false)
                        on m.IDMensaje equals r.IDMensaje into gj
                    from r in gj.DefaultIfEmpty()
                    where r == null
                    select m.IDMensaje;

            return q.Count();
        }

        private async Task<int> MarkAsReadInternalAsync(SolicitudAyudaChatModel chat, string normalizedUser, IEnumerable<int> messageIds)
        {
            var ids = messageIds?.Distinct().ToList() ?? new List<int>();
            if (ids.Count == 0)
            {
                return 0;
            }

            var msgs = _msgRepo.Query(m => m.IDChat == chat.IDChat && ids.Contains(m.IDMensaje) && m.IDUsuario != normalizedUser, tracking: false)
                               .Select(m => m.IDMensaje)
                               .ToList();

            if (msgs.Count == 0)
            {
                return 0;
            }

            var already = await _readRepo.Get(r => r.IDUsuario == normalizedUser && msgs.Contains(r.IDMensaje), tracking: false);
            var alreadySet = already.Select(r => r.IDMensaje).ToHashSet();

            var toInsert = msgs.Where(id => !alreadySet.Contains(id))
                               .Select(id => new SolicitudAyudaChatMensajeLecturaModel
                               {
                                   IDMensaje = id,
                                   IDUsuario = normalizedUser,
                               })
                               .ToList();

            if (toInsert.Count > 0)
            {
                await _readRepo.Insert(toInsert);
                await _unitOfWork.SaveChangesAsync();
            }

            return toInsert.Count;
        }

        private async Task<SolicitudAyudaChatModel> GetChatAsync(int idChat)
        {
            var chats = await _chatRepo.Get(c => c.IDChat == idChat, includeProperties: "Solicitud", tracking: false);
            var chat = chats.FirstOrDefault();
            if (chat == null)
            {
                throw new ArgumentException("Chat no encontrado", nameof(idChat));
            }

            if (chat.Solicitud == null)
            {
                throw new InvalidOperationException("La solicitud asociada al chat no se encuentra cargada.");
            }

            return chat;
        }

        private static void EnsureParticipant(SolicitudAyudaChatModel chat, string userId)
        {
            if (!string.Equals(chat.IDUsuarioAyudante, userId, UserComparison) &&
                !string.Equals(chat.Solicitud.IDUsuarioSolicitante, userId, UserComparison))
            {
                throw new InvalidOperationException("El usuario no participa del chat solicitado.");
            }
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
