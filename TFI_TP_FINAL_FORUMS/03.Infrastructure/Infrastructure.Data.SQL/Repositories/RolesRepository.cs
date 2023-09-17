using System;
using System.Collections.Generic;
using System.Text;
using Core.Contracts.Repositories;
using Core.Domain.IdentityModels;
using Infrastructure.Data.SQL;

namespace Infrastructure.Data.SQL.Repositories
{
    public class RolesRepository : GenericRepository<Roles>, IRolesRepository
    {
        public RolesRepository(ApplicationDbContext applicationDbContext)
            : base(applicationDbContext)
        {

        }
    }
}
