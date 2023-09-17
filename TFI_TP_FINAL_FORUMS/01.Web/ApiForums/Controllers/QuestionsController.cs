using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        [Route("GetQuestions")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            try
            {
                

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
