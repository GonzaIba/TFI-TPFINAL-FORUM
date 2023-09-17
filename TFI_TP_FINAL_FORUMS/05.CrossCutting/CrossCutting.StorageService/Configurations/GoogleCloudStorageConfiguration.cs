using System;
using System.Collections.Generic;
using System.Text;

namespace CrossCutting.StorageService.Configurations
{
    public class GoogleCloudStorageConfiguration
    {
        public string ProjectId { get; set; }
        public string BucketName { get; set; }
        public string CredFile { get; set; }
    }
}
