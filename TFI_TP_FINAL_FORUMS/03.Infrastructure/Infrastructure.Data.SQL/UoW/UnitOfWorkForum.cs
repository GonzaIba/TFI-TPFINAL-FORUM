using Core.Contracts.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CrossCutting.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Core.Contracts.UoW;

namespace Infrastructure.Data.SQL.UoW
{
    public class UnitOfWorkForum : UnitOfWorkBase, IUnitOfWorkForum
    {
        private List<Type> _repositoriesForum;
        private List<Type> _repositoriesGateway;

        public UnitOfWorkForum(ApplicationDbContext appDbContext) : base(appDbContext)
        {
            InitRepositoryForum();
        }

        public I GetRepository<I>()
        {
            var tipe = typeof(I);
            var repository = _repositoriesForum.FirstOrDefault(t => typeof(I).IsAssignableFrom(t));
            return (I)Activator.CreateInstance(repository, _dbContext);
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
    }
}
