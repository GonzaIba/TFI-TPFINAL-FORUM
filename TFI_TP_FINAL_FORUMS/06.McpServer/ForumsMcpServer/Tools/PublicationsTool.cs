using Core.Contracts.Repositories;
using Microsoft.Extensions.AI;
using ModelContextProtocol.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForumsMcpServer.Tools
{
    [McpServerToolType]
    public static class PublicationsTool
    {
        [McpServerTool, Description("Busca una publicacion por nombre de titulo")]
        public static async Task<string> BuscarUsuario(
        IPublicacionRepository publicacionRepository,
        [Description("Nombre del título de la publicación a buscar")] string titleName)
        {
            var publicaciones = await publicacionRepository.GetOne(u => u.Titulo == titleName);
            return publicaciones != null
                ? $"Publicacion encontrada, la descripción es: {publicaciones.Contenido}"
                : "No se encontró ningúna publicación con ese título.";
        }









        [McpServerTool, Description("Resumir una publicación")]
        public static async Task<string> ResumirPublicacion(
            IMcpServer server,
            IPublicacionRepository repo,
            int publicationId)
        {
            var pub = await repo.GetOne(u => u.IDPublicacion == publicationId, tracking: false);
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


    }
}
