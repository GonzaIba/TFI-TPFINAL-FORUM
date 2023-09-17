using Core.Domain.ML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Contracts.Services
{
    public interface IQuestionService : IGenericService<QuestionML>
    {
        Task<IEnumerable<QuestionML>> GetQuestionsAsync();
        Task AddQuestionAsync(QuestionML question);
    }
}
