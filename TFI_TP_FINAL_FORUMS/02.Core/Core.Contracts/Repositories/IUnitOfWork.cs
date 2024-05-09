using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Contracts.Repositories
{
    public interface IUnitOfWork : IUnitOfWorkBase
    {
        public I GetRepositoryForum<I>();
        public I GetRepositoryGateway<I>();
    }
}
