using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CrossCutting.Helpers
{
    public static class ServerInfoHelper
    {
        public static string MapPath(string path)
        {
            return Path.Combine(AppDomain.CurrentDomain?.GetData("ContentRootPath")?.ToString() ?? "", path ?? "");
        }

        public static string MapPath2(string path)
        {
            return Path.Combine(AppDomain.CurrentDomain?.GetData("Infrastructura")?.ToString() ?? "", path);
        }
    }
}
