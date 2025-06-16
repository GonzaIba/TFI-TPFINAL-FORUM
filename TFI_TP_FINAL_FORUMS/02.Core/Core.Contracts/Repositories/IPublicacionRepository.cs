using Core.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Contracts.Repositories
{
    public interface IPublicacionRepository : IGenericRepository<PublicacionModel>
    {
        Task<List<(PublicacionModel pub, float score)>> GetRelatedAsync(int id, int limit = 5);
        Task<IEnumerable<PublicacionModel>> GetTopPublicationsLastWeek();
    }
}
