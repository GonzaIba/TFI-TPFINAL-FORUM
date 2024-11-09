using Core.Contracts.Repositories;
using Core.Domain.IdentityModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Data.SQL.Repositories
{
    public class UsersRepository:GenericRepository<Users>,IUsersRepository
    {
        public UsersRepository(ApplicationGatewayDbContext applicationDbContext) 
            : base(applicationDbContext)
        {
            
        }
    }
}
