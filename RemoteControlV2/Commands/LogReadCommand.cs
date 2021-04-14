using RemoteControlV2.Logging;
using RemoteControlV2.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteControlV2.Commands
{
    class LogReadCommand : ICommand
    {
        public string Name => "logread";

        public string Syntax => "Usage: 'logread <verbosity level>'";

        public bool Enabled { get; set; } = true;

        public void Execute(string arguments)
        {
            var value = CommandParser.Int32Parser(arguments);
            if (!value.HasValue)
            {
                throw new ArgumentException();
            }
            foreach (LogMessage m in Program.Logger.Logs)
            {
                if (value.Value >= (int)m.Severity)
                {
                    Program.Connection.SendLine($"[{m.Severity.ToString().ToUpper()}] | [{m.Type.ToString().ToUpper()}] {m.LogTime} | {m.Message.Replace("\n", "").Replace("\r", "")}");
                }
            }

        }
    }
}
