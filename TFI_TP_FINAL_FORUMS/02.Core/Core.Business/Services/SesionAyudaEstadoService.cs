using Core.Contracts.Repositories;
using Core.Contracts.Services;
using Core.Contracts.UoW;
using Core.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Business.Services
{
    public class SesionAyudaEstadoService : GenericService<SesionAyudaEstadoModel>, ISesionAyudaEstadoService
    {
        public SesionAyudaEstadoService(IUnitOfWorkForum unitOfWork)
            : base(unitOfWork, unitOfWork.GetRepository<ISesionAyudaEstadoRepository>())
        {
        }
    }
}
