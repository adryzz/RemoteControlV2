using RemoteControlV2.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RemoteControlV2.Commands
{
    class ClipboardCommand : ICommand
    {
        public string Name => "clipboard";

        public string Syntax => "Usage: 'clipboard get' or 'clipboard set <text>' or 'clipboard clear'";

        public bool Enabled { get; set; } = true;

        public void Execute(string arguments)
        {
            string[] arr = arguments.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            string s = arguments;
            if (arr.Length > 2)
            {
                s = arr[0];
            }
            switch(s)
            {
                case "get":
                    {
                        Program.Connection.SendLine(Clipboard.GetText());
                        return;
                    }
                case "set":
                    {
                        Program.Connection.SendLine("Done!");
                        Clipboard.SetText(arguments.Remove(0, 4));
                        return;
                    }
                case "clear":
                    {
                        Clipboard.Clear();
                        Program.Connection.SendLine("Done!");
                        return;
                    }
            }
        }
    }
}
