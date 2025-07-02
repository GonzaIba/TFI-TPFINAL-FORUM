using Microsoft.AspNetCore.Http;

namespace CrossCutting.Helpers
{
    public static class ImageHelper
    {
        public const string UserDefault = "data:image/png;base64,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";

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
