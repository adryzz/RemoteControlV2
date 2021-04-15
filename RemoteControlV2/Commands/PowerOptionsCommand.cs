using RemoteControlV2.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RemoteControlV2.Commands
{
    class PowerOptionsCommand : ICommand
    {
        public string Name => "poweroptions";

        public string Syntax => "Usage: 'poweroptions <option>'";

        public bool Enabled { get; set; } = true;

        public void Execute(string arguments)
        {
            if (string.IsNullOrWhiteSpace(arguments))
            {
                Program.Connection.SendLine("Options available: shutdown, reboot, lock, suspend, hibernate");
                return;
            }
            switch(arguments)
            {
                case "shutdown":
                    {
                        ExitWindowsEx(0x00000001, 0x00000010);
                        break;
                    }
                case "reboot":
                    {
                        ExitWindowsEx(0x00000002, 0x00000010);
                        break;
                    }
                case "lock":
                    {
                        LockWorkStation();
                        break;
                    }
                case "suspend":
                    {
                        SetSuspendState(false, true, false);
                        break;
                    }
                case "hibernate":
                    {
                        SetSuspendState(true, true, true);
                        break;
                    }
            }
            Program.Connection.SendLine("Done!");
        }

        [DllImport("user32.dll")]
        public static extern int ExitWindowsEx(int uFlags, int dwReason);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool LockWorkStation();

        [DllImport("Powrprof.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern bool SetSuspendState(bool hiberate, bool forceCritical, bool disableWakeEvent);
    }
}
