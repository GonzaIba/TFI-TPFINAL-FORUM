using Infrastructure.ML.Contracts;
using Infrastructure_ML;
using Microsoft.ML.Data;
using Microsoft.ML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Infrastructure_ML.PublicacionTituloML;
using Microsoft.ML.Trainers;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Core.Domain.Models;

namespace Infrastructure.ML.Repositories
{
    public class TextoPrediccionRepositoryML : ITextoPrediccionRepositoryML
    {
        private readonly Lazy<PredictionEngine<ModelInput, ModelOutput>> PredictEngine;
        private readonly string _modelPath;

        public TextoPrediccionRepositoryML(string modelPath)
        {
            PredictEngine = new Lazy<PredictionEngine<ModelInput, ModelOutput>>(() => CargarModelo(), true);
            _modelPath = modelPath;
        }
        
        public PredictionEngine<ModelInput, ModelOutput> CargarModelo()
        {
            var mlContext = new MLContext();
            ITransformer mlModel = mlContext.Model.Load(_modelPath, out var _);
            return mlContext.Model.CreatePredictionEngine<ModelInput, ModelOutput>(mlModel);
        }

        public async Task<IOrderedEnumerable<KeyValuePair<string, float>>> PredecirEtiquetasYProbabilidades(ModelInput input)
        {
            var predEngine = PredictEngine.Value;
            var result = predEngine.Predict(input);
            return await ObtenerEtiquetasYProbabilidades(result);
        }

        public async Task<ModelOutput> PredecirEtiqueta(ModelInput input)
        {
            var predEngine = PredictEngine.Value;
            return predEngine.Predict(input);
        }

        public async Task<IEnumerable<string>> PredecirEtiquetas(ModelInput input, int cantidad)
        {
            var predEngine = PredictEngine.Value;
            var result = predEngine.Predict(input);
            var result2 = (await ObtenerEtiquetasYProbabilidades(result)).Select(x => x).Take(cantidad);
            List<string> etiquetasSpliteadas = new();
            result2.Select(pair => pair.Key).ToList().ForEach(x => { etiquetasSpliteadas.AddRange(x.Split(",")); });
            return etiquetasSpliteadas.Take(cantidad);
        }

        public async Task<IOrderedEnumerable<KeyValuePair<string, float>>> ObtenerEtiquetasYProbabilidades(ModelOutput result)
        {
            var unlabeledScores = result.Score;
            var labelNames = await ObtenerEtiquetas(result);

            Dictionary<string, float> labledScores = new Dictionary<string, float>();
            for (int i = 0; i < labelNames.Count(); i++)
            {
                // Map the names to the predicted result score array
                var labelName = labelNames.ElementAt(i);
                labledScores.Add(labelName.ToString(), unlabeledScores[i]);
            }

            return labledScores.OrderByDescending(c => c.Value);
        }

        public async Task<IEnumerable<string>> ObtenerEtiquetas(ModelOutput result)
        {
            var schema = PredictEngine.Value.OutputSchema;

            var labelColumn = schema.GetColumnOrNull("Etiquetas");
            if (labelColumn == null)
            {
                throw new Exception("Etiquetas column not found. Make sure the name searched for matches the name in the schema.");
            }

            // Key values contains an ordered array of the possible labels. This allows us to map the results to the correct label value.
            var keyNames = new VBuffer<ReadOnlyMemory<char>>();
            labelColumn.Value.GetKeyValues(ref keyNames);
            return keyNames.DenseValues().Select(x => x.ToString());
        }


        /// <summary>
        /// Train a new model with the provided dataset.
        /// </summary>
        /// <param name="outputModelPath">File path for saving the model. Should be similar to "C:\YourPath\ModelName.mlnet"</param>
        /// <param name="connectionString">Connection string for databases on-premises or in the cloud.</param>
        /// <param name="commandText">Command string for selecting training data.</param>
        public static void Train(string outputModelPath, List<TextoPrediccionModel> data)
        {
            var mlContext = new MLContext();

            var dataView = mlContext.Data.LoadFromEnumerable(data);
            var model = RetrainModel(mlContext, dataView);
            SaveModel(mlContext, model, dataView, outputModelPath);
        }

        /// <summary>
        /// Save a model at the specified path.
        /// </summary>
        /// <param name="mlContext">The common context for all ML.NET operations.</param>
        /// <param name="model">Model to save.</param>
        /// <param name="data">IDataView used to train the model.</param>
        /// <param name="modelSavePath">File path for saving the model. Should be similar to "C:\YourPath\ModelName.mlnet.</param>
        public static void SaveModel(MLContext mlContext, ITransformer model, IDataView data, string modelSavePath)
        {
            // Pull the data schema from the IDataView used for training the model
            DataViewSchema dataViewSchema = data.Schema;

            using (var fs = File.Create(modelSavePath))
            {
                mlContext.Model.Save(model, dataViewSchema, fs);
            }
        }


        /// <summary>
        /// Retrain model using the pipeline generated as part of the training process.
        /// </summary>
        /// <param name="mlContext"></param>
        /// <param name="trainData"></param>
        /// <returns></returns>
        public static ITransformer RetrainModel(MLContext mlContext, IDataView trainData)
        {
            var pipeline = BuildPipeline(mlContext);
            var model = pipeline.Fit(trainData);

            return model;
        }

        /// <summary>
        /// build the pipeline that is used from model builder. Use this function to retrain model.
        /// </summary>
        /// <param name="mlContext"></param>
        /// <returns></returns>
        public static IEstimator<ITransformer> BuildPipeline(MLContext mlContext)
        {
            // Data process configuration with pipeline data transformations
            var pipeline = mlContext.Transforms.Text.FeaturizeText(inputColumnName: @"Texto", outputColumnName: @"Texto")
                                    .Append(mlContext.Transforms.Concatenate(@"Features", new[] { @"Texto" }))
                                    .Append(mlContext.Transforms.Conversion.MapValueToKey(outputColumnName: @"Etiquetas", inputColumnName: @"Etiquetas"))
                                    .Append(mlContext.Transforms.NormalizeMinMax(@"Features", @"Features"))
                                    .Append(mlContext.MulticlassClassification.Trainers.SdcaMaximumEntropy(new SdcaMaximumEntropyMulticlassTrainer.Options() { L1Regularization = 1F, L2Regularization = 0.1F, LabelColumnName = @"Etiquetas", FeatureColumnName = @"Features" }))
                                    .Append(mlContext.Transforms.Conversion.MapKeyToValue(outputColumnName: @"PredictedLabel", inputColumnName: @"PredictedLabel"));

            return pipeline;
        }
    }
}
