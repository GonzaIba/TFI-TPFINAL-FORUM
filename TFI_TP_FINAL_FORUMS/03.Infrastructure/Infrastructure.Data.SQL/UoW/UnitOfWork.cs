using Core.Contracts.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CrossCutting.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Infrastructure.Data.SQL.UoW
{
    public class UnitOfWork : UnitOfWorkBase, IUnitOfWork
    {
        private List<Type> _repositoriesForum;
        private List<Type> _repositoriesGateway;

        public UnitOfWork(ApplicationDbContext appDbContext, ApplicationGatewayDbContext gatewayDbContext) : base(appDbContext, gatewayDbContext)
        {
            InitRepositoryForum();
            InitRepositoryGateway();
        }

        public I GetRepositoryForum<I>()
        {
            var tipe = typeof(I);
            var repository = _repositoriesForum.FirstOrDefault(t => typeof(I).IsAssignableFrom(t));
            return (I)Activator.CreateInstance(repository, _appDbcontext);
        }

        public I GetRepositoryGateway<I>()
        {
            var tipe = typeof(I);
            var repository = _repositoriesGateway.FirstOrDefault(t => typeof(I).IsAssignableFrom(t));
            return (I)Activator.CreateInstance(repository, _gatewayDbContext);
        }

        private void InitRepositoryForum()
        {
            _repositoriesForum = new List<Type>();
            var genericType = typeof(IGenericRepository<>).GetGenericTypeDefinition();

            foreach (var iRepository in genericType.Assembly.GetTypes(t => t.IsInterface && t.ImplementsGenericInterface(genericType)))
            {
                var repository = this.GetType().Assembly.FindType(t => t.IsClass && iRepository.IsAssignableFrom(t));

                ConstructorInfo[] constructors = repository.GetConstructors();

                // Verificar si alguno de los constructores toma un parámetro de tipo ApplicationDbContext
                bool hasApplicationDbContextConstructor = constructors.Any(ctor =>
                    ctor.GetParameters().Any(param =>
                        param.ParameterType == typeof(ApplicationDbContext)
                    )
                );

                if(hasApplicationDbContextConstructor)
                    _repositoriesForum.Add(repository);
            }
        }

        private void InitRepositoryGateway()
        {
            _repositoriesGateway = new List<Type>();
            var genericType = typeof(IGenericRepository<>).GetGenericTypeDefinition();

            foreach (var iRepository in genericType.Assembly.GetTypes(t => t.IsInterface && t.ImplementsGenericInterface(genericType)))
            {
                var repository = this.GetType().Assembly.FindType(t => t.IsClass && iRepository.IsAssignableFrom(t));

                ConstructorInfo[] constructors = repository.GetConstructors();

                // Verificar si alguno de los constructores toma un parámetro de tipo ApplicationDbContext
                bool hasApplicationDbContextConstructor = constructors.Any(ctor =>
                    ctor.GetParameters().Any(param =>
                        param.ParameterType == typeof(ApplicationGatewayDbContext)
                    )
                );

                if (hasApplicationDbContextConstructor)
                    _repositoriesGateway.Add(repository);
            }
        }
    }
}
