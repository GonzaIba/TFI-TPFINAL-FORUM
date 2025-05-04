using Core.Domain.Models;
using Core.Domain.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Contracts.Services
{
    public interface IPublicacionService : IGenericService<PublicacionModel>
    {
        Task<bool> CreatePublication(string userId, PublicacionModel publication);
        Task<bool> SavePublication(string userId, int codePublication);
        Task<bool> DeleteSavedPublication(string userId, int codePublication); 
        Task<IEnumerable<PublicacionModel>> GetPublications();
        Task<PublicacionModel> GetDetailPublication(int codePublication);
        Task<IEnumerable<PublicacionModel>> GetCreatedPublicationByUser(string userId);
        Task<IEnumerable<PublicacionModel>> GetSavedPublications(string userId);
        Task<IEnumerable<string>> PredictLabel(string texto);
        Task<IEnumerable<PublicacionModel>> GetPublicationByFilter(string texto);
    }
}