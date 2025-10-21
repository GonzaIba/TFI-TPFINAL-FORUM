using Core.Domain.Models;
using Core.Domain.Request;
using Core.Domain.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Contracts.Services
{
    public interface ISesionAyudaService : IGenericService<SesionAyudaModel>
    {
        public Task<bool> AcceptTyC(AcceptTyCRequest request);
        public Task<TerminosCondicionesModel> GetTyC();
        public Task<SesionAyudaModel> GetSession(int codeRequestHelp, string userId);
        public Task<SesionAyudaModel> EnterSession(EnterSessionRequest request);
    }
}
