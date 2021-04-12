using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteControlV2.Plugins
{
    public interface IPlugin : IDisposable
    {
        IReadOnlyCollection<ICommand> Commands { get; }

        void Initialize();
    }
}
