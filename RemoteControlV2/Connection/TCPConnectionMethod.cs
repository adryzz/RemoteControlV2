using RemoteControlV2.Connection.TCP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using RemoteControlV2.Logging;

namespace RemoteControlV2.Connection
{
    class TCPConnectionMethod : IConnectionMethod
    {
        public event EventHandler<CommandEventArgs> OnCommandReceived;

        Server server;
        public TCPConnectionMethod(int port)
        {
            IPAddress localAddr = IPAddress.Parse("127.0.0.1");
            server = new Server(localAddr, port);
            server.MessageReceived += Server_MessageReceived;
            server.ClientConnected += Server_ClientConnected;
            server.ClientDisconnected += Server_ClientDisconnected;
            server.ConnectionBlocked += Server_ConnectionBlocked;
        }

        private void Server_MessageReceived(Client c, string message)
        {
            OnCommandReceived?.Invoke(this, new CommandEventArgs(message));
        }

        public void Initialize()
        {
            server.Start();
            server.AllowIncomingConnections();
        }

        public void SendLine(string text)
        {
            server.SendMessageToAll(text + "\n");
        }

        public void SendText(string text)
        {
            server.SendMessageToAll(text);
        }

        public void Dispose()
        {
            server.Stop();
        }

        private void Server_ConnectionBlocked(IPEndPoint endPoint)
        {
            Program.Logger.Log(LogType.Network, LogSeverity.Error, $"Connection refused at {endPoint.Address}:{endPoint.Port}.");
        }

        private void Server_ClientDisconnected(Client c)
        {
            IPEndPoint endPoint = c.GetRemoteAddress();
            Program.Logger.Log(LogType.Network, LogSeverity.Error, $"Client disconnected at {endPoint.Address}:{endPoint.Port}.");
        }

        private void Server_ClientConnected(Client c)
        {
            IPEndPoint endPoint = c.GetRemoteAddress();
            Program.Logger.Log(LogType.Network, LogSeverity.Error, $"Client connected at {endPoint.Address}:{endPoint.Port}.");
        }
    }
}
