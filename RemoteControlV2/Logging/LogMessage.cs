using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteControlV2.Logging
{
    public struct LogMessage
    {
        public DateTime LogTime;
        public LogType Type;
        public LogSeverity Severity;
        public string Message;
    }
}
