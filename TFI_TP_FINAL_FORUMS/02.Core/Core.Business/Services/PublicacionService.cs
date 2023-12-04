using Core.Contracts.Repositories;
using Core.Contracts.Services;
using Core.Domain.Exceptions.BaseException;
using Core.Domain.Models;
using Infrastructure.ML.Contracts;

using static Infrastructure_ML.PublicacionTituloML;

namespace Core.Business.Services
{
    public class PublicacionService : GenericService<PublicacionModel>, IPublicacionService
    {
        private readonly IUsersService _usersService;
        private readonly ITextoPrediccionRepositoryML _textoPrediccionRepositoryML;
        public PublicacionService(
            IUnitOfWork unitOfWork,
            IUsersService usersService,
            ITextoPrediccionRepositoryML textoPrediccionRepositoryML
            )
        : base(unitOfWork, unitOfWork.GetRepository<IPublicacionRepository>())
        {
            _usersService = usersService;
            _textoPrediccionRepositoryML = textoPrediccionRepositoryML;
        }

        public async Task<bool> CrearPublicacion(string userId, PublicacionModel publicacion)
        {
            try
            {
                var user = await _usersService.GetByIdAsync(userId);
                if (user == null)
                    throw new ApiForumException("No existe el usuario.");

                publicacion.IDUsuario = user.Id;
                publicacion.CreateDate = DateTime.Now;
                publicacion.FechaCreacion = DateTime.Now;
                publicacion.FechaCierre = null;
                await _repository.Insert(publicacion);
                if (await _unitOfWork.Complete())
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<PublicacionModel>> ObtenerPublicaciones()
        {
            try
            {
                var result = await _repository.Get(tracking: false, ignoreQueryFilters: true, includeProperties: "EtiquetasPublicacion,EtiquetasPublicacion.Etiqueta,Respuestas");
                return result;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<IEnumerable<PublicacionModel>> ObtenerPublicacionesPorFiltro(string texto)
        {
            try
            {
                var etiquetas = ObtenerEtiquetas(texto);
                var result = await _repository.Get(tracking: false, ignoreQueryFilters: true, includeProperties: "EtiquetasPublicacion,EtiquetasPublicacion.Etiqueta,Respuestas");
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<string>> PredecirEtiquetas(string texto)
        {
            ModelInput modelInput = new ModelInput { Texto = texto };
            var etiquetas = await _textoPrediccionRepositoryML.PredecirEtiquetas(modelInput,5);
            return etiquetas;
        }

        #region Metodos Busqueda de textos
        private string ObtenerEtiquetas(string texto)
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
