using Api.StartupConfiguration;
using ApiForums.Background;
using ApiForums.Mapping;
using ApiForums.Middleware;
using ApiForums.StartupConfiguration;
using AutoMapper;
using Core.Contracts.Configurations;
using CrossCutting.Extensions;
using Hangfire;
using Infrastructure.Data.SQL;
using Infrastructure.ML.Contracts;
using Infrastructure.ML.Repositories;
using IoC.Resolver;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Logging;
using Microsoft.ML;
using ModelContextProtocol.Client;
using ModelContextProtocol.Server;
using OpenAI;
using OpenTelemetry;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

internal class Program
{
    [Obsolete]
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        ILoggerFactory _loggerFactory = LoggerFactory.Create(builder => builder.AddConsole().AddDebug());

        builder.Services.Configure<IISServerOptions>(options =>
        {
            options.AllowSynchronousIO = true;
        });

        #region ConfigureServices

        #region Configure Basic Services
        IdentityModelEventSource.ShowPII = true;
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddHttpClient();
        builder.Services.AddOutputCache(opciones => {
            opciones.AddPolicy("TESTER", builder => builder.Expire(TimeSpan.FromSeconds(30)).Tag("ROLES"));
        });
        #endregion

        #region Configure Personalized
        // Obtén el valor de modelPath de tu configuración de la aplicación
        var modelPath = builder.Configuration["ML_Config:TextoPrediccionesPath"];
        builder.Services.AddSingleton<MLContext>();
        builder.Services.AddTransient<ITextoPrediccionRepositoryML, TextoPrediccionRepositoryML>();
        //builder.Services.AddSingleton<ITextoPrediccionRepositoryML>(x => new TextoPrediccionRepositoryML(new MLContext()));
        builder.Services.ConfigureSwagger(builder.Environment);
        builder.Services.ConfigureIoC(builder.Configuration);
        builder.Services.ConfigureLogger(builder?.Configuration);
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddHangfire(x => x.UseSqlServerStorage(GetGatewayConnectionString()));
        builder.Services.AddHangfireServer();
        builder.Services.AddControllers(options =>
        {
            // Inserto primero la convención para que se aplique antes que otras
            options.Conventions.Insert(
                0,
                new RoutePrefixConvention(new RouteAttribute("api")));
        });
        #endregion

        #region Configure DbContext
        builder.Services.AddDbContext<ApplicationDbContext>
        (
            options => options
            .UseSqlServer(GetConnectionString())

            //.UseMySQL(GetMySQLConnectionString(), builder =>
            //     builder.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null)) //Al contexto le agrego la conexion de la base de datos

            //En esta parte configuramos el entity framework para ver los querys en consola (IMPORTANTE: desactivarlo en produccion)
            .EnableSensitiveDataLogging()
            .UseLoggerFactory(_loggerFactory)
        );

        builder.Services.AddDbContext<ApplicationGatewayDbContext>
        (
            options => options
            .UseSqlServer(GetGatewayConnectionString())
            .EnableSensitiveDataLogging()
            .UseLoggerFactory(_loggerFactory)
        );
        #endregion

        #region Configure AppSettings Inyection
        builder.Services.AddConfig<ActionLoggerMiddlewareConfiguration>(builder.Configuration, nameof(ActionLoggerMiddlewareConfiguration));
        builder.Services.AddConfig<ProfileImageConfiguration>(builder.Configuration, nameof(ProfileImageConfiguration));
        #endregion

        #region Configure Mapper
        var mappingConfig = new MapperConfiguration(mc =>
        {
            mc.AddProfile(new Mapping());
        });
        IMapper mapper = mappingConfig.CreateMapper();
        builder.Services.AddSingleton(mapper);
        #endregion

        #region Configure Host Services
        builder.Services.AddHostedService<TasksResolver>();
        builder.Services.AddSingleton<IBackgroundTasksQueue, BackgroundTasksQueue>();
        builder.Services.AddSignalR();
        #endregion

