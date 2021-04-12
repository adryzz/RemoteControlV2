using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteControlV2.Plugins
{
    public interface ICommand
    {
        string Name { get; }

        bool Enabled { get; set; }
        void Execute(string arguments);
    }
}
