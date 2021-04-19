using RemoteControlV2.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteControlV2.Commands
{
    class WindowCommand : ICommand
    {
        public string Name => "window";

        public string Syntax => "Usage: ";

        public bool Enabled { get; set; } = true;

        public void Execute(string arguments)
        {
            
        }
    }
}
