using Infrastructure_ML;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Infrastructure_ML.PublicacionTituloML;

namespace ApiForums.Controllers
{
    [Produces("application/json")]
    [ApiController]
    [Route("[controller]")]
    public class QuestionsController : ControllerBase
    {
        public QuestionsController()
        {
                
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("PredictLabel")]
        public async Task<IActionResult> PredictLabelML(string questionText)
        {
            try
            {
                // Get the question object from the text
                var question = new ModelInput { Text = questionText };

                // Use the prediction engine to get the recommended tags
                var predictionEngine = PublicacionTituloML.Predict(question);

                // Return the recommended tags
                return Ok(predictionEngine);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
