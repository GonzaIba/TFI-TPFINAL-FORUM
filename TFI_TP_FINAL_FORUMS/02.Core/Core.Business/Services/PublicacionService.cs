using Core.Contracts.Repositories;
using Core.Contracts.Services;
using Core.Contracts.UoW;
using Core.Domain.Exceptions.BaseException;
using Core.Domain.Models;
using Core.Domain.Response;
using Infrastructure.ML.Contracts;

using static Infrastructure_ML.PublicacionTituloML;

namespace Core.Business.Services
{
    public class PublicacionService : GenericService<PublicacionModel>, IPublicacionService
    {
        private readonly IUsersService _usersService;
        private readonly ITextoPrediccionRepositoryML _textoPrediccionRepositoryML;
        private readonly IUnitOfWorkGateway _unitOfWorkGateway;
        private readonly IPublicacionGuardadaRepository _publicacionGuardadaRepository;
        private readonly IUsersRepository _usersRepository;
        public PublicacionService(
            IUnitOfWorkForum unitOfWorkForum,
            IUnitOfWorkGateway unitOfWorkGateway,
            IUsersService usersService,
            ITextoPrediccionRepositoryML textoPrediccionRepositoryML
            )
        : base(unitOfWorkForum, unitOfWorkForum.GetRepository<IPublicacionRepository>())
        {
            _usersService = usersService;
            _textoPrediccionRepositoryML = textoPrediccionRepositoryML;
            _unitOfWorkGateway = unitOfWorkGateway;
            _publicacionGuardadaRepository = _unitOfWork.GetRepository<IPublicacionGuardadaRepository>();
            _usersRepository = _unitOfWorkGateway.GetRepository<IUsersRepository>();
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
                if (await _unitOfWork.Complete())
                    return true;
                else
                    return false;
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

        public async Task<IEnumerable<PublicacionModel>> GetPublications()
        {
            try
            {
                var result = await _repository.Get(tracking: false, ignoreQueryFilters: true, includeProperties: "EtiquetasPublicacion,EtiquetasPublicacion.Etiqueta,Respuestas,PublicacionesGuardadas");
                return result;
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
                var result = (await _repository.Get(x=> x.IDPublicacion == codePublication, tracking: false, ignoreQueryFilters: true, includeProperties: "EtiquetasPublicacion,EtiquetasPublicacion.Etiqueta,Respuestas,Respuestas.RespuestasVotos,PublicacionesVotos")).FirstOrDefault();
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

        public async Task<IEnumerable<PublicacionModel>> GetSavedPublications(string userId)
        {
            try
            {
                var result = await _publicacionGuardadaRepository.Get(x=> x.IDUsuario == userId, tracking: false, ignoreQueryFilters: true, includeProperties: "Publicacion,Publicacion.EtiquetasPublicacion,Publicacion.EtiquetasPublicacion.Etiqueta,Publicacion.Respuestas");
                return result.Select(x=> x.Publicacion);
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
