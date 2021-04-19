using RemoteControlV2.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
namespace RemoteControlV2.Commands
{
    class VersionCommand : ICommand
    {
        public string Name => "version";

        public string Syntax => "Usage: 'version'";

        public bool Enabled { get; set; } = true;

        public void Execute(string arguments)
        {
            Program.Connection.SendLine(Assembly.GetExecutingAssembly().GetName().Version.ToString());
        }
    }
}
