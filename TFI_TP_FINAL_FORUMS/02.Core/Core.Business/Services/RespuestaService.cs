using Core.Contracts.Repositories;
using Core.Contracts.Services;
using Core.Contracts.UoW;
using Core.Domain.Models;
using Infrastructure.ML.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Business.Services
{
    public class RespuestaService : GenericService<RespuestaModel>, IRespuestaService
    {
        private readonly IUnitOfWorkGateway _unitOfWorkGateway;
        public RespuestaService(
            IUnitOfWorkForum unitOfWorkForum,
            IUnitOfWorkGateway unitOfWorkGateway,
            IUsersService usersService,
            ITextoPrediccionRepositoryML textoPrediccionRepositoryML,
            IPublisherService publicationPublisher
        )
        : base(unitOfWorkForum, unitOfWorkForum.GetRepository<IRespuestaRepository>())
        {
            _unitOfWorkGateway = unitOfWorkGateway;
        }
    }
}
