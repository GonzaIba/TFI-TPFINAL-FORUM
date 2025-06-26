using Infrastructure.ML.Contracts;
using Microsoft.ML.Data;
using Microsoft.ML;
using Microsoft.ML.Trainers;
using Core.Domain.Models;
using Core.Contracts.Repositories;
using static Infrastructure_ML.PublicacionTituloML;

namespace Infrastructure.ML.Repositories
{
    public class TextoPrediccionRepositoryML : GenericRepositoryML<EtiquetasPrediccionModeloModel>, ITextoPrediccionRepositoryML
    {
        private readonly Lazy<PredictionEngine<ModelInput, ModelOutput>> PredictEngine;
        private readonly IEtiquetasPrediccionModeloRepository _etiquetasModeloRepository;
        private MLContext _mlContext;

        public TextoPrediccionRepositoryML(MLContext mlContext, IEtiquetasPrediccionModeloRepository etiquetasModeloRepository)
            : base(BuildPipeline(mlContext), mlContext)
        {
            _etiquetasModeloRepository = etiquetasModeloRepository;
            _mlContext = mlContext;
            PredictEngine = new Lazy<PredictionEngine<ModelInput, ModelOutput>>(() => CargarModeloDesdeDb(), true);
        }

        public PredictionEngine<ModelInput, ModelOutput> CargarModeloDesdeDb()
        {
            // 1. Trae el modelo más reciente
            var modelEntity = _etiquetasModeloRepository
                                 .GetAllAsync()
                                 .GetAwaiter().GetResult();  // evita deadlock en sync

            var latest = modelEntity.FirstOrDefault()
                     ?? throw new InvalidOperationException("No se encontró modelo en DB");

            // 2. Deserializa el stream
            using var ms = new MemoryStream(latest.ModelData);

            // 3. Deserializa el ITransformer
            ITransformer mlModel = _mlContext.Model.Load(ms, out DataViewSchema schema);

            // 4. Crea el PredictionEngine
            return _mlContext.Model.CreatePredictionEngine<ModelInput, ModelOutput>(mlModel);
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