        #region Configure ML (Machine Learning)
        //var modelPath = builder.Configuration["ML_Config:ModelPath"] ?? "";
        //builder.Services.AddSingleton(new QuestionPredictionEngine(modelPath));
        //builder.Services.AddSingleton<PredictionEngine<QuestionModel, QuestionPrediction>>();
        #endregion

        #region Configure MCP

        builder.Services.AddMcpServer()
            .WithHttpTransport()
            .WithToolsFromAssembly();

        var openAIClient = new OpenAIClient(builder.Configuration["AI:ApiKey"]).GetChatClient("gpt-4o-mini");

        //IChatClient samplingClient = openAIClient.AsIChatClient()
        //    .AsBuilder()
        //    .UseOpenTelemetry(loggerFactory: _loggerFactory, configure: o => o.EnableSensitiveData = true)
        //    .Build();

        //var mcpClient = McpClientFactory.CreateAsync(
        //    new StdioClientTransport(new()
        //    {
        //        Command = "npx",
        //        Arguments = ["-y", "--verbose", "@modelcontextprotocol/server-everything"],
        //        Name = "Everything",
        //    }),
        //    clientOptions: new()
        //    {
        //        Capabilities = new() { Sampling = new() { SamplingHandler = samplingClient.CreateSamplingHandler() } },
        //    },
        //    loggerFactory: _loggerFactory).Result;


        //builder.Services.AddSingleton<IMcpClient>(mcpClient);
        //https://github.com/3choff/mcp-chatbot


        IChatClient chatClient = openAIClient.AsIChatClient()
                .AsBuilder()
                .UseFunctionInvocation()
                .UseOpenTelemetry(loggerFactory: _loggerFactory, configure: o => o.EnableSensitiveData = true)
                .Build();

        //builder.Services.AddSingleton<McpServerTool>();


        //builder.Services.AddSingleton(openAIClient);
        builder.Services.AddSingleton<IChatClient>(chatClient);


        builder.Services.AddOpenTelemetry()
            .WithTracing(b => b.AddSource("*")
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation())
            .WithMetrics(b => b.AddMeter("*")
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation())
            .WithLogging()
            .UseOtlpExporter();

        //IChatClient client =
        //    new OpenAIClient(builder.Configuration["AI:ApiKey"])
        //        .AsChatClient("gpt-4o-mini");

        //builder.Services.AddKeyedSingleton<IChatClient>(chatClient);

        //builder.Services.AddChatClient(services => services.GetRequiredService<OpenAIClient>().GetChatClient("gpt-4o-mini"))
        //    .UseDistributedCache()
        //    .UseLogging();
        #endregion

        #endregion


