using RemoteControlV2.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RemoteControlV2.Commands.Extensions;

namespace RemoteControlV2.Commands
{
    class WindowCommand : ICommand
    {
        public string Name => "window";

        public string Syntax => "Usage: 'window get Window <title>' or 'window set <handle> <property>'";

        public bool Enabled { get; set; } = true;

        public void Execute(string arguments)
        {
            string[] arr = arguments.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            switch(arr[0])
            {
                case "get":
                    {
                        GetWindow(arr.Skip(1).ToArray());
                        break;
                    }
                case "set":
                    {
                        SetWindow(arr.Skip(1).ToArray());
                        break;
                    }
            }
        }

        private void GetWindow(string[] arr)
        {
            switch(arr[0])
            {
                case "ActiveWindow":
                    {
                        Window w = Window.GetActiveWindow();
                        Program.Connection.SendLine($"The active window handle is {w.Handle}");
                        break;
                    }
                case "Window":
                    {
                        Window w = Window.GetWindowFromTitle(arr[1]);
                        Program.Connection.SendLine($"The window handle is {w.Handle}");
                        break;
                    }
                case "WindowInfo":
                    {
                        Window w = new Window(new IntPtr(CommandParser.Int32Parser(arr[1]).Value));
                        StringBuilder s = new StringBuilder();
                        s.AppendLine($"Handle: {w.Handle}");
                        bool exists = w.Exists();
                        s.AppendLine($"Exists: {exists}");
                        if (exists)
                        {
                            s.AppendLine($"Title: {w.Title}");
                            s.AppendLine($"Visible: {w.Visible}");
                            s.AppendLine($"Enabled: {w.Enabled}");
                            s.AppendLine($"TopMost: {w.TopMost}");
                            s.AppendLine($"Opacity: {w.Opacity}/255");
                            var v = w.Size;
                            s.AppendLine($"Size: {v.Width}x{v.Height} px");
                            Program.Connection.SendText(s.ToString());
                        }
                        break;
                    }
            }
        }

        private void SetWindow(string[] arr)
        {
            switch(arr[0])
            {
                case "Title":
                    {
                        Window w = new Window(new IntPtr(CommandParser.Int32Parser(arr[1]).Value));
                        string newTitle = string.Join(" ", arr.Skip(2));
                        w.Title = newTitle;
                        break;
                    }
                case "Enabled":
                    {
                        Window w = new Window(new IntPtr(CommandParser.Int32Parser(arr[1]).Value));
                        w.Enabled = CommandParser.BooleanParser(arr[2]).Value;
                        break;
                    }
                case "Visible":
                    {
                        Window w = new Window(new IntPtr(CommandParser.Int32Parser(arr[1]).Value));
                        w.Visible = CommandParser.BooleanParser(arr[2]).Value;
                        break;
                    }
                case "TopMost":
                    {
                        Window w = new Window(new IntPtr(CommandParser.Int32Parser(arr[1]).Value));
                        w.TopMost = CommandParser.BooleanParser(arr[2]).Value;
                        break;
                    }
                case "Opacity":
                    {
                        Window w = new Window(new IntPtr(CommandParser.Int32Parser(arr[1]).Value));
                        w.Opacity = CommandParser.ByteParser(arr[2]).Value;
                        break;
                    }
            }
            Program.Connection.SendLine("Done!");
        }
    }
}
