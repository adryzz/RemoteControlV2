using RemoteControlV2.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RemoteControlV2.Commands
{
    class WGetCommand : ICommand
    {
        public string Name => "wget";

        public string Syntax => "Usage: 'wget <url> <local path>' or 'wget <url> <local path> <log level>'";

        public bool Enabled { get; set; } = true;

        private int logLevel = 1;

        private int previousPerc = 0;

        private WebClient client;

        public void Execute(string arguments)
        {
            string[] arr = arguments.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (arguments.Equals("abort"))
            {
                if (client != null)
                {
                    client.CancelAsync();
                    client.Dispose();
                    client = null;
                    Program.Connection.SendLine("Download aborted.");
                }
                else
                {
                    Program.Connection.SendLine("There was nothing to abort.");
                }
                return;
            }
            if (client != null)
            {
                Program.Connection.SendLine("A download is still in progress.");
                return;
            }
            if (arr.Length < 2)
            {
                throw new ArgumentException();
            }
            else if (arr.Length < 3)
            {
                string url = arr[0];
                string path = arr[1];
                client = new WebClient();
                client.DownloadProgressChanged += C_DownloadProgressChanged;
                client.DownloadFileCompleted += C_DownloadFileCompleted;
                client.DownloadFileAsync(new Uri(url), path);
                Program.Connection.SendLine("Download started!");
            }
            else
            {
                string url = arr[0];
                string path = arr[1];
                var log = CommandParser.Int32Parser(arr[2]);
                if (!log.HasValue || log.Value > 2 || log.Value < 0)
                {
                    throw new ArgumentException();
                }
                logLevel = log.Value;
                client = new WebClient();
                client.DownloadProgressChanged += C_DownloadProgressChanged;
                client.DownloadFileCompleted += C_DownloadFileCompleted;
                client.DownloadFileAsync(new Uri(url), path);
                Program.Connection.SendLine("Download started!");
            }
        }

        private void C_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (client != null)
            {
                Program.Connection.SendLine("Download completed!");
                client.Dispose();
                client = null;
            }
            logLevel = 1;
            previousPerc = 0;
        }

        private void C_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage != previousPerc)
            {
                switch (logLevel)
                {
                    case 0:
                        {
                            Program.Connection.SendLine($"Download progress: {e.ProgressPercentage}%");
                            break;
                        }
                    case 1:
                        {
                            if (e.ProgressPercentage % 10 == 0)
                            {
                                Program.Connection.SendLine($"Download progress: {e.ProgressPercentage}%");
                            }
                            break;
                        }
                }
                previousPerc = e.ProgressPercentage;
            }
        }
    }
}
