using Newtonsoft.Json;

namespace BorisProxy
{
    class ConfigRoot
    {
        [JsonProperty("run")]
        public dynamic run {get; set; }

        [JsonProperty("exec")]
        public dynamic exec {get; set;}
    }
}