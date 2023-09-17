using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrossCutting.Helpers
{
    public static class ImageHelper
    {
        
        public static async Task<string> ProcessImageProfile(IFormFile ProfileImage, string[] ValidExtensions)
        {
            //var blobConnectionString = _configuration.GetConnectionString("StorageConnectionString");
            //var blobContainerName = _configuration["AzureStorage:containerName"];
            //var blobBaseUrl = _configuration["AzureStorage:baseUrl"];
            //var profileImagePath = $"{CompanyId}/Imgs/Profile/{ProfileImage.FileName}";
            //string UrlProfileImage = string.Empty;

            if (IsValidFileExtension(ProfileImage, ValidExtensions))
            {
                MemoryStream img = new MemoryStream();
                await ProfileImage.CopyToAsync(img);
                Stream memoryStream = ProfileImage.OpenReadStream();

                //UrlProfileImage = $"{blobBaseUrl}{blobContainerName}/{profileImagePath}";
                ////subimos el archivo al Azure Storage
                //await Helper.UploadStorage(blobConnectionString, blobContainerName, profileImagePath, memoryStream);
            }

            return "";
        }


        
        #region Helper
        private static bool IsValidFileExtension(IFormFile fileobject, string[] migrationValidExtensions)
        {
            //string[] migrationValidExtensions = { ".CSV", ".XLS", ".XLSX" };

            if (string.IsNullOrEmpty(fileobject.FileName) || fileobject == null || fileobject.Length == 0)
            {
                return false;
            }

            bool flag = false;
            string ext = Path.GetExtension(fileobject.FileName);
            if (string.IsNullOrEmpty(ext))
            {
                return false;
            }

            ext = ext.ToUpperInvariant();

            if (migrationValidExtensions.Contains(ext))
            {
                return true;
            }

            return flag;
        }
        #endregion
    }
}
