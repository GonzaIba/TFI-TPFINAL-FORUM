using Core.Contracts.Repositories;
using Core.Contracts.UoW;
using CrossCutting.Extensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data.SQL.UoW
{
    public class UnitOfWorkGateway : UnitOfWorkBase, IUnitOfWorkGateway
    {
        private List<Type> _repositoriesGateway;

        public UnitOfWorkGateway(ApplicationGatewayDbContext dbContext):base(dbContext)
        {
            InitRepositoryGateway();
        }
        public I GetRepository<I>()
        {
            var tipe = typeof(I);
            var repository = _repositoriesGateway.FirstOrDefault(t => typeof(I).IsAssignableFrom(t));
            return (I)Activator.CreateInstance(repository, _dbContext);
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
