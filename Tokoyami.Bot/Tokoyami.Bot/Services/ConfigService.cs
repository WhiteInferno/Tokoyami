using System.IO;
using Newtonsoft.Json;

namespace Tokoyami.Bot.Services
{
    public interface IConfigServices
    {
        Config GetConfig();
    }

    public class ConfigService : IConfigServices
    {
        public Config GetConfig()
        {
            var file = "settings.json";
            var data = File.ReadAllText(file);
            return JsonConvert.DeserializeObject<Config>(data);
        }
    }
}
