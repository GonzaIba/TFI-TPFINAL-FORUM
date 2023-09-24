using System;
using System.Collections.Generic;
using System.IO.Compression;
using Infrastructure_ML;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers;
using static Infrastructure_ML.PublicacionTituloML;

class Program
{
    static void Main(string[] args)
    {
        string questionText = "pizza";
        // Get the question object from the text
        var question = new ModelInput { Pregunta = questionText };

        // Use the prediction engine to get the recommended tags
        //PublicacionTituloML.Train("D:\\Repositorios-SmartGit\\TFI-TPFINAL-FORUM\\TFI_TP_FINAL_FORUMS\\ConsoleApp1\\bin\\Debug\\net7.0\\PublicacionTituloML.mlnet");
        var predictionEngine = PublicacionTituloML.Predict(question);

        Console.WriteLine(predictionEngine.PredictedLabel);
    }
}



//// Define una clase para representar tus datos
//public class Pregunta
//{
//    [LoadColumn(0)]
//    public string Text;

//    [LoadColumn(1)]
//    public string Tags;
//}

//// Define una clase para las predicciones
//public class Prediccion
//{
//    [ColumnName("PredictedLabel")]
//    public string EtiquetasPredictivas;

//    public float[] Score { get; set; }
//}

//class Program
//{
//    static void Main(string[] args)
//    {
//        string zipModelRoute = "D:\\Repositorios-SmartGit\\TFI-TPFINAL-FORUM\\TFI_TP_FINAL_FORUMS\\ConsoleApp1\\bin\\Debug\\net7.0\\datos.zip";


//        // Ruta al archivo CSV con tus datos de entrenamiento
//        string dataPath = "D:\\Repositorios-SmartGit\\TFI-TPFINAL-FORUM\\TFI_TP_FINAL_FORUMS\\03.Infrastructure\\Infrastructure.ML\\questionModelsJoined.csv";

//        // Crea una instancia de MLContext
//        var mlContext = new MLContext();

//        // Carga los datos desde el archivo CSV
//        var data = mlContext.Data.LoadFromTextFile<Pregunta>(dataPath, separatorChar: ',');

//        // Divide los datos en conjuntos de entrenamiento y prueba
//        var split = mlContext.Data.TrainTestSplit(data);

//        // Ruta donde deseas guardar el archivo ZIP
//        string archivoZipPath = "datos.zip";

//        // Crear un nuevo archivo ZIP y agregar el archivo CSV
//        if (!File.Exists(zipModelRoute))
//        {
//            using (var archive = ZipFile.Open(archivoZipPath, ZipArchiveMode.Create))
//            {
//                // Agregar el archivo CSV al archivo ZIP
//                archive.CreateEntryFromFile(dataPath, Path.GetFileName(dataPath));
//            }
//        }

//        // Define el pipeline de transformación y entrenamiento
//        var pipeline = mlContext.Transforms.Conversion.MapValueToKey("Label", "Tags")
//            .Append(mlContext.Transforms.Text.FeaturizeText("Features", "Text"))
//            .Append(mlContext.Transforms.NormalizeMinMax("Features"))
//            .Append(mlContext.BinaryClassification.Trainers.SdcaNonCalibrated());

//        // Train model
//        ITransformer trainedModel = pipeline.Fit(data);
//        // Carga el modelo previamente entrenado
//        mlContext.Model.Save(trainedModel, data.Schema,"data.zip");

//        //Define DataViewSchema for data preparation pipeline and trained model
//        DataViewSchema modelSchema;

//        // Load trained model
//        ITransformer model = mlContext.Model.Load("model.zip", out modelSchema);

//        // Crea un motor de inferencia
//        var predictor = mlContext.Model.CreatePredictionEngine<Pregunta, Prediccion>(model);

//        // Ingresa una pregunta para hacer una predicción
//        var pregunta = new Pregunta
//        {
//            Text = "auto"
//        };

//        // Realiza la predicción
//        var prediccion = predictor.Predict(pregunta);

//        // Divide las etiquetas predichas
//        var etiquetasPredichas = prediccion.EtiquetasPredictivas.Split(',');

//        // Imprime las etiquetas predichas
//        Console.WriteLine("Etiquetas predichas:");
//        foreach (var etiqueta in etiquetasPredichas)
//        {
//            Console.WriteLine($"- {etiqueta.Trim()}");
//        }
//    }
//}
