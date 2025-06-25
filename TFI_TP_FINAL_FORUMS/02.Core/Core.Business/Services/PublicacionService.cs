using Core.Contracts.Publishers;
using Core.Contracts.Repositories;
using Core.Contracts.Services;
using Core.Contracts.UoW;
using Core.Domain.Exceptions.BaseException;
using Core.Domain.Exceptions.BusinessExceptions;
using Core.Domain.Models;
using Core.Domain.Request;
using Core.Domain.Response;
using CrossCutting.Helpers;
using Infrastructure.ML.Contracts;
using Org.BouncyCastle.Asn1.Ocsp;
using static Infrastructure_ML.PublicacionTituloML;

namespace Core.Business.Services
{
    public class PublicacionService : GenericService<PublicacionModel>, IPublicacionService
    {
        private readonly IUsersService _usersService;
        private readonly ITextoPrediccionRepositoryML _textoPrediccionRepositoryML;
        private readonly IUnitOfWorkGateway _unitOfWorkGateway;
        private readonly IPublicacionGuardadaRepository _publicacionGuardadaRepository;
        private readonly IPublicacionVotoRepository _publicacionVotoRepository;
        private readonly IRespuestaVotoRepository _respuestaVotoRepository;
        private readonly IRespuestaRepository _respuestaRepository;
        private readonly IUsersRepository _usersRepository;
        private readonly IPublisherPublication _publicationPublisher;
        public PublicacionService(
            IUnitOfWorkForum unitOfWorkForum,
            IUnitOfWorkGateway unitOfWorkGateway,
            IUsersService usersService,
            ITextoPrediccionRepositoryML textoPrediccionRepositoryML,
            IPublisherPublication publicationPublisher
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
            _publicationPublisher = publicationPublisher;
        }

        public async Task<bool> CreatePublication(string userId, PublicacionModel publication)
        {
            try
            {
                var user = await _usersService.GetByIdAsync(userId);
                if (user == null)
                    throw new ApiForumException("No existe el usuario.");

                publication.IDUsuario = user.Id;
                publication.CreateDate = DateTime.Now;
                publication.FechaCreacion = DateTime.Now;
                publication.FechaCierre = null;
                await _repository.Insert(publication);
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
                await _unitOfWork.SaveChangesAsync();
                respuesta.Usuario = user; // Asignar el usuario a la respuesta
                //await _publicationPublisher.PublishAddAnswerAsync(respuesta);
                return respuesta;
            }
            catch (Exception)
            {
                throw new ApiForumException("Error al agregar la respuesta a la publicación.");
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

        public async Task<List<PublicacionModel>> GetPublications()
        {
            try
            {
                var result = (await _repository.Get(tracking: false, includeProperties: "EtiquetasPublicacion,EtiquetasPublicacion.Etiqueta,Respuestas,PublicacionesGuardadas")).ToList();
                if (result != null)
                {
                    foreach (var pub in result)
                    {
                        pub.Usuario = (await _usersRepository.Get(x => x.Id == pub.IDUsuario, includeProperties: "UsersForum", tracking: false)).FirstOrDefault();
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
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

        public async Task<List<PublicacionModel>> GetCreatedPublicationByUser(string userId)
        {
            try
            {
                var result = (await _repository.Get(x => x.IDUsuario == userId, tracking: false, ignoreQueryFilters: true, includeProperties: "EtiquetasPublicacion,EtiquetasPublicacion.Etiqueta,Respuestas")).ToList();
                if (result != null)
                {
                    foreach (var pub in result)
                    {
                        pub.Usuario = (await _usersRepository.Get(x => x.Id == pub.IDUsuario, includeProperties: "UsersForum", tracking: false)).FirstOrDefault();
                    }
                }
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<PublicacionModel>> GetSavedPublications(string userId)
        {
            try
            {
                var result = await _publicacionGuardadaRepository.Get(x=> x.IDUsuario == userId, tracking: false, ignoreQueryFilters: true, includeProperties: "Publicacion,Publicacion.EtiquetasPublicacion,Publicacion.EtiquetasPublicacion.Etiqueta,Publicacion.Respuestas");
                var pubs = result.Select(x => x.Publicacion).ToList();
                if (pubs != null)
                {
                    foreach (var pub in pubs)
                    {
                        pub.Usuario = (await _usersRepository.Get(x => x.Id == pub.IDUsuario, includeProperties: "UsersForum", tracking: false)).FirstOrDefault();
                    }
                }
                return pubs;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<PublicacionModel>> GetPublicationByFilter(string texto)
        {
            try
            {
                var etiquetas = GetLabels(texto);
                var result = await _repository.Get(tracking: false, ignoreQueryFilters: true, includeProperties: "EtiquetasPublicacion,EtiquetasPublicacion.Etiqueta,Respuestas");
                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<IEnumerable<string>> PredictLabel(string texto)
        {
            ModelInput modelInput = new ModelInput { Texto = texto };
            var etiquetas = await _textoPrediccionRepositoryML.PredecirEtiquetas(modelInput,5);
            return etiquetas;
        }

        public async Task<AnswerPublicationVoteResponse> UserPublicationVote(PublicationVoteRequest request)
        {
            try
            {
                var publication = (await _repository.Get(x => x.IDPublicacion == request.CodePublication, includeProperties: "PublicacionesVotos")).FirstOrDefault();
                if (publication == null)
                    return new AnswerPublicationVoteResponse(false, false);

                var vote = (await _publicacionVotoRepository.Get(x => x.IDPublicacion == request.CodePublication && x.IDUsuario == request.UserId)).FirstOrDefault();

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
            catch (Exception)
            {
                return new AnswerPublicationVoteResponse(false, false);
            }
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
            }
            else
            {
                if (DateTime.UtcNow - vote.CreateDate > TimeSpan.FromMinutes(5))
                    throw new AnswerVoteExpiredException();

                if (vote.Positivo == request.IsPositive)
                    await _respuestaVotoRepository.Delete(vote);
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

        public async Task<IEnumerable<PublicacionModel>> GetTopPublications()
        {
            var repo = _unitOfWork.GetRepository<IPublicacionRepository>();
            return await repo.GetTopPublicationsLastWeek();
        }

        public async Task<bool> DeleteAnswerByUser(DeleteAnswerRequest request)
        {
            try
            {
                var publication = (await _repository.Get(x => x.IDPublicacion == request.CodePublication, includeProperties: "Respuestas")).FirstOrDefault();
                if (publication == null)
                    throw new PublicationNotFoundException();

                var answer = (await _respuestaRepository.Get(x => x.IDRespuesta == request.AnswerCode)).FirstOrDefault();

                if (answer == null || answer?.RespuestaCorrecta == true || TimeHelper.IsExpired(TimeSpan.FromHours(1), answer?.FechaCreacion ?? DateTime.MinValue))
                    throw new CantDeleteAnswerException();

                //await _respuestaRepository.Delete(answer);
                if(publication.Respuestas.Count == 1)
                {
                    publication.Respondida = false;
                    await _repository.Update(publication);
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


        #region Metodos Busqueda de textos
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
        #endregion
    }
}
