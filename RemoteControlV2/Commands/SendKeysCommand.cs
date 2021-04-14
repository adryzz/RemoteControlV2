using RemoteControlV2.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RemoteControlV2.Commands
{
    class SendKeysCommand : ICommand
    {
        public string Name => "sendkeys";

        public string Syntax => "Usage: 'sendkeys <keys>'";

        public bool Enabled { get; set; } = true;

        public void Execute(string arguments)
        {
            SendKeys.SendWait(arguments);
            Program.Connection.SendLine("Done!");
        }
    }
}
