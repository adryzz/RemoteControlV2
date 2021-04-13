using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteControlV2.Connection
{
    public class CommandEventArgs : EventArgs
    {
        public string Command;

        public CommandEventArgs(string c) => Command = c;
    }
}
