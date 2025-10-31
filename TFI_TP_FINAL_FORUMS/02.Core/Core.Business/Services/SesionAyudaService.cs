using Core.Contracts.Repositories;
using Core.Contracts.Services;
using Core.Contracts.UoW;
using Core.Domain.Exceptions.BaseException;
using Core.Domain.Exceptions.BusinessExceptions;
using Core.Domain.Exceptions.GenericExceptions;
using Core.Domain.GenericEntityClass;
using Core.Domain.Models;
using Core.Domain.Request;
using Core.Domain.Response;
using CrossCutting.Helpers;
using Microsoft.Extensions.Options;
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
        private readonly IJaasTokenService _jaas;
        private readonly IOptions<JaasOptions> _jaasOpts;

        public SesionAyudaService(
            IUnitOfWorkForum unitOfWork, 
            IUnitOfWorkGateway unitOfWorkGateway,
            IJaasTokenService jaas,
            IOptions<JaasOptions> jaasOpts
        ) : base(unitOfWork, unitOfWork.GetRepository<ISesionAyudaRepository>())
        {
            _unitOfWorkGateway = unitOfWorkGateway;
            _unitOfWorkForum = unitOfWork;
            _solicitudAyudaRepository = unitOfWork.GetRepository<ISolicitudAyudaRepository>();
            _jaas = jaas;
            _jaasOpts = jaasOpts;
        }

        public async Task<bool> AcceptTyC(AcceptTyCRequest request)
        {
            var user = (await _unitOfWorkGateway.GetRepository<IUsersRepository>().Get(x => x.Id == request.UserId)).FirstOrDefault();
            if (user == null)
                throw new UserNotFoundException();

            var requestHelp = (await _solicitudAyudaRepository.Get(x=> x.IDSolicitudAyuda == request.CodeRequestHelp, includeProperties: "SolicitudAyudaEstado,Disponibilidades,Disponibilidades.Reservas")).FirstOrDefault();
            if (requestHelp == null)
                throw new RequestHelpCantAccessException();

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
                throw new UserNotFoundException();

            var requestHelp = (await _solicitudAyudaRepository.Get(x => x.IDSolicitudAyuda == codeRequestHelp, includeProperties: "SolicitudAyudaEstado,Disponibilidades,Disponibilidades.Reservas")).FirstOrDefault();
            if (requestHelp == null)
                throw new RequestHelpCantAccessException();

            var estadoRepo = _unitOfWorkForum.GetRepository<ISolicitudAyudaEstadoRepository>();
            var estadoReservada = (await estadoRepo.Get(x => x.Estado == "Reservada", tracking: true)).First();
            if (requestHelp.SolicitudAyudaEstado != estadoReservada)
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

        public async Task<SessionResponse> EnterSession(EnterSessionRequest request)
        {
            var user = (await _unitOfWorkGateway.GetRepository<IUsersRepository>()
                .Get(x => x.Id == request.UserId, includeProperties: "UsersForum"))
                .FirstOrDefault();
            if (user == null)
                throw new UserNotFoundException();

            var session = (await _unitOfWork.GetRepository<ISesionAyudaRepository>()
                .Get(x => x.IDSesion == request.CodeSession,
                     includeProperties: "Reserva,Reserva.Disponibilidad,Reserva.Disponibilidad.Solicitud"))
                .FirstOrDefault();
            if (session == null)
                throw new ApiForumException("Help session not found for the active reservation");

            var req = session.Reserva.Disponibilidad.Solicitud;
            var isOwner = req.IDUsuarioSolicitante == request.UserId
                       || session.Reserva.IDUsuarioAyudante == request.UserId;

            // Ventana tomada de la disponibilidad (asumo horario local almacenado)
            // Ya los tenés en UTC
            var initAt = TimeHelper.EnsureUtc(session.Reserva.Disponibilidad.Inicio);
            var expiresAt = TimeHelper.EnsureUtc(session.Reserva.Disponibilidad.Fin);

            // nbf: no antes de ahora-5s, ni más de 2 min antes del inicio
            var now = DateTimeOffset.UtcNow;
            var nbf = (now > new DateTimeOffset(initAt).AddMinutes(-2) ? now : new DateTimeOffset(initAt)).AddSeconds(-5);

            // exp: la hora de fin real
            var exp = new DateTimeOffset(expiresAt);

            // Garantía mínima: exp > nbf
            if (exp <= nbf)
                exp = nbf.AddMinutes(1);

            var appId = _jaasOpts.Value.AppId;
            var serverUrl = _jaasOpts.Value.ServerUrl;

            // unificamos minúsculas
            var roomName = $"livehelp-{session.IDSesion:D}";
            var roomFull = $"{appId}/{roomName}";

            var role = isOwner ? "moderator" : "participant";

            // JWT
            var displayName = string.Join(" ", new[] { user.FirstName, user.LastName }
                                          .Where(s => !string.IsNullOrWhiteSpace(s))).Trim();
            if (string.IsNullOrWhiteSpace(displayName))
                displayName = user.UserName ?? "Usuario";

            //var avatar = user.UsersForum?.ImageForum;
            var avatar = "";

            var jwt = _jaas.CreateToken(
                appId: appId,
                room: roomName,
                notBefore: nbf,
                expiresAt: exp,
                userId: user.Id,
                displayName: displayName,
                email: user.Email,
                avatarUrl: avatar,
                isModerator: isOwner
            );

            var shouldCloseAt = exp.AddSeconds(_jaasOpts.Value.GraceSeconds).UtcDateTime; // Z

            return new SessionResponse
            {
                CodeSession = session.IDSesion.ToString(),
                Domain = $"liveHelp/meeting/{session.IDSesion:D}",
                RoomName = roomName,
                InitAt = initAt,
                ExpiresAt = expiresAt,
                IsOwner = isOwner,

                Provider = "jaas",
                AppId = appId,
                Room = roomFull,
                Jwt = jwt,
                ServerUrl = serverUrl,
                Role = role,
                ShouldCloseAt = shouldCloseAt,

                Ui = new UiSettings
                {
                    DisplayName = displayName,
                    AvatarUrl = avatar,
                    StartWithAudioMuted = true,
                    StartWithVideoMuted = true
                }
            };
        }

    }
}
