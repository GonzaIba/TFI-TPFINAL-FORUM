using Core.Contracts.Repositories;
using Core.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data.SQL.Repositories
{
    public class RecompensaUsuarioRepository : GenericRepository<RecompensaUsuarioModel>, IRecompensaUsuarioRepository
    {
        public RecompensaUsuarioRepository(ApplicationDbContext applicationDbContext)
            : base(applicationDbContext)
        {

        }
    }
}
