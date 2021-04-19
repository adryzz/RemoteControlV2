using RemoteControlV2.Commands.Extensions;
using RemoteControlV2.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RemoteControlV2.Commands
{
    class DisplayCommand : ICommand
    {
        public string Name => "display";

        public string Syntax => "Usage: 'display rotate <screen index> <degrees>' or 'display off'";

        public bool Enabled { get; set; } = true;

        public void Execute(string arguments)
        {
            string[] arr = arguments.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            switch (arr[0])
            {
                case "off":
                    {
                        var handle = Display.GetDesktopWindow();
                        Display.SendMessage(handle, Display.WM_SYSCOMMAND, (IntPtr)Display.SC_MONITORPOWER, (IntPtr)(int)Display.MonitorState.OFF);
                        Program.Connection.SendLine("Done!");
                        break;
                    }
                case "standby":
                    {
                        var handle = Display.GetDesktopWindow();
                        Display.SendMessage(handle, Display.WM_SYSCOMMAND, (IntPtr)Display.SC_MONITORPOWER, (IntPtr)(int)Display.MonitorState.STANDBY);
                        Program.Connection.SendLine("Done!");
                        break;
                    }
                case "rotate":
                    {
                        RotateDisplay(arr[1], arr[2]);
                        break;
                    }
            }
        }

        private void RotateDisplay(string v1, string v2)
        {
            var value = CommandParser.Int32Parser(v1);
            if (!value.HasValue)
            {
                throw new ArgumentException();
            }

            if (v1.Length == 2)
            {
                var value1 = CommandParser.Int32Parser(v2);
                if (!value1.HasValue)
                {
                    throw new ArgumentException();
                }
                Rotate((uint)value.Value, (Orientations)value1.Value);
                Program.Connection.SendLine("Screen " + value.ToString() + " rotated to " + value1.ToString() + "°");
                return;
            }

            Rotate(0, (Orientations)value.Value);
            Program.Connection.SendLine("Screen 0 Rotated to " + value.ToString() + "°");
            return;
        }

        public enum Orientations
        {
            DEGREES_CW_0 = 0,
            DEGREES_CW_90 = 90,
            DEGREES_CW_180 = 180,
            DEGREES_CW_270 = 360
        }

        public static bool Rotate(uint DisplayNumber, Orientations Orientation)
        {
            bool result = false;
            DISPLAY_DEVICE d = new DISPLAY_DEVICE();
            DEVMODE dm = new DEVMODE();
            d.cb = Marshal.SizeOf(d);

            if (!Display.EnumDisplayDevices(null, DisplayNumber, ref d, 0))
                throw new ArgumentOutOfRangeException("DisplayNumber", DisplayNumber, "Number is greater than connected displays.");

            if (Display.EnumDisplaySettings(
                d.DeviceName, Display.ENUM_CURRENT_SETTINGS, ref dm) != 0)
            {
                if ((dm.dmDisplayOrientation + (int)Orientation) % 2 == 1) // Need to swap height and width?
                {
                    int temp = dm.dmPelsHeight;
                    dm.dmPelsHeight = dm.dmPelsWidth;
                    dm.dmPelsWidth = temp;
                }

                switch (Orientation)
                {
                    case Orientations.DEGREES_CW_90:
                        dm.dmDisplayOrientation = 3;
                        break;
                    case Orientations.DEGREES_CW_180:
                        dm.dmDisplayOrientation = 2;
                        break;
                    case Orientations.DEGREES_CW_270:
                        dm.dmDisplayOrientation = 1;
                        break;
                    case Orientations.DEGREES_CW_0:
                        dm.dmDisplayOrientation = 0;
                        break;
                    default:
                        break;
                }

                DISP_CHANGE ret = Display.ChangeDisplaySettingsEx(
                    d.DeviceName, ref dm, IntPtr.Zero,
                    DisplaySettingsFlags.CDS_UPDATEREGISTRY, IntPtr.Zero);

                result = ret == 0;
            }

            return result;
        }
    }
}
