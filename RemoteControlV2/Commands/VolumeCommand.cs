using RemoteControlV2.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteControlV2.Commands
{
    class VolumeCommand : ICommand
    {
        public string Name => "volume";

        public bool Enabled { get; set; }

        public void Execute(string arguments)
        {
            throw new NotImplementedException();
        }
    }
}
