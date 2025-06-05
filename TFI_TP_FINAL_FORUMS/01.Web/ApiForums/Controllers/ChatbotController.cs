using ApiForums.Tools;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.AI;

namespace ApiForum.Controllers
{
    [Produces("application/json")]
    [ApiController]
    [Route("v1/[controller]")]
    public class ChatbotController : ControllerBase
    {
        private readonly IChatClient _chatClient;

        public ChatbotController(IChatClient chatClient)
        {
            _chatClient = chatClient;
        }

        [HttpPost]
        public async Task<IActionResult> Chat([FromBody] string userMessage)
        {
            if (string.IsNullOrWhiteSpace(userMessage))
                return BadRequest("Mensaje vacío");

            try
            {
                var mensajes = new[]
                {
                    new ChatMessage(ChatRole.User, $"{userMessage}"),
                };

                List<ChatMessage> messages = [];
                List<ChatResponseUpdate> updates = [];
                messages.Add(new(ChatRole.User, $"{userMessage}"));
                await foreach (var update in _chatClient.GetStreamingResponseAsync(messages))
                {
                    Console.Write(update);
                    updates.Add(update);
                }
                messages.AddMessages(updates);
                return Ok(messages);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno en el asistente: {ex.Message}");
            }
        }
    }
}
