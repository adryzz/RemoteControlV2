using System;
using System.IO;
using Newtonsoft.Json;
using RemoteControlV2.Logging;

namespace RemoteControlV2
{
    public class Configuration
    {
        public string Port = "";
        public int BaudRate = 115200;
        public int NetPort = 69;
        public bool ShowIcon = true;
        public bool ForceCommandsOnNewThread = false;
        public bool RestartOnUnhandledException = false;
        public int MaxUnhandledExceptions = 4;
        public LogSeverity ConsoleVerbosity = LogSeverity.Info;

        public void Save(string fileName)
        {
            string json = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText(fileName, json);
        }

        public static Configuration FromFile(string fileName)
        {
            string json = File.ReadAllText(fileName);
            return JsonConvert.DeserializeObject<Configuration>(json);
        }
    }
}