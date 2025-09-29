using Core.Contracts.Repositories;
using Core.Contracts.Services;
using Core.Contracts.UoW;
using Core.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Core.Business.Services
{
    public class SolicitudAyudaChatMensajeService : GenericService<SolicitudAyudaChatMensajeModel>, ISolicitudAyudaChatMensajeService
    {
        private readonly ISolicitudAyudaChatMensajeRepository _msgRepo;
        private readonly ISolicitudAyudaChatMensajeLecturaRepository _readRepo;

        public SolicitudAyudaChatMensajeService(
            IUnitOfWorkForum unitOfWork
        ) : base(unitOfWork, unitOfWork.GetRepository<ISolicitudAyudaChatMensajeRepository>())
        {
            _msgRepo = unitOfWork.GetRepository<ISolicitudAyudaChatMensajeRepository>();
            _readRepo = unitOfWork.GetRepository<ISolicitudAyudaChatMensajeLecturaRepository>();
        }

        public async Task<IReadOnlyList<SolicitudAyudaChatMensajeModel>> GetMessagesAsync(int idChat, DateTime? afterUtc, int take, bool ascending, string currentUserId)
        {
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
                var read = await _readRepo.Get(r => r.IDUsuario == currentUserId && ids.Contains(r.IDMensaje), tracking: false);
                var readSet = read.Select(r => r.IDMensaje).ToHashSet();
                foreach (var m in list)
                    m.LeidoPorUsuarioActual = readSet.Contains(m.IDMensaje);
            }

            return list;
        }

        public async Task<int> MarkAsReadAsync(int idChat, string userId, IEnumerable<int> messageIds)
        {
            var ids = messageIds?.Distinct().ToList() ?? new List<int>();
            if (ids.Count == 0) return 0;

            // Filter to ensure they belong to chat and are not own messages
            var msgs = _msgRepo.Query(m => m.IDChat == idChat && ids.Contains(m.IDMensaje) && m.IDUsuario != userId, tracking: false)
                               .Select(m => m.IDMensaje)
                               .ToList();

            if (msgs.Count == 0) return 0;

            var already = await _readRepo.Get(r => r.IDUsuario == userId && msgs.Contains(r.IDMensaje), tracking: false);
            var alreadySet = already.Select(r => r.IDMensaje).ToHashSet();

            var toInsert = msgs.Where(id => !alreadySet.Contains(id))
                               .Select(id => new SolicitudAyudaChatMensajeLecturaModel
                               {
                                   IDMensaje = id,
                                   IDUsuario = userId,
                               })
                               .ToList();

            if (toInsert.Count > 0)
            {
                await _readRepo.Insert(toInsert);
                await _unitOfWork.SaveChangesAsync();
            }

            return toInsert.Count;
        }

        public async Task<int> MarkAllAsReadUpToAsync(int idChat, string userId, DateTime upToUtc)
        {
            var msgs = _msgRepo.Query(m => m.IDChat == idChat && m.IDUsuario != userId && m.CreateDate <= upToUtc, tracking: false)
                               .Select(m => m.IDMensaje)
                               .ToList();

            return await MarkAsReadAsync(idChat, userId, msgs);
        }

        public async Task<int> CountUnreadAsync(int idChat, string userId)
        {
            // Count messages from others without a read receipt for this user
            var q = from m in _msgRepo.Query(m => m.IDChat == idChat && m.IDUsuario != userId, tracking: false)
                    join r in _readRepo.Query(r => r.IDUsuario == userId, tracking: false)
                        on m.IDMensaje equals r.IDMensaje into gj
                    from r in gj.DefaultIfEmpty()
                    where r == null
                    select m.IDMensaje;

            return q.Count();
        }
    }
}
