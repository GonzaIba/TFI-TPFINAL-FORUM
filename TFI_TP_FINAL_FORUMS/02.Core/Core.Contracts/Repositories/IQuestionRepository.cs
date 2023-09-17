using Core.Domain.IdentityModels;
using Core.Domain.ML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Contracts.Repositories
{
    public interface IQuestionRepository : IGenericRepository<QuestionML>
    {
    }
}
