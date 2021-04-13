using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;

namespace RemoteControlV2.Connection
{
    public class SerialConnectionMethod : IConnectionMethod
    {
        public event EventHandler OnCommandReceived;

        public SerialPort Port { get; private set; }

        public SerialConnectionMethod(string port, int baud)
        {
            Port = new SerialPort(port, baud, Parity.None, 8, StopBits.One);
        }

        public void Initialize() => Port.Open();

        public void SendText(string text) => Port.Write(text);

        public void Dispose() => Port.Dispose();
    }
}
