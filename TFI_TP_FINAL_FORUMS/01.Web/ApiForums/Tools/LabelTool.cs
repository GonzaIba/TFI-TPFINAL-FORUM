using Core.Contracts.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.AI;
using ModelContextProtocol.Server;
using System.ComponentModel;

namespace ApiForums.Tools
{
    [McpServerToolType]
    public class LabelTool
    {
        [McpServerTool(Name = "ConocerPaginaEtiqueta"), Description("Introducción a la página de etiquetas")]
        public static string LabelsIntroduction()
        {
            return "Esta es una página en el que podes filtrar etiquetas y conocerlas.";
        }
    }
}
