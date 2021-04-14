using RemoteControlV2.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteControlV2.Commands
{
    class ReloadCommand : ICommand
    {
        public string Name => "reload";

        public bool Enabled { get; set; } = true;

        public void Execute(string arguments)
        {
            Program.Logger.Log(Logging.LogType.Commands, Logging.LogSeverity.Info, "Reloading all settings...");
            Program.Connection.SendLine("Reloading all settings...");
        }
    }
}
