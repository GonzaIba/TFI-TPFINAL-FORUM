using Core.Contracts.Repositories;
using Core.Domain.IdentityModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Data.SQL.Repositories
{
    public class UsersRolesRepository : GenericRepository<UsersRoles>, IUsersRolesRepository
    {
        public UsersRolesRepository(ApplicationDbContext applicationDbContext) 
            : base(applicationDbContext)
        {
            
        }
    }
}
