using Core.Contracts.Repositories;
using Core.Domain.Models;

namespace Infrastructure.Data.SQL.Repositories
{
    public class SolicitudAyudaChatMensajeLecturaRepository : GenericRepository<SolicitudAyudaChatMensajeLecturaModel>, ISolicitudAyudaChatMensajeLecturaRepository
    {
        public SolicitudAyudaChatMensajeLecturaRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {
        }
    }
}

