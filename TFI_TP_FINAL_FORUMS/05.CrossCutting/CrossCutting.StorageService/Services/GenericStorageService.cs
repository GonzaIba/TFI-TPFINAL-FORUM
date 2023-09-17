using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CrossCutting.StorageService.Contracts;

namespace CrossCutting.StorageService.Services
{
    public abstract class GenericStorageService : IGenericStorageService
    {
        public abstract Task<GenericStoreResult> Store(string filename, string contentFile);
        public abstract Task<string> Restore(string volume, string fileFullPath);
        public abstract void Delete(string volume, string fileFullPath);
    }
}
