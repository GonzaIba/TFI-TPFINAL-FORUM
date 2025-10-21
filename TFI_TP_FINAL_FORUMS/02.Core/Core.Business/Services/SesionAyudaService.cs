using Core.Contracts.Repositories;
using Core.Contracts.Services;
using Core.Contracts.UoW;
using Core.Domain.Exceptions.BaseException;
using Core.Domain.Models;
using Core.Domain.Request;
using Core.Domain.Response;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Business.Services
{
    public class SesionAyudaService : GenericService<SesionAyudaModel>, ISesionAyudaService
    {
        private readonly IUnitOfWorkGateway _unitOfWorkGateway;
        private readonly IUnitOfWorkForum _unitOfWorkForum;
        private readonly ISolicitudAyudaRepository _solicitudAyudaRepository;
        public SesionAyudaService(
            IUnitOfWorkForum unitOfWork, 
            IUnitOfWorkGateway unitOfWorkGateway
        ) : base(unitOfWork, unitOfWork.GetRepository<ISesionAyudaRepository>())
        {
            _unitOfWorkGateway = unitOfWorkGateway;
            _unitOfWorkForum = unitOfWork;
            _solicitudAyudaRepository = unitOfWork.GetRepository<ISolicitudAyudaRepository>();
        }

        public async Task<bool> AcceptTyC(AcceptTyCRequest request)
        {
            var user = (await _unitOfWorkGateway.GetRepository<IUsersRepository>().Get(x => x.Id == request.UserId)).FirstOrDefault();
            if (user == null)
                throw new Exception("User not found");

            var requestHelp = (await _solicitudAyudaRepository.Get(x=> x.IDSolicitudAyuda == request.CodeRequestHelp, includeProperties: "SolicitudAyudaEstado,Disponibilidades,Disponibilidades.Reservas")).FirstOrDefault();
            if (requestHelp == null)
                throw new ApiForumException("Request Help not found");

            var estadoRepo = _unitOfWorkForum.GetRepository<ISolicitudAyudaEstadoRepository>();
            var estadoActiva = (await estadoRepo.Get(x => x.Estado == "Reservada", tracking: true)).First();
            if (requestHelp.SolicitudAyudaEstado != estadoActiva)
                throw new ApiForumException("The help request is not active");

            //Obtenemos la reserva activa. Reserva es iCollection para mantener historial de reservas canceladas en tal caso.
            var reserva = requestHelp.Disponibilidades.First(x=> x.Reservas.Any(y=> y.Active)).Reservas.First(z=> z.Active);
            if (reserva == null)
                throw new ApiForumException("No active reservation found for the help request");

            var session = (await _unitOfWork.GetRepository<ISesionAyudaRepository>().Get(x => x.IDReserva == reserva.IDReserva)).FirstOrDefault();
            if(session == null)
                throw new ApiForumException("Help session not found for the active reservation");

            if(requestHelp.IDUsuarioSolicitante != request.UserId && reserva.IDUsuarioAyudante != request.UserId)
                throw new ApiForumException("User is not authorized to accept the terms and conditions for this help session");

            var tycRepo = _unitOfWork.GetRepository<ITerminosCondicionesRepository>();
            var tycUserRepo = _unitOfWork.GetRepository<ITerminosCondicionesSesionAyudaRepository>();
            var lastTyC = (await tycRepo.Get()).OrderByDescending(x => x.Version).FirstOrDefault();

            if ((await tycUserRepo.Get(x => x.IDSesion == session.IDSesion && x.UserId == request.UserId)).Any())
                return true;

            TerminosCondicionesSesionAyudaModel terminosCondicionesSesionAyudaModel = new TerminosCondicionesSesionAyudaModel
            {
                UserId = request.UserId,
                IDSesion = session.IDSesion,
                Aceptado = true,
                FechaAceptado = DateTime.UtcNow,
                IdTyC = lastTyC.Id,
                Version = lastTyC.Version
            };
            await _unitOfWork.GetRepository<ITerminosCondicionesSesionAyudaRepository>().Insert(terminosCondicionesSesionAyudaModel);
            return await _unitOfWork.Complete();
        }

        public async Task<TerminosCondicionesModel> GetTyC()
        {
            var tycRepo = _unitOfWork.GetRepository<ITerminosCondicionesRepository>();
            var lastTyC = (await tycRepo.Get()).OrderByDescending(x => x.Version).FirstOrDefault();
            return lastTyC;
        }

        public async Task<SesionAyudaModel> GetSession(int codeRequestHelp, string userId)
        {
            var user = (await _unitOfWorkGateway.GetRepository<IUsersRepository>().Get(x => x.Id == userId)).FirstOrDefault();
            if (user == null)
                throw new Exception("User not found");

            var requestHelp = (await _solicitudAyudaRepository.Get(x => x.IDSolicitudAyuda == codeRequestHelp, includeProperties: "SolicitudAyudaEstado,Disponibilidades,Disponibilidades.Reservas")).FirstOrDefault();
            if (requestHelp == null)
                throw new ApiForumException("Request Help not found");

            var estadoRepo = _unitOfWorkForum.GetRepository<ISolicitudAyudaEstadoRepository>();
            var estadoActiva = (await estadoRepo.Get(x => x.Estado == "Reservada", tracking: true)).First();
            if (requestHelp.SolicitudAyudaEstado != estadoActiva)
                throw new ApiForumException("The help request is not active");

            //Obtenemos la reserva activa. Reserva es iCollection para mantener historial de reservas canceladas en tal caso.
            var reserva = requestHelp.Disponibilidades.First(x => x.Reservas.Any(y => y.Active)).Reservas.First(z => z.Active);
            if (reserva == null)
                throw new ApiForumException("No active reservation found for the help request");

            var session = (await _unitOfWork.GetRepository<ISesionAyudaRepository>().Get(x => x.IDReserva == reserva.IDReserva)).FirstOrDefault();
            if (session == null)
                throw new ApiForumException("Help session not found for the active reservation");

            if (requestHelp.IDUsuarioSolicitante != userId && reserva.IDUsuarioAyudante != userId)
                throw new ApiForumException("User is not authorized to access this help session");

            return session;
        }

        public async Task<SesionAyudaModel> EnterSession(EnterSessionRequest request)
        {
            var user = (await _unitOfWorkGateway.GetRepository<IUsersRepository>().Get(x => x.Id == request.UserId)).FirstOrDefault();
            if (user == null)
                throw new Exception("User not found");

            var session = (await _unitOfWork.GetRepository<ISesionAyudaRepository>().Get(x => x.IDSesion == request.CodeSession, includeProperties: "Reserva,Reserva.Disponibilidad,Disponibilidades.Reservas.Solicitud")).FirstOrDefault();
            if (session == null)
                throw new ApiForumException("Help session not found for the active reservation");

            var requestHelp = session.Reserva.Disponibilidad.Solicitud;
            if (requestHelp.IDUsuarioSolicitante != request.UserId && session.Reserva.IDUsuarioAyudante != request.UserId)
                throw new ApiForumException("User is not authorized to access this help session");

            return session;
        }
    }
}
