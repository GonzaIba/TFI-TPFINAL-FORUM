using Core.Contracts.Repositories;
using Core.Domain.Models;

namespace Infrastructure.Data.SQL.Repositories
{
    public class SolicitudAyudaChatMensajeRepository : GenericRepository<SolicitudAyudaChatMensajeModel>, ISolicitudAyudaChatMensajeRepository
    {
        public SolicitudAyudaChatMensajeRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {
        }
    }
}

