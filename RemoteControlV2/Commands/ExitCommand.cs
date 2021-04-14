using RemoteControlV2.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RemoteControlV2.Logging;

namespace RemoteControlV2.Commands
{
    class ExitCommand : ICommand
    {
        public string Name => "exit";

        public string Syntax => "Usage: 'exit'";

        public bool Enabled { get; set; } = true;

        public void Execute(string arguments)
        {
            Program.Exit(0);
        }
    }
}