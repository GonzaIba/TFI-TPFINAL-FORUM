using Core.Business.Publishers;
using Core.Contracts.Publishers;
using CrossCutting.Helpers;
using Infrastructure.Data.SQL.Publishers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace IoC.Resolver.Register
{
    public static class IoCRegisterPublisher
    {
        public static IServiceCollection RegisterPublishers(this IServiceCollection services, IConfiguration configuration)
        {
            var redisConnectionString = configuration.GetConnectionString("Redis");
            var connect = ConnectionMultiplexer.Connect(redisConnectionString);
            services.AddSingleton<IConnectionMultiplexer>(connect);
            services.AddTransient<IPublisherPublication, PublisherPublication>();
            services.AddTransient<IPublisherNotification, PublisherNotification>();
            //services.AddTransient<IPublisherBase, PublisherBase>();
            services.AddSingleton<IPublisherBase, RedisPublisher>();
            services.AddTransient<IPublisherCoreBase, PublisherCoreBase>();
            return services;
        }
    }
}
