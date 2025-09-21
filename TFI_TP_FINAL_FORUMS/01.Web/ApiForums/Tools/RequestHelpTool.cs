using ModelContextProtocol.Server;
using System.ComponentModel;

namespace ApiForums.Tools
{
    [McpServerToolType]
    public class RequestHelpTool
    {
        [McpServerTool(Name = "ConocerPaginaAyudaEnVivo"), Description("Introducción a la página de ayuda en vivo")]
        public static string RequestHelpIntroduction()
        {
            return "Esta es una página en el que podes pedir ayuda, ayudar a gente y ganar plata en tu tiempo libre.";
        }
    }
}
