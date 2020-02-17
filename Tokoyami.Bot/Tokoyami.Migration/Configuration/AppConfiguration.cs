using Microsoft.Extensions.Configuration;
using System.Collections.Concurrent;

namespace Tokoyami.Context.Configuration
{
    public class AppConfiguration
    {
        private static readonly ConcurrentDictionary<string, IConfigurationRoot> _configurationCache;

        static AppConfiguration()
        {
            _configurationCache = new ConcurrentDictionary<string, IConfigurationRoot>();
        }

        public static IConfigurationRoot Get(string path, string environmentName = null, bool addUserSecrets = false)
        {
            var cacheKey = path + "#" + environmentName + "#" + addUserSecrets;
            return _configurationCache.GetOrAdd(
                cacheKey,
                _ => BuildConfiguration(path, environmentName, addUserSecrets)
            );
        }

        private static IConfigurationRoot BuildConfiguration(string path, string enviromentName = null, bool addUserSecrets = false)
        {

            IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(path)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            if (!string.IsNullOrEmpty(enviromentName))
                builder = builder.AddJsonFile($"appsettings.{enviromentName}.json", optional: true);


            if (addUserSecrets) {
                builder.AddUserSecrets<AppConfiguration>();
            }
            return builder.Build();
        }
    }
}
