using System;
using System.Collections.Generic;
using System.Text;
using CrossCutting.StorageService.Contracts;

namespace CrossCutting.StorageService.Factory
{
    public interface IGenericStorageServiceFactory
    {
        IGenericStorageService GetDefault();

        IGenericStorageService Get(GenericStorageTypeEnum storageType);

        IGenericStorageService Get(string storageType);
    }
}
