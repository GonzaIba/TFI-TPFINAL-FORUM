using Core.Contracts.Repositories;
using Core.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data.SQL.Repositories
{
    public class SolicitudAyudaEtiquetasRepository : GenericRepository<SolicitudAyudaEtiquetasModel>, ISolicitudAyudaEtiquetasRepository
    {
        public SolicitudAyudaEtiquetasRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {
            
        }
    }
}
