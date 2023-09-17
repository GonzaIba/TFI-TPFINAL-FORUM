using System;
using System.Collections.Generic;
using System.Text;

namespace CrossCutting.StorageService
{
    public enum GenericStorageTypeEnum
    {
        FSS, //File System Storage
        GCS, //Google Cloud Storage
        ABS  //Azure Blob Storage
    }
}
