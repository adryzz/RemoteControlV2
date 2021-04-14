using RemoteControlV2.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RemoteControlV2.Logging;

namespace RemoteControlV2.Commands
{
    class HelpCommand : ICommand
    {
        public string Name => "help";

        public string Syntax => "help <command>";

        public bool Enabled { get; set; } = true;

        public void Execute(string arguments)
        {
            if (string.IsNullOrWhiteSpace(arguments))
            {
                Program.Connection.SendLine(Syntax);
            }
            else
            {
                bool found = false;
                foreach (ICommand c in Program.Commands)
                {
                    if (c.Name.Equals(arguments))
                    {
                        found = true;
                        Program.Connection.SendLine(c.Syntax);
                    }
                }

                if (!found)
                {
                    Program.Connection.SendLine($"Command {arguments} not found!");
                }
            }
        }
    }
}