using RemoteControlV2.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using RemoteControlV2.Commands.Extensions;

namespace RemoteControlV2.Commands
{
    class ProcessCommand : ICommand
    {
        public string Name => "process";

        public string Syntax => "Usage: 'process list <query>' or 'process start <exe path>' or 'process kill/suspend/resume <id>'";

        public bool Enabled { get; set; } = true;

        public void Execute(string arguments)
        {
            string[] arr = arguments.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            switch(arr[0])
            {
                case "list":
                    {
                        if (arr.Length == 1)
                        {
                            ListProcesses("");
                        }
                        else
                        {
                            ListProcesses(string.Join(" ", arr.Skip(1)));
                        }
                        break;
                    }
                case "start":
                    {
                        StartProcess(arr);
                        break;
                    }
                case "kill":
                    {
                        KillProcess(arr);
                        break;
                    }
                case "suspend":
                    {
                        SuspendProcess(arr);
                        break;
                    }
                case "resume":
                    {
                        ResumeProcess(arr);
                        break;
                    }
            }
        }

        private void ListProcesses(string query)
        {
            List<Process> processes = new List<Process>();
            if (string.IsNullOrWhiteSpace(query))
            {
                processes.AddRange(Process.GetProcesses());
            }
            else
            {
                processes.AddRange(Process.GetProcessesByName(query));
            }
            StringBuilder builder = new StringBuilder();
            foreach (Process p in processes)
            {
                if (string.IsNullOrWhiteSpace(p.MainWindowTitle))
                {
                    builder.AppendLine($"[{p.Id}] - {p.ProcessName}.exe");
                }
                else
                {
                    builder.AppendLine($"[{p.Id}] - {p.ProcessName}.exe - {p.MainWindowTitle}");
                }
            }
            Program.Connection.SendText(builder.ToString());
        }

        private void StartProcess(string[] arr)
        {
            Process p = null;
            if (arr.Length == 1)
            {
                throw new ArgumentException();
            }
            else if (arr.Length == 2)
            {
                p = Process.Start(arr[1]);
            }
            else
            {
                p = Process.Start(arr[1], string.Join(" ", arr.Skip(2)));
            }
            if (p != null)
            {
                Program.Connection.SendLine($"The process {p.Id} has been started.");
            }
            else
            {
                Program.Connection.SendLine($"An error has occurred.");
            }
        }

        private void KillProcess(string[] arr)
        {
            var pid = CommandParser.Int32Parser(arr[1]);
            if (!pid.HasValue)
            {
                throw new ArgumentException();
            }
            Process p = Process.GetProcessById(pid.Value);
            if (p != null && !p.HasExited)
            {
                p.Kill();
                Program.Connection.SendLine($"Done!");
            }
            else
            {
                Program.Connection.SendLine($"The process does not exist.");
            }
        }

        private void SuspendProcess(string[] arr)
        {
            var pid = CommandParser.Int32Parser(arr[1]);
            if (!pid.HasValue)
            {
                throw new ArgumentException();
            }
            Process p = Process.GetProcessById(pid.Value);
            if (p != null && !p.HasExited)
            {
                p.Suspend();
                Program.Connection.SendLine($"Done!");
            }
            else
            {
                Program.Connection.SendLine($"The process does not exist.");
            }
        }

        private void ResumeProcess(string[] arr)
        {
            var pid = CommandParser.Int32Parser(arr[1]);
            if (!pid.HasValue)
            {
                throw new ArgumentException();
            }
            Process p = Process.GetProcessById(pid.Value);
            if (p != null && !p.HasExited)
            {
                p.Resume();
                Program.Connection.SendLine($"Done!");
            }
            else
            {
                Program.Connection.SendLine($"The process does not exist.");
            }
        }
    }
}
