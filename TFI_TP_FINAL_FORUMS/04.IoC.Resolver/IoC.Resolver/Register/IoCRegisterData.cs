using Core.Contracts.Repositories;
using Infrastructure.Data.SQL.UoW;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace IoC.Resolver.Register
{
    internal static class IoCRegisterData
    {
        internal static IServiceCollection RegisterUnitOfWork(this IServiceCollection services)
        {
            return services.AddTransient<IUnitOfWork, UnitOfWork>();
        }
    }
}
