using System;
using System.Collections.Concurrent;
using System.Threading;
using System.IO;

namespace RemoteControlV2.Logging
{
    public  class Logger : IDisposable
    {
        static Logger _Instance;

        public bool Disposed { get; private set; } = false;

        public LogSeverity ConsoleVerbosity = LogSeverity.Info;

        private Thread logThread;//the thread looping and saving logs from the queue

        private ConcurrentQueue<LogMessage> logQueue;//the thread-safe queue

        private  bool logging = true;//set to false to stop logging

        public  bool IsLogging => logThread.ThreadState == ThreadState.Running;

        private  StreamWriter runtimeLog;

        private  StreamWriter networkLog;

        private  StreamWriter commandsLog;


        private Logger()
        {

        }

        public static  Logger AllocateLogger()
        {
            if (_Instance == null || _Instance.Disposed)
            {
                _Instance = new Logger();
            }
            return _Instance;
        }


        public void Initialize()
        {
            logQueue = new ConcurrentQueue<LogMessage>();

            Directory.CreateDirectory("Logs");

            runtimeLog = File.AppendText(Path.Combine("Logs", "runtime.log"));
            networkLog = File.AppendText(Path.Combine("Logs", "network.log"));
            commandsLog = File.AppendText(Path.Combine("Logs", "commands.log"));

            logThread = new Thread(new ThreadStart(LogLoop));
            logThread.Name = "LoggerThread";
            logThread.IsBackground = true;
            logThread.Start();
        }

        public  void Log(LogType type, LogSeverity severity, string message)
        {
            LogMessage m = new LogMessage()
            {
                LogTime = DateTime.Now,
                Type = type,
                Severity = severity,
                Message = message
            };
            logQueue.Enqueue(m);
        }

        public  void Log(LogMessage message)
        {
            logQueue.Enqueue(message);
        }
        public void Dispose()
        {
            logging = false;
            runtimeLog.Flush();
            runtimeLog.Close();
            networkLog.Flush();
            networkLog.Close();
            commandsLog.Flush();
            commandsLog.Close();
            Disposed = true;
        }

        public  void Flush()
        {
            runtimeLog.Flush();
            networkLog.Flush();
            commandsLog.Flush();
        }

        private  void LogLoop()
        {
            while (logging)
            {
                //dequeue logs and save them to disk
                while (!logQueue.IsEmpty)
                {
                    LogMessage message;
                    if (!logQueue.TryDequeue(out message))
                    {
                        Thread.Sleep(50);//if queue is busy, wait 50ms
                    }
                    ConsoleLog(message);
                    DiskLog(message);
                }
                Thread.Sleep(100);
            }
        }

        private  void ConsoleLog(LogMessage message)
        {
            if ((int)ConsoleVerbosity <= (int)message.Severity)//log to console only if verbosity is lower or equal
            {
                Console.Write("[");
                Console.ForegroundColor = GetColor(message.Severity);
                Console.Write(message.Severity.ToString().ToUpper());
                Console.ResetColor();
                Console.Write($"] | [");
                Console.ForegroundColor = GetColor(message.Type);
                Console.Write(message.Type.ToString().ToUpper());
                Console.ResetColor();
                Console.WriteLine($"] {message.LogTime} | {message.Message.Replace("\n", "").Replace("\r", "")}");
            }
        }

        private  void DiskLog(LogMessage message)
        {
            string log = $"[{message.Severity.ToString().ToUpper()}] | {message.LogTime} | {message.Message.Replace("\n", "").Replace("\r", "")}\n";
            switch (message.Type)
            {
                case LogType.Runtime:
                    {
                        runtimeLog.Write(log);
                        break;
                    }
                case LogType.Network:
                    {
                        networkLog.Write(log);
                        break;
                    }
                case LogType.Commands:
                    {
                        commandsLog.Write(log);
                        break;
                    }
            }
        }

        public static ConsoleColor GetColor(LogSeverity severity)
        {
            switch (severity)
            {
                case LogSeverity.Trace:
                    {
                        return ConsoleColor.Gray;
                    }
                case LogSeverity.Debug:
                    {
                        return ConsoleColor.White;
                    }
                case LogSeverity.Info:
                    {
                        return ConsoleColor.Green;
                    }
                case LogSeverity.Warning:
                    {
                        return ConsoleColor.DarkYellow;
                    }
                case LogSeverity.Error:
                    {
                        return ConsoleColor.Red;
                    }
                case LogSeverity.Fatal:
                    {
                        return ConsoleColor.DarkRed;
                    }
                default:
                    {
                        return ConsoleColor.White;
                    }
            }
        }

        public static ConsoleColor GetColor(LogType type)
        {
            switch (type)
            {
                case LogType.Runtime:
                    {
                        return ConsoleColor.Gray;
                    }
                case LogType.Network:
                    {
                        return ConsoleColor.Green;
                    }
                case LogType.Commands:
                    {
                        return ConsoleColor.DarkYellow;
                    }

                default:
                    {
                        return ConsoleColor.Gray;
                    }
            }
        }
    }
}
