using Core.Contracts.Repositories;
using Core.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data.SQL.Repositories
{
    public class FilterRepository : GenericRepository<FilterModel>, IFilterRepository
    {
        public FilterRepository(ApplicationGatewayDbContext applicationDbContext)
        : base(applicationDbContext)
        {

        }
    }
}
