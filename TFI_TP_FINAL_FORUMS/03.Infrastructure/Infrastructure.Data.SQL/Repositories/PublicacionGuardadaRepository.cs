using Core.Contracts.Repositories;
using Core.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data.SQL.Repositories
{
    public class PublicacionGuardadaRepository : GenericRepository<PublicacionGuardadaModel>, IPublicacionGuardadaRepository
    {
        public PublicacionGuardadaRepository(ApplicationDbContext applicationDbContext)
            : base(applicationDbContext)
        {

        }
    }
}
