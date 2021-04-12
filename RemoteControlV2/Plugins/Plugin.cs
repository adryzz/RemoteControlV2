using Newtonsoft.Json;
using System;
using System.Reflection;

namespace RemoteControlV2.Plugins
{
    [Serializable()]
    public class Plugin
    {
        public string AssemblyName = "";
        public bool Enabled = true;

        [JsonIgnore()]
        public Assembly Assembly;
        [JsonIgnore()]
        public Type PluginType;
        [JsonIgnore()]
        public bool Loaded = false;
        [JsonIgnore()]
        public int UnhandledExceptions = 0;
        [JsonIgnore()]
        public IPlugin Instance;
    }
}