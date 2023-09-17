using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrossCutting.Helpers
{
    public static class RequestHelper
    {

        public static async Task<T> GetRequest<T>(string urlRequest)
        {
            try
            {
                T result = default;

                HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.GetAsync(urlRequest);
                string contents = response.Content.ReadAsStringAsync().Result;
                result = JsonConvert.DeserializeObject<T>(contents);

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
