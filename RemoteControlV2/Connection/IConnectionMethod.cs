using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteControlV2.Connection
{
    public interface IConnectionMethod : IDisposable
    {
        void Initialize();

        event EventHandler<CommandEventArgs> OnCommandReceived;

        void SendText(string text);

        void SendLine(string text);
    }
}
