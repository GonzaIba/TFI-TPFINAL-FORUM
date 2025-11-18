using Core.Contracts.Publishers;
using Core.Contracts.Repositories;
using Core.Contracts.Services;
using Core.Contracts.UoW;
using Core.Domain.Exceptions.BaseException;
using Core.Domain.Exceptions.BusinessExceptions;
using Core.Domain.GenericEntityClass;
using Core.Domain.Models;
using Core.Domain.Request;
using Core.Domain.Response;
using Core.Domain.Specification;
using Core.Domain.Specification.Business;
using CrossCutting.Helpers;
using Infrastructure.ML.Contracts;
using System.Text.RegularExpressions;
using static Infrastructure_ML.PublicacionTituloML;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Core.Business.Services
{
    public class PublicacionService : GenericService<PublicacionModel>, IPublicacionService
    {
        private readonly IUsersService _usersService;
        private readonly ITextoPrediccionRepositoryML _textoPrediccionRepositoryML;
        private readonly ITextoPrediccionRepository _textoPrediccionRepository;
        private readonly IUnitOfWorkGateway _unitOfWorkGateway;
        private readonly IPublicacionGuardadaRepository _publicacionGuardadaRepository;
        private readonly IPublicacionVotoRepository _publicacionVotoRepository;
        private readonly IRespuestaVotoRepository _respuestaVotoRepository;
        private readonly IRespuestaRepository _respuestaRepository;
        private readonly IDenunciaRepository _denunciaRepository;
        private readonly IUsersRepository _usersRepository;
        private readonly IPublisherPublication _publicationPublisher;
        private readonly IPublisherNotification _publisherNotification;

        public PublicacionService(
            IUnitOfWorkForum unitOfWorkForum,
            IUnitOfWorkGateway unitOfWorkGateway,
            IUsersService usersService,
            ITextoPrediccionRepositoryML textoPrediccionRepositoryML,
            IPublisherPublication publicationPublisher,
            IPublisherNotification publisherNotification
        )
        : base(unitOfWorkForum, unitOfWorkForum.GetRepository<IPublicacionRepository>())
        {
            _usersService = usersService;
            _textoPrediccionRepositoryML = textoPrediccionRepositoryML;
            _unitOfWorkGateway = unitOfWorkGateway;
            _publicacionGuardadaRepository = _unitOfWork.GetRepository<IPublicacionGuardadaRepository>();
            _usersRepository = _unitOfWorkGateway.GetRepository<IUsersRepository>();
            _publicacionVotoRepository = _unitOfWork.GetRepository<IPublicacionVotoRepository>();
            _respuestaVotoRepository = _unitOfWork.GetRepository<IRespuestaVotoRepository>();
            _respuestaRepository = _unitOfWork.GetRepository<IRespuestaRepository>();
            _denunciaRepository = _unitOfWork.GetRepository<IDenunciaRepository>();
            _textoPrediccionRepository = _unitOfWork.GetRepository<ITextoPrediccionRepository>();
            _publicationPublisher = publicationPublisher;
            _publisherNotification = publisherNotification;
        }

        public async Task<bool> CreatePublication(CreatePublicationRequest request)
        {
            try
            {
                var user = await _usersService.GetByIdAsync(request.UserId);
                if (user == null)
                    throw new ApiForumException("No existe el usuario.");

                PublicacionModel publication = new();
                publication.IDUsuario = user.Id;
                publication.CreateDate = DateTime.Now;
                publication.FechaCreacion = DateTime.Now;
                publication.FechaCierre = null;
                publication.Titulo = request.Title;
                publication.Contenido = request.Content;
                publication.Cerrada = false;
                publication.Respondida = false;
                //publication.Active = true;
                await _repository.Insert(publication);

                TextoPrediccionModel textoPrediccionModel = new TextoPrediccionModel
                {
                    Texto = request.Title,
                    Etiquetas = string.Join(", ",request.Labels),
                };

                await _textoPrediccionRepository.Insert(textoPrediccionModel);
                return await _unitOfWork.Complete();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> EditPublication(string userId, EditPublicationRequest editPublication)
        {
            try
            {
                var user = await _usersService.GetByIdAsync(userId);
                if (user == null)
                    throw new ApiForumException("No existe el usuario.");

                var publication = (await _repository.Get(x => x.IDPublicacion == editPublication.CodePublication)).FirstOrDefault();
                if (publication == null)
                    throw new PublicationNotFoundException();

                if (publication.Cerrada)
                    throw new ApiForumException("La publicación ya se encuentra cerrada.");

                publication.Contenido = editPublication.Contenido;
                await _repository.Update(publication);
                await _unitOfWork.SaveChangesAsync();
                await _publicationPublisher.PublishEditPublicationAsync(publication.Contenido, editPublication.ConnectionId, publication.IDPublicacion);
                return true;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<bool> EditAnswer(string userId, EditAnswerRequest editAnswer)
        {
            try
            {
                var user = await _usersService.GetByIdAsync(userId);
                if (user == null)
                    throw new ApiForumException("No existe el usuario.");

                var publication = (await _repository.Get(x => x.IDPublicacion == editAnswer.CodePublication, tracking: false)).FirstOrDefault();
                if (publication == null)
                    throw new PublicationNotFoundException();

                var answer = (await _respuestaRepository.Get(x => x.IDRespuesta == editAnswer.AnswerCode)).FirstOrDefault();
                if (answer == null)
                    throw new PublicationNotFoundException();

                if (publication.Cerrada)
                    throw new ApiForumException("La publicación ya se encuentra cerrada.");

                answer.TextoRespuesta = editAnswer.Contenido;
                await _respuestaRepository.Update(answer);
                await _unitOfWork.SaveChangesAsync();
                await _publicationPublisher.PublishEditAnswerAsync(answer.IDRespuesta, answer.TextoRespuesta, editAnswer.ConnectionId, publication.IDPublicacion);
                return true;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<RespuestaModel> AddAnswer(AddAnswerRequest request)
        {
            try
            {
                var user = await _usersService.GetByIdAsync(request.UserId);
                if (user == null)
                    throw new ApiForumException("No existe el usuario.");

                var publication = (await _repository.Get(x => x.IDPublicacion == request.CodePublication)).FirstOrDefault();
                if (publication == null)
                    throw new PublicationNotFoundException();

                if (publication.Cerrada)
                    throw new ApiForumException("La publicación ya se encuentra cerrada.");

                //publication.Usuario = (await _usersRepository.Get(x => x.Id == publication.IDUsuario, includeProperties: "UsersForum", tracking: false)).FirstOrDefault();

                var respuesta = new RespuestaModel
                {
                    IDPublicacion = request.CodePublication,
                    IDUsuario = user.Id,
                    TextoRespuesta = request.TextResponse,
                    FechaCreacion = DateTime.Now,
                    RespuestaCorrecta = false,
                    Active = true
                };
                await _respuestaRepository.Insert(respuesta);
                if (!publication.Respondida)
                {
                    publication.Respondida = true;
                    //publication.FechaCierre = DateTime.Now.AddDays(7); // Asignar fecha de cierre 7 días después de la respuesta
                    await _repository.Update(publication);
                }
                respuesta.Usuario = user; // Asignar el usuario a la respuesta

                if(request.UserId != publication.IDUsuario)
                {
                    NotificacionesModel notificacionesModel = new NotificacionesModel
                    {
                        Mensaje = $"El usuario {(user is null ? "Desconocido" : (user.FirstName + " " + user.LastName))} ha respondido a tu publicación {publication.IDPublicacion}.",
                        IDUsuario = publication.IDUsuario, // Le notificamos al autor de la publicación
                        FechaNotificacion = DateTime.Now,
                        Leida = false
                    };
                    await _unitOfWork.GetRepository<INotificacionRepository>().Insert(notificacionesModel);
                    await _publisherNotification.AddNotification(
                        notificacionesModel.IDNotificacion, 
                        notificacionesModel.IDUsuario, 
                        notificacionesModel.Mensaje, 
                        notificacionesModel.FechaNotificacion,
                        notificacionesModel.Leida
                    );
                }
                await _unitOfWork.SaveChangesAsync();

                //await _publicationPublisher.PublishAddAnswerAsync(respuesta);
                return respuesta;
            }
            catch (Exception)
            {
                throw new ApiForumException("Error al agregar la respuesta a la publicación.");
            }
        }

        public async Task<bool> DeleteAnswerByUser(DeleteAnswerRequest request) //Aclaramos byUser porque puede un admin forzar un delete de publication.
        {
            try
            {
                var publication = (await _repository.Get(x => x.IDPublicacion == request.CodePublication, includeProperties: "Respuestas")).FirstOrDefault();
                if (publication == null)
                    throw new PublicationNotFoundException();

                var answer = (await _respuestaRepository.Get(x => x.IDRespuesta == request.AnswerCode)).FirstOrDefault();

                if (answer == null || answer?.RespuestaCorrecta == true || TimeHelper.IsExpired(TimeSpan.FromHours(1), answer?.FechaCreacion ?? DateTime.MinValue))
                    throw new CantDeleteAnswerException();

                var notifRepo = _unitOfWork.GetRepository<INotificacionRepository>();

                //await _respuestaRepository.Delete(answer);
                if (publication.Respuestas.Count == 1)
                {
                    publication.Respondida = false;
                    await _repository.Update(publication);
                }

                if (publication.IDUsuario != request.UserId)
                {
                    var notificationModel = (await notifRepo.Get(x => x.IDUsuario == publication.IDUsuario && x.Mensaje.Contains($"ha respondido a tu publicación"))).LastOrDefault();
                    if (notificationModel != null)
                    {
                        await notifRepo.Delete(notificationModel);
                        await _publisherNotification.RemoveNotification(notificationModel.IDNotificacion, notificationModel.IDUsuario);
                    }
                }

                answer.Active = false;
                await _respuestaRepository.Update(answer);
                await _unitOfWork.SaveChangesAsync();
                await _publicationPublisher.PublishDeleteAnswerAsync(publication.IDPublicacion, answer.IDRespuesta, request.ConnectionId);
                return true;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<bool> SavePublication(string userId, int codePublication)
        {
            try
            {
                if(!(await _publicacionGuardadaRepository.Get(x=> x.IDUsuario == userId && x.IDPublicacion == codePublication)).Any())
                {
                    await _publicacionGuardadaRepository.Insert(new PublicacionGuardadaModel() { IDPublicacion = codePublication, IDUsuario = userId });
                    await _unitOfWork.SaveChangesAsync();
                }

                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<bool> DeleteSavedPublication(string userId, int codePublication)
        {
            try
            {
                var row = (await _publicacionGuardadaRepository.Get(x => x.IDUsuario == userId && x.IDPublicacion == codePublication)).FirstOrDefault();
                if (row != null)
                {
                    await _publicacionGuardadaRepository.Delete(row);
                    await _unitOfWork.SaveChangesAsync();
                }

                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<PaginatedList<PublicacionModel>> GetPublications(int pageIndex, int pageCount)
        {
            var paged = await _repository.GetPagedElements(
                pageIndex,
                pageCount,
                orderByExpression: p => p.FechaCreacion,
                ascending: false,
                includeProperties: "EtiquetasPublicacion,EtiquetasPublicacion.Etiqueta,Respuestas,PublicacionesGuardadas",
                tracking: false
            );

            var publicaciones = paged.List.ToList();

            var ids = publicaciones.Select(p => p.IDUsuario).Distinct().ToList();
            var usuarios = (await _usersRepository
                .Get(x => ids.Contains(x.Id), includeProperties: "UsersForum", tracking: false))
                .ToDictionary(u => u.Id);

            foreach (var pub in publicaciones)
            {
                usuarios.TryGetValue(pub.IDUsuario, out var user);
                pub.Usuario = user;
            }

            paged.List = publicaciones;
            return paged;
        }


        public async Task<IEnumerable<PublicacionModel>> GetRelatedPublications(int publicationCode)
        {
            try
            {
                var repo = _unitOfWork.GetRepository<IPublicacionRepository>();
                var result = await repo.GetRelatedAsync(publicationCode);
                return result.Select(x=> x.pub);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<PublicacionModel> GetDetailPublication(int codePublication)
        {
            try
            {
                var result = (await _repository.Get(x=> x.IDPublicacion == codePublication, tracking: false, includeProperties: "EtiquetasPublicacion,EtiquetasPublicacion.Etiqueta,Respuestas,Respuestas.RespuestasVotos,PublicacionesVotos")).FirstOrDefault();
                if(result != null)
                {
                    result.Usuario = (await _usersRepository.Get(x => x.Id == result.IDUsuario, includeProperties: "UsersForum", tracking: false)).FirstOrDefault();
                    foreach (var respuesta in result.Respuestas)
                    {
                        respuesta.Usuario = (await _usersRepository.Get(x => x.Id == respuesta.IDUsuario, includeProperties: "UsersForum", tracking: false)).FirstOrDefault();
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<PaginatedList<PublicacionModel>> GetCreatedPublicationByUser(
            int pageIndex,
            int pageCount,
            string userId)
        {
            var paged = await _repository.GetPagedElements(
                pageIndex,
                pageCount,
                orderByExpression: p => p.FechaCreacion,
                filter: new PublicationUserIdSpec(userId),
                ascending: false,
                includeProperties: "EtiquetasPublicacion,EtiquetasPublicacion.Etiqueta,Respuestas",
                tracking: false
            );

            if (paged?.List?.Any() == true)
            {
                var publicaciones = paged.List.ToList();

                var ids = publicaciones
                    .Select(p => p.IDUsuario)
                    .Distinct()
                    .ToList();

                var usuarios = (await _usersRepository
                        .Get(x => ids.Contains(x.Id),
                             includeProperties: "UsersForum",
                             tracking: false))
                    .ToDictionary(u => u.Id, u => u);

                foreach (var pub in publicaciones)
                {
                    usuarios.TryGetValue(pub.IDUsuario, out var usr);
                    pub.Usuario = usr;
                }

                paged.List = publicaciones;
            }

            return paged;
        }

        public async Task<PaginatedList<PublicacionModel>> GetSavedPublications(
            int pageIndex,
            int pageCount,
            string userId
        )
        {
            var pagedGuardadas = await _publicacionGuardadaRepository.GetPagedElements(
                pageIndex,
                pageCount,
                orderByExpression: p => p.IDPublicacionGuardada,
                filter: new PublicationSavedUserIdSpec(userId),
                ascending: false,
                includeProperties: "Publicacion,Publicacion.EtiquetasPublicacion,Publicacion.EtiquetasPublicacion.Etiqueta,Publicacion.Respuestas",
                tracking: false
            );

            var publicaciones = pagedGuardadas?.List?
                .Select(x => x.Publicacion)
                .ToList()
                ?? new List<PublicacionModel>();

            if (publicaciones.Any())
            {
                var ids = publicaciones
                    .Select(p => p.IDUsuario)
                    .Distinct()
                    .ToList();

                var usuarios = (await _usersRepository
                        .Get(x => ids.Contains(x.Id),
                             includeProperties: "UsersForum",
                             tracking: false))
                    .ToDictionary(u => u.Id, u => u);

                foreach (var pub in publicaciones)
                {
                    usuarios.TryGetValue(pub.IDUsuario, out var usr);
                    pub.Usuario = usr;
                }
            }

            return new PaginatedList<PublicacionModel>(
                publicaciones,
                pagedGuardadas.PageIndex,
                pagedGuardadas.PageCount,
                pagedGuardadas.TotalCount,
                pagedGuardadas.TotalPages
            );
        }


        public async Task<PaginatedList<PublicacionModel>> GetPublicationsByFilter(
            string rawQuery,
            int pageIndex,
            int pageCount)
        {
            // 1) Lo primero que hacemos es parsear el rawQuery.
            var parsed = ParseRawQuery(rawQuery);

            // 2) Spec base = “true” para ir encadenando &
            Specification<PublicacionModel> baseSpec =
                new AdHocSpecification<PublicacionModel>(p => true);

            // 2.1) Filtrar por userName → traer userId desde el otro repositorio (ya que es otra DB)
            if (!string.IsNullOrEmpty(parsed.Filters.UserName))
            {
                var user = (await _usersRepository
                    .Get(u => u.UserName == parsed.Filters.UserName, tracking: false))
                    .FirstOrDefault();

                // Si no existe, devolvemos vacío
                if (user == null)
                    return new PaginatedList<PublicacionModel>(
                        new List<PublicacionModel>(), pageIndex, pageCount, 0, 0);

                // Si existe, filtramos por su ID
                baseSpec &= new AdHocSpecification<PublicacionModel>(
                    p => p.IDUsuario == user.Id);
            }

            // 2.2) Tags
            if (parsed.Filters.Tags.Any())
                baseSpec &= new AdHocSpecification<PublicacionModel>(p =>
                    p.EtiquetasPublicacion.Any(pt =>
                        parsed.Filters.Tags.Contains(pt.Etiqueta.NombreEtiqueta)));

            // 2.3) Fecha (exact match de día completo)
            if (!string.IsNullOrEmpty(parsed.Filters.Date)
                && DateTime.TryParse(parsed.Filters.Date, out var dt))
            {
                var start = dt.Date;
                var end = start.AddDays(1);
                baseSpec &= new AdHocSpecification<PublicacionModel>(p =>
                    p.FechaCreacion >= start && p.FechaCreacion < end);
            }

            // 2.4) MinScore
            if (parsed.Filters.MinScore.HasValue)
                baseSpec &= new AdHocSpecification<PublicacionModel>(
                    p => p.Recompensa >= parsed.Filters.MinScore.Value);

            // 3) Specs ad‐hoc para texto libre
            var titleSpec = new AdHocSpecification<PublicacionModel>(
                p => p.Titulo.Contains(parsed.Text));
            var bodySpec = new AdHocSpecification<PublicacionModel>(
                p => !p.Titulo.Contains(parsed.Text)
                  && p.Contenido.Contains(parsed.Text));

            // 4) Traigo coincidencias de título y cuerpo (cada una con su peso)
            var include =
                "EtiquetasPublicacion,EtiquetasPublicacion.Etiqueta,Respuestas,PublicacionesGuardadas,PublicacionesVotos";

            var titleMatches = (await _repository
              .Get(baseSpec & titleSpec, includeProperties: include, tracking: false))
              .Select(p => new { Entity = p, Weight = 2 })
              .ToList();

            var bodyMatches = (await _repository
              .Get(baseSpec & bodySpec, includeProperties: include, tracking: false))
              .Select(p => new { Entity = p, Weight = 1 })
              .ToList();

            // 5) Union en memoria, agrupando para no repetir la misma publicación,
            // luego ordeno por Weight y por FechaCreacion
            var all = titleMatches
              .Concat(bodyMatches)
              .GroupBy(x => x.Entity.IDPublicacion)
              .Select(g => g.OrderByDescending(x => x.Weight).First())
              .OrderByDescending(x => x.Weight)
              .ThenByDescending(x => x.Entity.FechaCreacion)
              .ToList();

            var totalCount = all.Count;
            int totalPages = (int)Math.Ceiling(totalCount / (double)pageCount);

            var pageItems = all
              .Skip((pageIndex - 1) * pageCount)
              .Take(pageCount)
              .Select(x => x.Entity)
              .ToList();

            // 6) Cargo en lote los usuarios desde la otra DB. Esto se hace para 
            var userIds = pageItems.Select(p => p.IDUsuario).Distinct().ToList();
            var users = await _usersRepository
              .Get(u => userIds.Contains(u.Id),
                   includeProperties: "UsersForum",
                   tracking: false);
            var dict = users.ToDictionary(u => u.Id);

            foreach (var pub in pageItems)
                if (dict.TryGetValue(pub.IDUsuario, out var usr))
                    pub.Usuario = usr;

            // 7) Empaqueto en el PaginatedList
            return new PaginatedList<PublicacionModel>(
              pageItems, pageIndex, pageCount, totalCount, totalPages);
        }

        public async Task<IEnumerable<string>> PredictLabel(string texto)
        {
            ModelInput modelInput = new ModelInput { Texto = texto };
            var etiquetas = await _textoPrediccionRepositoryML.PredecirEtiquetas(modelInput,5);
            return etiquetas;
        }

        public async Task<AnswerPublicationVoteResponse> UserPublicationVote(PublicationVoteRequest request)
        {
            var publication = (await _repository.Get(x => x.IDPublicacion == request.CodePublication, includeProperties: "PublicacionesVotos")).FirstOrDefault();
            if (publication == null)
                return new AnswerPublicationVoteResponse(false, false);

            var vote = (await _publicacionVotoRepository.Get(x => x.IDPublicacion == request.CodePublication && x.IDUsuario == request.UserId)).FirstOrDefault();
            var notifRepo = _unitOfWork.GetRepository<INotificacionRepository>();

            // Crear nuevo voto si no existe
            if (vote == null)
            {
                var newVote = new PublicacionVotoModel
                {
                    IDPublicacion = request.CodePublication,
                    IDUsuario = request.UserId,
                    Positivo = request.IsPositive,
                    CreateDate = DateTime.UtcNow
                };

                await _publicacionVotoRepository.Insert(newVote);
                publication.Recompensa += request.IsPositive ? 10 : -10;

                var userCreator = (await _usersRepository.Get(x => x.Id == publication.IDUsuario, includeProperties: "UsersForum", tracking: false)).FirstOrDefault();

                NotificacionesModel notificacionesModel = new();
                notificacionesModel.Mensaje = $"El usuario {(userCreator is null ? "Desconocido" : (userCreator.FirstName + " " + userCreator.LastName))} ha votado tu publicación  como {(request.IsPositive ? "positivo" : "negativo")}.";
                notificacionesModel.IDUsuario = publication.IDUsuario; // Asignar al usuario de la publicación
                notificacionesModel.FechaNotificacion = DateTime.Now;
                notificacionesModel.Leida = false;
                await notifRepo.Insert(notificacionesModel);
                await _publisherNotification.AddNotification(
                    notificacionesModel.IDNotificacion, 
                    notificacionesModel.IDUsuario, 
                    notificacionesModel.Mensaje, 
                    notificacionesModel.FechaNotificacion, 
                    notificacionesModel.Leida
                );
            }
            else
            {
                // Verificar si ya expiró el voto
                if (DateTime.UtcNow - vote.CreateDate > TimeSpan.FromMinutes(5))
                    throw new PublicationVoteExpiredException();

                // Si quiere deshacer el voto (mismo valor)
                if (vote.Positivo == request.IsPositive)
                {
                    await _publicacionVotoRepository.Delete(vote);
                    publication.Recompensa -= request.IsPositive ? 10 : -10;
                    var notificationModel = (await notifRepo.Get(x => x.IDUsuario == publication.IDUsuario && x.Mensaje.Contains($"ha votado tu publicación"))).LastOrDefault();
                    if (notificationModel != null)
                    {
                        await notifRepo.Delete(notificationModel);
                        await _publisherNotification.RemoveNotification(notificationModel.IDNotificacion, notificationModel.IDUsuario);
                    }
                }
                else
                {
                    // Cambió de positivo a negativo o viceversa
                    vote.Positivo = request.IsPositive;
                    vote.CreateDate = DateTime.UtcNow;
                    publication.Recompensa += request.IsPositive ? 20 : -20;
                }
            }

            await _repository.Update(publication);
            await _unitOfWork.SaveChangesAsync();

            // Cantidad de votos positivos - votos negativos = eso vamos a mandar
            var votosPositivos = publication.PublicacionesVotos.Count(x=> x.Positivo);
            var votosNegativos = publication.PublicacionesVotos.Count(x => !x.Positivo);
            int votos = votosPositivos - votosNegativos;
            await _publicationPublisher.PublishVotePublicationChangedAsync(publication.IDPublicacion, votos, request.ConnectionId);
            return new AnswerPublicationVoteResponse(true, false);
        }


        public async Task<AnswerPublicationVoteResponse> UserAnswerVote(AnswerVoteRequest request)
        {
            var publication = (await _repository.Get(x => x.IDPublicacion == request.CodePublication)).FirstOrDefault();
            if (publication == null)
                return new AnswerPublicationVoteResponse(false, false);

            var answer = (await _respuestaRepository.Get(x => x.IDRespuesta == request.AnswerCode, includeProperties: "RespuestasVotos")).FirstOrDefault();
            if (answer == null)
                return new AnswerPublicationVoteResponse(false, false);

            var vote = (await _respuestaVotoRepository.Get(
                x => x.IDRespuesta == request.AnswerCode && x.IDUsuario == request.UserId)).FirstOrDefault();

            var notifRepo = _unitOfWork.GetRepository<INotificacionRepository>();

            if (vote == null)
            {
                var newVote = new RespuestaVotoModel
                {
                    IDRespuesta = request.AnswerCode,
                    IDUsuario = request.UserId,
                    Positivo = request.IsPositive,
                    CreateDate = DateTime.UtcNow
                };

                await _respuestaVotoRepository.Insert(newVote);

                var userCreator = (await _usersRepository.Get(x => x.Id == answer.IDUsuario, includeProperties: "UsersForum", tracking: false)).FirstOrDefault();

                NotificacionesModel notificacionesModel = new NotificacionesModel
                {
                    Mensaje = $"El usuario {(userCreator is null ? "Desconocido" : (userCreator.FirstName + " " + userCreator.LastName))} ha votado tu respuesta como {(request.IsPositive ? "positivo" : "negativo")}",
                    IDUsuario = answer.IDUsuario, // Asignar al usuario de la respuesta
                    FechaNotificacion = DateTime.Now,
                    Leida = false
                };
                await notifRepo.Insert(notificacionesModel);
                await _publisherNotification.AddNotification(
                    notificacionesModel.IDNotificacion,
                    notificacionesModel.IDUsuario,
                    notificacionesModel.Mensaje,
                    notificacionesModel.FechaNotificacion,
                    notificacionesModel.Leida
                );
            }
            else
            {
                if (DateTime.UtcNow - vote.CreateDate > TimeSpan.FromMinutes(5))
                    throw new AnswerVoteExpiredException();

                if (vote.Positivo == request.IsPositive)
                {
                    await _respuestaVotoRepository.Delete(vote);
                    var notificationModel = (await notifRepo.Get(x => x.IDUsuario == answer.IDUsuario && x.Mensaje.Contains($"ha votado tu respuesta"))).LastOrDefault();
                    if (notificationModel != null)
                    {
                        await notifRepo.Delete(notificationModel);
                        await _publisherNotification.RemoveNotification(notificationModel.IDNotificacion, notificationModel.IDUsuario);
                    }
                }
                else
                {
                    vote.Positivo = request.IsPositive;
                    vote.CreateDate = DateTime.UtcNow;
                    await _respuestaVotoRepository.Update(vote);
                }
            }

            await _unitOfWork.SaveChangesAsync();
            // Cantidad de votos positivos - votos negativos = eso vamos a mandar
            var votosPositivos = answer.RespuestasVotos.Count(x => x.Positivo);
            var votosNegativos = answer.RespuestasVotos.Count(x => !x.Positivo);
            int votos = votosPositivos - votosNegativos;
            await _publicationPublisher.PublishVoteAnswerChangedAsync(publication.IDPublicacion, answer.IDRespuesta, votos, request.ConnectionId);

            return new AnswerPublicationVoteResponse(true, false);           
        }

        public async Task<bool> ReportPublication(ReportPublicationRequest request)
        {
            try
            {
                await ValidateReportPayloadAsync(request.UserId, request.Reason, request.Detail);

                var publication = (await _repository.Get(x => x.IDPublicacion == request.CodePublication, tracking: false)).FirstOrDefault();
                if (publication == null)
                    throw new PublicationNotFoundException();

                var report = new DenunciaModel
                {
                    IDUsuarioReporto = request.UserId,
                    IDPublicacion = publication.IDPublicacion,
                    Motivo = request.Reason.Trim(),
                    Detalle = request.Detail?.Trim(),
                    FechaDenuncia = DateTime.Now,
                    CreateDate = DateTime.Now
                };

                await _denunciaRepository.Insert(report);
                return await _unitOfWork.Complete();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> ReportAnswer(ReportAnswerRequest request)
        {
            try
            {
                await ValidateReportPayloadAsync(request.UserId, request.Reason, request.Detail);

                var answer = (await _respuestaRepository.Get(x => x.IDRespuesta == request.AnswerCode, tracking: false)).FirstOrDefault();
                if (answer == null)
                    throw new PublicationNotFoundException();

                var report = new DenunciaModel
                {
                    IDUsuarioReporto = request.UserId,
                    IDRespuesta = answer.IDRespuesta,
                    Motivo = request.Reason.Trim(),
                    Detalle = request.Detail?.Trim(),
                    FechaDenuncia = DateTime.Now,
                    CreateDate = DateTime.Now
                };

                await _denunciaRepository.Insert(report);
                return await _unitOfWork.Complete();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<PublicacionModel>> GetTopPublications()
        {
            var repo = _unitOfWork.GetRepository<IPublicacionRepository>();
            return await repo.GetTopPublicationsLastWeek();
        }

        #region Helpers
        private string GetLabels(string texto)
        {
            var etiquetas = new List<string>();
            var palabras = texto.Split(' ');
            foreach (var palabra in palabras)
            {
                if (palabra.StartsWith("["))
                {
                    etiquetas.Add(palabra);
                }
            }
            return string.Join(',', etiquetas);
        }

        private ParsedQueryRequest ParseRawQuery(string raw)
        {
            var filters = new SearchFilters();
            var text = raw;

            // etiquetas: [tag]
            foreach (Match m in Regex.Matches(text, @"\[(?<tag>[^\]]+)\]"))
            {
                filters.Tags.Add(m.Groups["tag"].Value);
                text = text.Replace(m.Value, "");
            }

            // user:username
            var mUser = Regex.Match(text, @"\buser:(?<name>\S+)\b");
            if (mUser.Success)
            {
                filters.UserName = mUser.Groups["name"].Value;
                text = text.Replace(mUser.Value, "");
            }

            // date:DD/MM/YYYY
            var mDate = Regex.Match(text, @"\bdate:(?<d>\d{1,2}/\d{1,2}/\d{4})\b");
            if (mDate.Success)
            {
                filters.Date = mDate.Groups["d"].Value;
                text = text.Replace(mDate.Value, "");
            }

            // score:N
            var mScore = Regex.Match(text, @"\bscore:(?<n>\d+)\b");
            if (mScore.Success)
            {
                filters.MinScore = int.Parse(mScore.Groups["n"].Value);
                text = text.Replace(mScore.Value, "");
            }

            // limpio espacios
            text = Regex.Replace(text, @"\s+", " ").Trim();

            return new ParsedQueryRequest
            {
                Text = text,
                Filters = filters
            };
        }

        private async Task ValidateReportPayloadAsync(string userId, string reason, string detail)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ApiForumException("Debe indicar el usuario que realiza la denuncia.");

            if (string.IsNullOrWhiteSpace(reason))
                throw new ApiForumException("Debe indicar el motivo de la denuncia.");

            if (string.IsNullOrWhiteSpace(detail))
                throw new ApiForumException("Debe detallar el motivo de la denuncia.");

            var user = await _usersService.GetByIdAsync(userId);
            if (user == null)
                throw new ApiForumException("No existe el usuario.");
        }

        //private Specification<PublicacionModel> BuildFilterSpecification(SearchFilters filters)
        //{
        //    // empezamos con “verdad” para poder ir haciendo &=
        //    Specification<PublicacionModel> spec =
        //        new AdHocSpecification<PublicacionModel>(p => true);

        //    if (!string.IsNullOrEmpty(filters.UserId))
        //        spec &= new AdHocSpecification<PublicacionModel>(p => 
        //            p.IDUsuario == filters.UserId);

        //    if (filters.Tags.Any())
        //        spec &= new AdHocSpecification<PublicacionModel>(p =>
        //            p.EtiquetasPublicacion.Any(pt => filters.Tags.Contains(pt.Etiqueta.NombreEtiqueta)));

        //    if (!string.IsNullOrEmpty(filters.Date))
        //        spec &= new AdHocSpecification<PublicacionModel>(p =>
        //            p.CreateDate == Convert.ToDateTime(filters.Date));

        //    if (filters.MinScore.HasValue)
        //        spec &= new AdHocSpecification<PublicacionModel>(p =>
        //            p.Recompensa >= filters.MinScore.Value);

        //    return spec;
        //}

        #endregion
    }
}
