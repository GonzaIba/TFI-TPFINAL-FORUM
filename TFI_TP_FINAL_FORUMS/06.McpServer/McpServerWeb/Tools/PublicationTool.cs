using Core.Contracts.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.AI;
using ModelContextProtocol.Server;
using System.ComponentModel;

namespace McpServerWeb.Tools
{
    [McpServerToolType]
    public sealed class PublicationsTool
    {
        [McpServerTool(Name = "BuscarPublicacion"), Description("Busca una publicacion por nombre de titulo")]
        public static async Task<string> BuscarPublicacion(
        [Description("Nombre del título de la publicación a buscar")] string titleName,
        [FromServices] IPublicacionService publicacionService)
        {
            var publicaciones = (await publicacionService.Get(u => u.Titulo == titleName)).FirstOrDefault();
            return publicaciones != null
                ? $"Publicacion encontrada, la descripción es: {publicaciones.Contenido}"
                : "No se encontró ningúna publicación con ese título.";
        }

        [McpServerTool, Description("Resumir una publicación")]
        public static async Task<string> ResumirPublicacion(
            [Description("Id de publicacion")] int publicationId,
            [FromServices] IMcpServer server,
            [FromServices] IPublicacionService pubService)
        {
            var pub = (await pubService.Get(u => u.IDPublicacion == publicationId, tracking: false)).FirstOrDefault();
            if (pub == null) return "No se encontró la publicación.";

            var chat = server.AsSamplingChatClient();
            var mensajes = new[]
            {
                new ChatMessage(ChatRole.User, $"Resumí la siguiente publicación: {pub.Contenido}"),
                new ChatMessage(ChatRole.Assistant, "Claro, aquí tienes un resumen de la publicación.")
            };

            var response = await chat.GetResponseAsync(mensajes, new ChatOptions { MaxOutputTokens = 150 }, default);
            return response?.Text ?? "No se pudo generar un resumen.";
        }

        [McpServerTool, Description("Resume el debate de una publicación a partir de sus respuestas")]
        public static async Task<string> ResumirDebatePublicacion(
            [Description("Id de publicacion")] int publicationId,
            [FromServices] IMcpServer server,
            [FromServices] IPublicacionService pubService,
            [FromServices] IRespuestaService respuestaService)
        {
            var pub = (await pubService.Get(u => u.IDPublicacion == publicationId, tracking: false)).FirstOrDefault();
            if (pub == null) return "No se encontró la publicación.";

            var respuestas = await respuestaService.Get(x => x.IDPublicacion == publicationId);
            if (!respuestas.Any()) return "La publicación no tiene respuestas.";

            var respuestasTexto = respuestas.Select((r, i) => $"{i + 1}. {r.TextoRespuesta}").ToList();
            var todoElContenido = string.Join("\n", respuestasTexto);

            var chat = server.AsSamplingChatClient();
            var mensajes = new[]
            {
                new ChatMessage(ChatRole.User, $"Estas son las respuestas a una publicación:\n{todoElContenido}\n\n¿Podés hacer un resumen del debate?"),
                new ChatMessage(ChatRole.Assistant, "Claro, aquí tienes un resumen de las ideas principales debatidas:")
            };

            var result = await chat.GetResponseAsync(mensajes, new ChatOptions { MaxOutputTokens = 200, Temperature = 0.3f }, default);
            return result?.Text ?? "No se pudo generar el resumen del debate.";
        }

    }
}
