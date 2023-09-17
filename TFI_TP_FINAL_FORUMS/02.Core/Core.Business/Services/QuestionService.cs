using Core.Contracts.Repositories;
using Core.Contracts.Services;
using Core.Domain.IdentityModels;
using Core.Domain.ML;
using CrossCutting.Helpers.JWT;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers;
using Org.BouncyCastle.Ocsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Core.Business.Services
{
    public class QuestionService : GenericService<QuestionML>, IQuestionService
    {
        private readonly IQuestionRepository _questionRepository;
        
        public QuestionService(IUnitOfWork unitOfWork)
            : base(unitOfWork, unitOfWork.GetRepository<IQuestionRepository>())
        {
            _questionRepository = unitOfWork.GetRepository<IQuestionRepository>();
        }
        public async Task<IEnumerable<QuestionML>> GetQuestionsAsync()
        {
            return await _questionRepository.Get();
        }

        public async Task AddQuestionAsync(QuestionML question)
        {
            await _questionRepository.Insert(question);
            await _unitOfWork.SaveChangesAsync();
        }
    }
    public class QuestionPredictionEngine
    {
        private readonly PredictionEngine<QuestionML, QuestionPrediction> _predictionEngine;

        public QuestionPredictionEngine(string modelPath)
        {
            var mlContext = new MLContext();

            var dataView = mlContext.Data.LoadFromTextFile<QuestionML>(modelPath, separatorChar: ',', hasHeader: true);

            // Define the machine learning pipeline   
            var pipeline = mlContext.Transforms.Text.FeaturizeText(nameof(QuestionML.Text))
                .Append(mlContext.Transforms.Conversion.MapValueToKey(
                    inputColumnName: nameof(QuestionML.Tags), outputColumnName: "Tags"))
                .Append(mlContext.Transforms.Conversion.MapKeyToValue("Tags", "Tags"));

            // Fit the training pipeline to the data view
            var transformedData = pipeline.Fit(dataView);

            _predictionEngine = mlContext.Model.CreatePredictionEngine<QuestionML, QuestionPrediction>(transformedData);
        }

        public PredictionEngine<QuestionML, QuestionPrediction> GetPredictionEngine()
        {
            return _predictionEngine;
        }
    }
}



//// Evaluate the model on the testing dataset
//var predictions = model.Transform(dataView);
//var metrics = mlContext.BinaryClassification.Evaluate(predictions, "Label");
//Console.WriteLine($"Accuracy: {metrics.Accuracy:P2}");

////// Save the model to a file
////using (var fs = new FileStream(modelPath, FileMode.Create, FileAccess.Write, FileShare.Write))
////    mlContext.Model.Save();