using Core.Business.Services;
using Core.Contracts.Services;
using IoC.Resolver.Register;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using CrossCutting.EmailService.Configurations;
using CrossCutting.EmailService.Contracts;
using CrossCutting.EmailService.Factory;
using CrossCutting.EmailService.Services;
using CrossCutting.Extensions;
using CrossCutting.StorageService.Configurations;
using CrossCutting.StorageService.Contracts;
using CrossCutting.StorageService.Factory;
using CrossCutting.StorageService.Services;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Data.SQL;
using Core.Contracts.Publishers;
using Infrastructure.Data.SQL.Publishers;
using StackExchange.Redis;

namespace IoC.Resolver
{
    public static class IoCRegister
    {
        public static IServiceCollection ConfigureIoC(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<DbContext, ApplicationGatewayDbContext>();
            services.AddScoped<DbContext, ApplicationDbContext>();

            services.RegisterDataLayer(configuration);
            services.RegisterUnitOfWork();
            services.RegisterBusinessLayer();
            services.RegisterEmails(configuration);
            services.RegisterStorages(configuration);
            services.RegisterPublishers(configuration);
            return services;
        }

        private static IServiceCollection RegisterEmails(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IEmailService, EmailService>();

            //services.AddTransient(typeof(IGenericEmailService), typeof(GenericEmailService));
            services.AddTransient<IEmailSendGridService, EmailSendgridService>();
            services.AddTransient<IEmailSmtpService, EmailSmtpService>();
            services.AddTransient<IGenericEmailFactory, GenericEmailFactory>();

            services.AddConfig<EmailConfiguration>(configuration, nameof(EmailConfiguration));
            services.AddConfig<EmailSendGridConfiguration>(configuration, nameof(EmailConfiguration) + "." + nameof(EmailSendGridConfiguration));
            services.AddConfig<EmailSMTPConfiguration>(configuration, nameof(EmailConfiguration) + "." + nameof(EmailSMTPConfiguration));

            return services;
        }

        private static IServiceCollection RegisterStorages(this IServiceCollection services, IConfiguration configuration)
        {
            //services.AddTransient<IGenericStorageService, GenericStorageService>();
            services.AddTransient<IAzureBlobStorageService, AzureBlobStorageService>();
            services.AddTransient<IGoogleCloudStorageService, GoogleCloudStorageService>();
            services.AddTransient<IFileSystemStorageService, FileSystemStorageService>();
            services.AddTransient<IGenericStorageServiceFactory, GenericStorageServiceFactory>();

            services.AddConfig<GenericStorageConfiguration>(configuration, nameof(GenericStorageConfiguration));
            services.AddConfig<FileSystemStorageConfiguration>(configuration, nameof(GenericStorageConfiguration) + "." + nameof(FileSystemStorageConfiguration));
            services.AddConfig<GoogleCloudStorageConfiguration>(configuration, nameof(GenericStorageConfiguration) + "." + nameof(GoogleCloudStorageConfiguration));
            services.AddConfig<AzureBlobStorageConfiguration>(configuration, nameof(GenericStorageConfiguration) + "." + nameof(AzureBlobStorageConfiguration));

            return services;
        }

        private static IServiceCollection RegisterPublishers(this IServiceCollection services, IConfiguration configuration)
        {
            var redisConnectionString = configuration.GetConnectionString("Redis");
            services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConnectionString));
            services.AddTransient<IPublicationVotePublisher, PublicationVotePublisher>();
            services.AddTransient<IPublisherService, PublisherService>();
            return services;
        }


        //Mas adelante, la idea es mudar todos los configurations a un nuevo proyecto en donde solo se guardan las configs y levantarlos con reflexión.
    }
}
