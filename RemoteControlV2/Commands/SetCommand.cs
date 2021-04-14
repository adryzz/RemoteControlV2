using RemoteControlV2.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RemoteControlV2.Logging;

namespace RemoteControlV2.Commands
{
    class SetCommand : ICommand
    {
        public string Name => "set";

        public bool Enabled { get; set; } = true;

        public void Execute(string arguments)
        {
            string[] arr = arguments.Split(' ');
            switch(arr[0])
            {
                case "ConsoleVerbosity":
                    {
                        int? value = CommandParser.Int32Parser(arr[1]);
                        if (!value.HasValue)
                        {
                            throw new ArgumentException();
                        }
                        Program.Config.ConsoleVerbosity = (LogSeverity)value;
                        Program.Connection.SendText("Set console verbosity to " + value);
                        break;
                    }
            }
            Program.Config.Save("config.json");
        }
    }
}
