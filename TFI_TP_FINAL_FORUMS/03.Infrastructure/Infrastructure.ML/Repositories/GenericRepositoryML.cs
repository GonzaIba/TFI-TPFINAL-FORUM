using Microsoft.ML;
using Infrastructure.ML.Contracts;

namespace Infrastructure.ML.Repositories
{
    public abstract class GenericRepositoryML<T> : IGenericRepositoryML<T> where T : class
    {
        public readonly IEstimator<ITransformer> _pipeline;
        public readonly MLContext _mlContext;

        public GenericRepositoryML(IEstimator<ITransformer> pipeline, MLContext mlContext)
        {
            _pipeline = pipeline;
            _mlContext = mlContext;
        }

        public byte[] TrainAndSaveAsync(List<T> data)
        {
            // 1) Carga los ejemplos a un IDataView
            IDataView trainingData = _mlContext.Data.LoadFromEnumerable(data);

            // 2) Ajusta (fit) el pipeline definido en BuildPipeline
            ITransformer modeloEntrenado = _pipeline.Fit(trainingData);

            // 3) Serializa el modelo entrenado a un MemoryStream
            using var ms = new MemoryStream();
            _mlContext.Model.Save(modeloEntrenado, trainingData.Schema, ms);
            byte[] modeloBytes = ms.ToArray();
            return modeloBytes;
        }

        public void Entrenar(string outputModelPath, List<T> data)
        {
            var dataView = _mlContext.Data.LoadFromEnumerable(data);
            var model = ReentrenarModelo(dataView);
            GuardarModelo(_mlContext, model, dataView, outputModelPath);
        }

        public void GuardarModelo(MLContext mlContext, ITransformer model, IDataView data, string modelSavePath)
        {
            // Pull the data schema from the IDataView used for training the model
            DataViewSchema dataViewSchema = data.Schema;

            using (var fs = File.Create(modelSavePath))
                _mlContext.Model.Save(model, dataViewSchema, fs);
        }
        
        public ITransformer ReentrenarModelo(IDataView trainData)
        {
            var model = _pipeline.Fit(trainData);
            return model;
        }
    }
}
