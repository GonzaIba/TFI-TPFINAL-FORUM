using Core.Domain.IdentityModels;
using CrossCutting.Helpers;
using CrossCutting.Extensions;
using Infrastructure.Data.SQL;
using IoC.Resolver;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Core.Contracts.Configurations;
using AutoMapper;
using Microsoft.IdentityModel.Logging;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ApiForums.Background;
using Core.Business.Services;
using Microsoft.ML;
using ApiForums.Mapping;
using ApiForums.StartupConfiguration;
using Api.StartupConfiguration;
using ApiForums.Middleware;

internal class Program
{
    [Obsolete]
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        ILoggerFactory _loggerFactory = LoggerFactory.Create(builder => builder.AddConsole().AddDebug());

        #region ConfigureServices

        #region Configure Basic Services
        IdentityModelEventSource.ShowPII = true;
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddHttpClient();
        builder.Services.AddOutputCache(opciones => {
            opciones.AddPolicy("TESTER", builder => builder.Expire(TimeSpan.FromSeconds(30)).Tag("ROLES"));
        });
        #endregion

        #region Configure Personalized
        builder.Services.ConfigureIoC(builder.Configuration);
        builder.Services.ConfigureLogger(builder?.Configuration);
        builder.Services.ConfigureSwagger(builder?.Configuration);
        builder.Services.AddHttpContextAccessor();
        //builder.Services.TryAddScoped<SignInManager<Users>>();
        #endregion

        #region Configure DbContext
        builder.Services.AddDbContext<ApplicationDbContext>
        (
            options => options
            .UseSqlServer(GetConnectionString(), builder =>
                 builder.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null)) //Al contexto le agrego la conexion de la base de datos

            //.UseMySQL(GetMySQLConnectionString(), builder =>
            //     builder.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null)) //Al contexto le agrego la conexion de la base de datos

            //En esta parte configuramos el entity framework para ver los querys en consola (IMPORTANTE: desactivarlo en produccion)
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
                app.UseSwagger();
                app.UseSwaggerUI();
                app.UseDeveloperExceptionPage();
                context.Database.Migrate(); //Cuando se ejecuta la aplicación se ejecuta el metodo update-database de dotnet ef core...
            }
            #endregion

            #region Configure Swagger
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.RoutePrefix = String.Empty;
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "UAI TFI-TP-FINAL API V1");
            });
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
            app.UseMiddleware<ExceptionMiddleware>();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseOutputCache();
            //app.UseHttpsRedirection();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            #endregion
        }
        #endregion

        #region Run App
        //RUN APP
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
        string GetMySQLConnectionString()
        {
            var connectionString = builder.Configuration.GetConnectionString("MySqlConnection");
            return connectionString;
        }
        #endregion
    }
}
