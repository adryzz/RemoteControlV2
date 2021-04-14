using RemoteControlV2.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RemoteControlV2.Logging;

namespace RemoteControlV2.Commands
{
    class CommandsListCommand : ICommand
    {
        public string Name => "commandslist";

        public string Syntax => "Usage: 'commandslist'";

        public bool Enabled { get; set; } = true;

        public void Execute(string arguments)
        {
            string commands = "List of all installed commands: ";
            foreach (ICommand c in Program.Commands)
            {
                commands += c.Name + ", ";
            }
            
            Program.Connection.SendLine(commands.Remove(commands.Length - 2));
        }
    }
}