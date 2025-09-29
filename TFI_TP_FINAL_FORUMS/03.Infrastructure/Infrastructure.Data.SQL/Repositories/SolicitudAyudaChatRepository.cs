using Core.Contracts.Repositories;
using Core.Domain.Models;

namespace Infrastructure.Data.SQL.Repositories
{
    public class SolicitudAyudaChatRepository : GenericRepository<SolicitudAyudaChatModel>, ISolicitudAyudaChatRepository
    {
        public SolicitudAyudaChatRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {
        }
    }
}

