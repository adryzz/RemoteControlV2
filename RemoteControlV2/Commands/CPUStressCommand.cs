using RemoteControlV2.Plugins;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RemoteControlV2.Commands
{
    class CPUStressCommand : ICommand
    {
        public string Name => "cpustress";

        public string Syntax => "Usage: 'cpustress <number of threads>'";

        public bool Enabled { get; set; } = true;

        List<Thread> runningThreads = new List<Thread>();

        List<CancellationTokenSource> tokens = new List<CancellationTokenSource>();

        public void Execute(string arguments)
        {
            var value = CommandParser.Int32Parser(arguments);
            if (!value.HasValue)
            {
                throw new ArgumentException();
            }
            value++;
            if (value.Value == 1)
            {
                for (int i = 0; i < runningThreads.Count; i++)
                {
                    removeThread();
                }
                Program.Connection.SendLine("Done!");
                return;
            }
            else if (runningThreads.Count < value.Value)
            {
                for (int i = 0; i <  value.Value - runningThreads.Count; i++)
                {
                    addThread();
                }
            }
            else if (runningThreads.Count > value.Value)
            {
                for (int i = 0; i < runningThreads.Count - value.Value; i++)
                {
                    removeThread();
                }
            }
            Program.Connection.SendLine("Done!");
        }

        private void removeThread()
        {
            if (runningThreads.Count > 0)
            {
                tokens[0].Cancel();
                tokens[0].Dispose();
                tokens.RemoveAt(0);
                runningThreads.RemoveAt(0);
            }
        }

        private void addThread()
        {
            var source = new CancellationTokenSource();
            tokens.Add(source);
            Thread t = new Thread(new ThreadStart(() => CPUKill(source.Token)));
            t.Name = "Load Thread";
            t.Start();
            runningThreads.Add(t);
        }

        public static void CPUKill(CancellationToken token, int cpuUsage = 100)
        {
            Parallel.For(0, 1, new Action<int>((int i) =>
            {
                Stopwatch watch = new Stopwatch();
                watch.Start();
                while (!token.IsCancellationRequested)
                {
                    if (watch.ElapsedMilliseconds > cpuUsage)
                    {
                        Thread.Sleep(100 - cpuUsage);
                        watch.Reset();
                        watch.Start();
                    }
                }
            }));
        }
    }
}
