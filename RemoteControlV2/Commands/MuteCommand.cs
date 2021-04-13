using RemoteControlV2.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteControlV2.Commands
{
    class MuteCommand : ICommand
    {
        public string Name => "mute";

        public bool Enabled { get; set; }

        public void Execute(string arguments)
        {
            var state = CommandParser.BooleanParser(arguments);
            if (!state.HasValue)
            {
                throw new ArgumentException();
            }
            Program.Logger.Log(Logging.LogType.Commands, Logging.LogSeverity.Info, "Mute set to " + state.ToString());
            Program.Connection.SendText("Mute set to " + state.ToString());
        }
    }
}
