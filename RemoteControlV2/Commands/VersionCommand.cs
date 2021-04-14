using RemoteControlV2.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RemoteControlV2.Commands
{
    class VersionCommand : ICommand
    {
        public string Name => "version";

        public string Syntax => "Usage: 'version'";

        public bool Enabled { get; set; }

        public void Execute(string arguments)
        {
            Program.Connection.SendLine(Application.ProductVersion);
        }
    }
}