        #region ConfigureApp
        var app = builder.Build();
        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            Configure(app,
                      app.Environment,
                      services.GetRequiredService<ApplicationDbContext>()
                      );
        }

        #region Configure Init Application
        void Configure(IApplicationBuilder app, IWebHostEnvironment env, ApplicationDbContext context)
        {
            #region Configure Development Environment
            if (env.IsDevelopment())
            {
                //context.Database.Migrate(); //Cuando se ejecuta la aplicación se ejecuta el metodo update-database de dotnet ef core...
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    //c.RoutePrefix = String.Empty;
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "UAI TFI-TP-FINAL API V1");
                    c.InjectStylesheet("/swagger-ui/SwaggerDark.css");
                    c.EnableValidator(null);
                    c.OAuthAdditionalQueryStringParams(new Dictionary<string, string> { { "audience", "" } });
                });
                //context.Database.Migrate(); //Cuando se ejecuta la aplicación se ejecuta el metodo update-database de dotnet ef core...
            }
            #endregion

            #region Configure CORS
            var corsAllowAll = builder.Configuration["CorsAllowedAllHosts"] ?? "false";
            app.UseCors(GetCorsConfig(corsAllowAll == "true"));
            #endregion

            #region Save Data For Application
            var htmlDocuments = builder.Configuration.GetSection("TemplatesEmailPath").Value ?? AppDomain.CurrentDomain.BaseDirectory + "HtmlDocuments\\";

            AppDomain.CurrentDomain.SetData("ContentRootPath", env.ContentRootPath);
            AppDomain.CurrentDomain.SetData("WebRootPath", env.WebRootPath);
            AppDomain.CurrentDomain.SetData("HtmlDocuments", htmlDocuments);
            #endregion

            #region Configure Default Methods .NET
            app.UseStaticFiles();
            app.UseRouting();
            //app.UseMiddleware<McpInspectorMiddleware>();
            app.UseMiddleware<RequestMiddleware>();
            app.UseOutputCache();
            app.UseHttpsRedirection();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            #endregion

            app.UseHangfireDashboard();
        }
        #endregion

        #region Run App
        app.MapMcp();
        app.Run();
        #endregion
        #endregion


        #region Helpers
        Action<CorsPolicyBuilder> GetCorsConfig(bool allowAnyOrigin)
        {
            void configAllowSpecific(CorsPolicyBuilder configurePolicy)
            {
                string origins = builder?.Configuration?.GetSection("AllowedOrigins")?.Value ?? "";

                configurePolicy
                .AllowAnyHeader()
                .AllowAnyMethod()
                .WithOrigins(origins.Split(","))
                .AllowCredentials();
            }

            void configAllowAll(CorsPolicyBuilder configurePolicy)
            {
                configurePolicy
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowAnyOrigin();
            }

            if (allowAnyOrigin) return configAllowAll;
            else return configAllowSpecific;
        }
        string GetConnectionString()
        {
            var connectionString = builder.Configuration.GetConnectionString("SqlConnection");
            return connectionString;
        }
        string GetGatewayConnectionString()
        {
            var connectionString = builder.Configuration.GetConnectionString("SqlConnectionGateway");
            return connectionString;
        }
        string GetMySQLConnectionString()
        {
            var connectionString = builder.Configuration.GetConnectionString("MySqlConnection");
            return connectionString;
        }

        //static Task ConfigureMcpSessionOptions(HttpContext httpContext, McpServerOptions options, CancellationToken cancellationToken)
        //{
        //    if (httpContext == null) throw new ArgumentNullException(nameof(httpContext));
        //    if (options == null) throw new ArgumentNullException(nameof(options));

        //    options.Capabilities = new()
        //    {
        //        Prompts = new()
        //        {
        //            GetPromptHandler = (promptId, cancellationToken) =>
        //            {
        //                // Aquí puedes implementar la lógica para obtener un prompt específico por su ID
        //                // Por ejemplo, podrías buscar en una base de datos o en un archivo de configuración.
        //                return Task.FromResult(new McpPrompt(promptId, "Descripción del prompt"));
        //            }
        //        }
        //    };
        //    options.Cookie.HttpOnly = true;
        //    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;

        //    return Task.CompletedTask;
        //}
        #endregion
    }

    public class RoutePrefixConvention : IApplicationModelConvention
    {
        private readonly AttributeRouteModel _prefix;
        public RoutePrefixConvention(IRouteTemplateProvider routeAttribute)
        {
            _prefix = new AttributeRouteModel(routeAttribute);
        }

        public void Apply(ApplicationModel application)
        {
            foreach (var controller in application.Controllers)
            {
                // Si ya hay rutas con atributos, las combinamos:
                foreach (var selector in controller.Selectors
                                                   .Where(s => s.AttributeRouteModel != null))
                {
                    selector.AttributeRouteModel =
                        AttributeRouteModel.CombineAttributeRouteModel(
                            _prefix, selector.AttributeRouteModel);
                }

                // Si no tiene ruta, le ponemos solo el prefijo:
                if (!controller.Selectors.Any(s => s.AttributeRouteModel != null))
                {
                    controller.Selectors.Add(new SelectorModel
                    {
                        AttributeRouteModel = _prefix
                    });
                }
            }
        }
    }
}
