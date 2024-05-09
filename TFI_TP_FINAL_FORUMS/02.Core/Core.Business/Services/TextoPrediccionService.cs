using Core.Contracts.Repositories;
using Core.Contracts.Services;
using Core.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Business.Services
{
    public class TextoPrediccionService : GenericService<TextoPrediccionModel>, ITextoPrediccionService
    {
        private readonly IUsersService _usersService;
        public TextoPrediccionService(
            IUnitOfWork unitOfWork,
            IUsersService usersService
            )
        : base(unitOfWork, unitOfWork.GetRepositoryForum<ITextoPrediccionRepository>())
        {
            _usersService = usersService;
        }
    }
}
