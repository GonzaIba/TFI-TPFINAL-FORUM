using Core.Contracts.Repositories;
using Core.Domain.ML;
using Core.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data.SQL.Repositories
{
    public class ArchivoRepository : GenericRepository<ArchivoModel>, IArchivoRepository
    {
        public ArchivoRepository(ApplicationDbContext applicationDbContext)
            : base(applicationDbContext)
        {

        }
    }
}
