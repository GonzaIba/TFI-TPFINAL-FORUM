using Core.Domain.Models;
using Microsoft.ML.Trainers;
using Microsoft.ML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public async Task Entrenar(string outputModelPath, List<T> data)
        {
            var dataView = _mlContext.Data.LoadFromEnumerable(data);
            var model = await ReentrenarModelo(dataView);
            await GuardarModelo(_mlContext, model, dataView, outputModelPath);
        }

        public async Task GuardarModelo(MLContext mlContext, ITransformer model, IDataView data, string modelSavePath)
        {
            // Pull the data schema from the IDataView used for training the model
            DataViewSchema dataViewSchema = data.Schema;

            using (var fs = File.Create(modelSavePath))
                _mlContext.Model.Save(model, dataViewSchema, fs);
        }
        
        public async Task<ITransformer> ReentrenarModelo(IDataView trainData)
        {
            var model = _pipeline.Fit(trainData);
            return model;
        }


    }
}
