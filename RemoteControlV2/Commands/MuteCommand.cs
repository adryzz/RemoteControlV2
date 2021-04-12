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
            switch(arguments)
            {
                case "0":
                case "off":
                case "false":
                    {
                        break;
                    }
                case "1":
                case "on":
                case "true":
                    {
                        break;
                    }
            }
        }
    }
}
