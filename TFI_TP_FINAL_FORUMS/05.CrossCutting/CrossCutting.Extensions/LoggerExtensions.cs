using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace CrossCutting.Extensions
{
    public static class LoggerExtensions
    {
        public static IDisposable BeginNamedScope(this ILogger logger, string name, params ValueTuple<string, object>[] properties)
        {
            var dictionary = properties.ToDictionary(p => p.Item1, p => p.Item2);
            dictionary[name + ".Scope"] = Guid.NewGuid().ToString();

            //return logger.BeginScope(new Dictionary<string, object> { { "Scope", dictionary } });
            return logger.BeginScope(dictionary);
        }
    }
}
