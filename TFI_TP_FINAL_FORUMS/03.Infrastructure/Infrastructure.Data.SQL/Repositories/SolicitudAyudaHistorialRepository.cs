using Core.Contracts.Repositories;
using Core.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data.SQL.Repositories
{
    public class SolicitudAyudaHistorialRepository : GenericRepository<SolicitudAyudaHistorialModel>, ISolicitudAyudaHistorialRepository
    {
        public SolicitudAyudaHistorialRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {
            
        }
    }
}
