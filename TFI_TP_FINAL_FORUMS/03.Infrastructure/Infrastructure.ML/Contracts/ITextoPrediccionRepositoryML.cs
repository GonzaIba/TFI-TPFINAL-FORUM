using Core.Domain.Models;
using Microsoft.ML;
using static Infrastructure_ML.PublicacionTituloML;

namespace Infrastructure.ML.Contracts
{
    public interface ITextoPrediccionRepositoryML : IGenericRepositoryML<EtiquetasPrediccionModeloModel>
    {
        /// <summary>
        /// Carga el modelo de predicción desde la base de datos.
        /// </summary>
        /// <returns></returns>
        public PredictionEngine<ModelInput, ModelOutput> CargarModeloDesdeDb();

        /// <summary>
        /// Use este método para obtener mas de una etiqueta
        /// </summary>
        /// <param name="input">model input.</param>
        /// <returns><seealso cref=" ModelOutput"/></returns>
        public Task<IOrderedEnumerable<KeyValuePair<string, float>>> PredecirEtiquetasYProbabilidades(ModelInput input);

        /// <summary>
        /// Use este método para obtener un solo grupo de etiquetas <see cref="ModelInput"/>.
        /// </summary>
        /// <param name="input">model input.</param>
        /// <returns><seealso cref=" ModelOutput"/></returns>
        public Task<ModelOutput> PredecirEtiqueta(ModelInput input);

        /// <summary>
        /// Use este método para obtener todos los grupos de etiquetas <see cref="ModelInput"/>.
        /// </summary>
        /// <param name="input">model input.</param>
        /// <returns><seealso cref=" ModelOutput"/></returns>
        public Task<IEnumerable<string>> PredecirEtiquetas(ModelInput input, int cantidad);

        /// <summary>
        /// Obtiene etiquetas ordenadamente de la mas cercana a la menos las probabilidades de cada uno (grupo).
        /// </summary>
        /// <param name="result">Prediction to get the labeled scores from.</param>
        /// <returns>Ordered list of label and score.</returns>
        /// <exception cref="Exception"></exception>
        public Task<IOrderedEnumerable<KeyValuePair<string, float>>> ObtenerEtiquetasYProbabilidades(ModelOutput result);


        /// <summary>
        /// Obtiene etiquetas ordenadamente de la mas cercana a la menos.
        /// </summary>
        /// <param name="result">Predicted result to get the labels from.</param>
        /// <returns>List of labels.</returns>
        /// <exception cref="Exception"></exception>
        public Task<IEnumerable<string>> ObtenerEtiquetas(ModelOutput result);
    }
}
