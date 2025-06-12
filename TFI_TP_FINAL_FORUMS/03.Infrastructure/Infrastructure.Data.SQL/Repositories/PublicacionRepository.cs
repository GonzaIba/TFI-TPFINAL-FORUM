using Core.Contracts.Repositories;
using Core.Domain.Models;
using Microsoft.ML;
using Microsoft.ML.Data;
using System.Security.Cryptography.Xml;

namespace Infrastructure.Data.SQL.Repositories
{
    public class PublicacionRepository : GenericRepository<PublicacionModel>, IPublicacionRepository
    {
        private readonly MLContext _ml;
        private readonly ITransformer _tfidfModel;
        private IDataView _allTransformed; // vectores precomputados

        public PublicacionRepository(ApplicationDbContext ctx) : base(ctx)
        {
            _ml = new MLContext(seed: 42); // Inicializamos MLContext con una semilla para reproducibilidad
            var all = Get().Result.ToList();
            _tfidfModel = BuildTfIdfPipeline(all);
            BuildIndex().Wait(); // ahora con pipeline correctamente ajustado
        }

        private ITransformer BuildTfIdfPipeline(List<PublicacionModel> seedData)
        {
            // Ejemplo: tomamos una sola publicación real
            var sample = seedData.Take(1)
                .Select(p => new Input { ID = p.IDPublicacion, Texto = p.Titulo + " " + p.Contenido })
                .ToList();

            // Creamos el DataView con datos reales
            var dv = _ml.Data.LoadFromEnumerable(sample);

            var pipeline = _ml.Transforms.Text
                .FeaturizeText(outputColumnName: "Features", inputColumnName: nameof(Input.Texto))
                .Append(_ml.Transforms.NormalizeLpNorm("Features"));

            return pipeline.Fit(dv);
        }

        private async Task BuildIndex()
        {
            var allPubs = await Get(); // tu método existente

            // Convertimos a Input solo los campos necesarios
            var data = allPubs.Select(p => new Input
            {
                ID = p.IDPublicacion,
                Texto = p.Titulo + " " + p.Contenido
            }).ToList();

            // Creamos un SchemaDefinition que incluya solo ID y Texto
            var schema = SchemaDefinition.Create(typeof(Input));
            // No necesitamos hacer nada más, DateTime no está en Input

            // Cargamos IDataView usando solo Input
            var dv = _ml.Data.LoadFromEnumerable(data, schema);
            _allTransformed = _tfidfModel.Transform(dv);
        }

        public async Task<List<(PublicacionModel pub, float score)>> GetRelatedAsync(int id, int limit = 5)
        {
            var all = await Get();
            var targetPub = all.FirstOrDefault(p => p.IDPublicacion == id);
            if (targetPub == null) return new List<(PublicacionModel, float)>();

            var enumerated = _ml.Data.CreateEnumerable<TransformedInput>(_allTransformed, reuseRowObject: false).ToList();
            var targetVec = enumerated.First(x => x.ID == id).Features;

            var results = new List<(PublicacionModel, float)>();
            foreach (var item in enumerated)
            {
                if (item.ID == id) continue;
                var candidate = all.First(p => p.IDPublicacion == item.ID);
                var sim = Cosine(targetVec, item.Features);
                results.Add((candidate, sim));
            }

            return results
                .Where(x => x.Item2 > 0)
                .OrderByDescending(x => x.Item2)
                .Take(limit)
                .ToList();
        }

        private float Cosine(VBuffer<float> a, VBuffer<float> b)
        {
            var dot = 0f;
            var normA = 0f;
            var normB = 0f;
            foreach (var (i, v) in a.Items())
            {
                dot += v * b.GetItemOrDefault(i);
            }
            foreach (var v in a.DenseValues()) normA += v * v;
            foreach (var v in b.DenseValues()) normB += v * v;
            return dot / (MathF.Sqrt(normA) * MathF.Sqrt(normB) + 1e-6f);
        }

        private class Input { public int ID; public string Texto; }
        private class TransformedInput { public int ID; public VBuffer<float> Features; }
    }
}
