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
        public event EventHandler<CommandEventArgs> OnCommandReceived;

        public SerialPort Port { get; private set; }

        public SerialConnectionMethod(string port, int baud)
        {
            Port = new SerialPort(port, baud, Parity.None, 8, StopBits.One);
            Port.DataReceived += Port_DataReceived;
        }

        private void Port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            OnCommandReceived?.Invoke(this, new CommandEventArgs(Port.ReadExisting()));
        }

        public void Initialize() => Port.Open();

        public void SendText(string text) => Port.Write(text);

        public void SendLine(string text) => Port.WriteLine(text);

        public void Dispose() => Port.Dispose();
    }
}
