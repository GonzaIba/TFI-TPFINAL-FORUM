using Core.Contracts.Repositories;
using Core.Domain.IdentityModels;
using Core.Domain.ML;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data.SQL.Repositories
{
    public class QuestionRepository : GenericRepository<QuestionML>, IQuestionRepository
    {
        public QuestionRepository(ApplicationDbContext applicationDbContext)
            : base(applicationDbContext)
        {

        }
    }
}
