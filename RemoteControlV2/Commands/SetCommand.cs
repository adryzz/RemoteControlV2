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

        public string Syntax => "Usage: set <setting> <value>";

        public bool Enabled { get; set; } = true;

        public void Execute(string arguments)
        {
            string[] arr = arguments.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
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
                        Program.Connection.SendLine("Set console verbosity to " + value);
                        break;
                    }
                case "Port":
                    {
                        if (arr.Length < 2)
                        {
                            throw new ArgumentException();
                        }
                        Program.Config.Port = arr[1];
                        Program.Connection.SendLine("Set port to " + arr[1]);
                        break;
                    }
                case "BaudRate":
                    {
                        int? value = CommandParser.Int32Parser(arr[1]);
                        if (!value.HasValue)
                        {
                            throw new ArgumentException();
                        }
                        Program.Config.BaudRate = value.Value;
                        Program.Connection.SendLine("Set baud rate to " + value.Value + "bps");
                        break;
                    }
                case "ShowIcon":
                    {
                        var value = CommandParser.BooleanParser(arr[1]);
                        if (!value.HasValue)
                        {
                            throw new ArgumentException();
                        }
                        Program.Config.ShowIcon = value.Value;
                        Program.Connection.SendLine("Set taskbar icon visibility to " + value.Value);
                        break;
                    }
            }
            Program.Config.Save("config.json");
        }
    }
}
