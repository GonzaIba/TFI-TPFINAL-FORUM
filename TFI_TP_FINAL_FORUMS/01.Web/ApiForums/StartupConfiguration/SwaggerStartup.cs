using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.IO;
using System.Reflection;

namespace Api.StartupConfiguration
{
    public static class SwaggerStartup
    {
        public static IServiceCollection ConfigureSwagger(this IServiceCollection services, IWebHostEnvironment environment)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "TFI API FORUM", Version = "1" });

                if (environment.IsDevelopment())
                {
                    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                    var baseDirectory = Directory.GetCurrentDirectory();
                    var xmlPath = Path.Combine(baseDirectory, xmlFile);

                    // Esto sirve para cuando usamos comentarios, descripciones y detalles de métodos, todo se almacena acá.
                    c.IncludeXmlComments(xmlPath);
                }
            });

            return services;
        }    
    }
}
