using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteControlV2.Connection
{
    class AggregateConnectionMethod : IConnectionMethod
    {
        public event EventHandler<CommandEventArgs> OnCommandReceived;

        IConnectionMethod method1;
        IConnectionMethod method2;

        public AggregateConnectionMethod(IConnectionMethod a, IConnectionMethod b)
        {
            method1 = a;
            method2 = b;
        }

        public void Initialize()
        {
            method1.Initialize();
            method2.Initialize();
            method1.OnCommandReceived += (s, e) => OnCommandReceived?.Invoke(s, e);
            method2.OnCommandReceived += (s, e) => OnCommandReceived?.Invoke(s, e);
        }

        public void SendLine(string text)
        {
            method1.SendLine(text);
            method2.SendLine(text);
        }

        public void SendText(string text)
        {
            method1.SendText(text);
            method2.SendText(text);
        }

        public void Dispose()
        {
            method1.Dispose();
            method2.Dispose();
        }
    }
}
