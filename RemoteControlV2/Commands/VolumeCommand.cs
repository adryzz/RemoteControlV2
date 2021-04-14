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

        public string Syntax => "Usage: 'volume <percentage>'";

        public bool Enabled { get; set; } = true;

        public void Execute(string arguments)
        {
            var value = CommandParser.Int32Parser(arguments);
            if (!value.HasValue)
            {
                throw new ArgumentException();
            }
        }
    }
}
