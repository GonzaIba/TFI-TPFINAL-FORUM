using Microsoft.ML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.ML.Contracts
{
    public partial interface IGenericRepositoryML<T> where T : class
    {
        /// <summary>
        /// Entrena un nuevo modelo enviando la información en una lista
        /// </summary>
        /// <param name="outputModelPath">Ubicacion del archivo modelo donde se obtiene la información. Debe ser algo parecido como: "C:\YourPath\ModelName.mlnet"</param>
        /// <param name="data">Lista de información donde va a entrenar el modelo.</param>
        void Entrenar(string outputModelPath, List<T> data);

        /// <summary>
        /// Guarda el modelo en la ubicación especificada.
        /// </summary>
        /// <param name="mlContext">The common context for all ML.NET operations.</param>
        /// <param name="model">Model to save.</param>
        /// <param name="data">IDataView used to train the model.</param>
        /// <param name="modelSavePath">File path for saving the model. Should be similar to "C:\YourPath\ModelName.mlnet.</param>
        void GuardarModelo(MLContext mlContext, ITransformer model, IDataView data, string modelSavePath);

        /// <summary>
        /// Retrain model using the pipeline generated as part of the training process.
        /// </summary>
        /// <param name="mlContext"></param>
        /// <param name="trainData"></param>
        /// <returns></returns>
        ITransformer ReentrenarModelo(IDataView trainData);
    }
}
