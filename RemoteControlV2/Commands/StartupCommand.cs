using RemoteControlV2.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteControlV2.Commands
{
    class StartupCommand : ICommand
    {
        public string Name => "startup";

        public string Syntax => "Usage: 'startup set' or 'startup remove'";

        public bool Enabled { get; set; }

        public void Execute(string arguments)
        {
            if (string.IsNullOrWhiteSpace(arguments))
            {
                throw new ArgumentException();
            }
            switch(arguments)
            {
                case "set":
                    {
                        break;
                    }
                case "remove":
                    {
                        break;
                    }
            }
        }
    }
}
