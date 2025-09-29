using Core.Contracts.Repositories;
using Core.Contracts.Services;
using Core.Contracts.UoW;
using Core.Domain.Models;

namespace Core.Business.Services
{
    public class SolicitudAyudaChatService : GenericService<SolicitudAyudaChatModel>, ISolicitudAyudaChatService
    {
        private readonly ISolicitudAyudaChatRepository _chatRepo;
        private readonly ISolicitudAyudaRepository _solRepo;

        public SolicitudAyudaChatService(
            IUnitOfWorkForum unitOfWork
        ) : base(unitOfWork, unitOfWork.GetRepository<ISolicitudAyudaChatRepository>())
        {
            _chatRepo = unitOfWork.GetRepository<ISolicitudAyudaChatRepository>();
            _solRepo = unitOfWork.GetRepository<ISolicitudAyudaRepository>();
        }

        public async Task<SolicitudAyudaChatModel?> GetBySolicitudAsync(int idSolicitud)
        {
            var chat = (await _chatRepo.Get(c => c.IDSolicitudAyuda == idSolicitud, tracking: false)).FirstOrDefault();
            return chat;
        }

        public async Task<SolicitudAyudaChatModel> GetOrCreateBySolicitudAsync(int idSolicitud)
        {
            var chat = (await _chatRepo.Get(c => c.IDSolicitudAyuda == idSolicitud, tracking: true)).FirstOrDefault();
            if (chat != null) return chat;

            // Ensure solicitud exists
            var sol = (await _solRepo.Get(s => s.IDSolicitudAyuda == idSolicitud, tracking: false)).FirstOrDefault();
            if (sol == null) throw new ArgumentException("Solicitud no encontrada", nameof(idSolicitud));

            chat = new SolicitudAyudaChatModel
            {
                IDSolicitudAyuda = idSolicitud,
                CreateDate = DateTime.UtcNow,
                Active = true
            };
            await _chatRepo.Insert(chat);
            await _unitOfWork.SaveChangesAsync();
            return chat;
        }
    }
}
