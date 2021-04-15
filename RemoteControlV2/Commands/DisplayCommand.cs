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
                        var handle = GetDesktopWindow();
                        SendMessage(handle, WM_SYSCOMMAND, (IntPtr)SC_MONITORPOWER, (IntPtr)(int)MonitorState.OFF);
                        Program.Connection.SendLine("Done!");
                        break;
                    }
                case "standby":
                    {
                        var handle = GetDesktopWindow();
                        SendMessage(handle, WM_SYSCOMMAND, (IntPtr)SC_MONITORPOWER, (IntPtr)(int)MonitorState.STANDBY);
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

            if (!EnumDisplayDevices(null, DisplayNumber, ref d, 0))
                throw new ArgumentOutOfRangeException("DisplayNumber", DisplayNumber, "Number is greater than connected displays.");

            if (0 != EnumDisplaySettings(
                d.DeviceName, ENUM_CURRENT_SETTINGS, ref dm))
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
                        dm.dmDisplayOrientation = DMDO_270;
                        break;
                    case Orientations.DEGREES_CW_180:
                        dm.dmDisplayOrientation = DMDO_180;
                        break;
                    case Orientations.DEGREES_CW_270:
                        dm.dmDisplayOrientation = DMDO_90;
                        break;
                    case Orientations.DEGREES_CW_0:
                        dm.dmDisplayOrientation = DMDO_DEFAULT;
                        break;
                    default:
                        break;
                }

                DISP_CHANGE ret = ChangeDisplaySettingsEx(
                    d.DeviceName, ref dm, IntPtr.Zero,
                    DisplaySettingsFlags.CDS_UPDATEREGISTRY, IntPtr.Zero);

                result = ret == 0;
            }

            return result;
        }

        [DllImport("user32.dll", SetLastError = false)]
        static extern IntPtr GetDesktopWindow();

        [DllImport("user32.dll", EntryPoint = "SendMessage", SetLastError = true)]
        static extern IntPtr SendMessage(IntPtr hWnd, Int32 Msg, IntPtr wParam, IntPtr lParam);

        private static int SC_MONITORPOWER = 0xF170;
        private static int WM_SYSCOMMAND = 0x0112;

        enum MonitorState
        {
            ON = -1,
            OFF = 2,
            STANDBY = 1
        }

        [DllImport("user32.dll")]
        internal static extern DISP_CHANGE ChangeDisplaySettingsEx(
            string lpszDeviceName, ref DEVMODE lpDevMode, IntPtr hwnd,
            DisplaySettingsFlags dwflags, IntPtr lParam);

        [DllImport("user32.dll")]
        internal static extern bool EnumDisplayDevices(
            string lpDevice, uint iDevNum, ref DISPLAY_DEVICE lpDisplayDevice,
            uint dwFlags);

        [DllImport("user32.dll", CharSet = CharSet.Ansi)]
        internal static extern int EnumDisplaySettings(
            string lpszDeviceName, int iModeNum, ref DEVMODE lpDevMode);

        public const int DMDO_DEFAULT = 0;
        public const int DMDO_90 = 1;
        public const int DMDO_180 = 2;
        public const int DMDO_270 = 3;

        public const int ENUM_CURRENT_SETTINGS = -1;

    }

    // See: https://msdn.microsoft.com/en-us/library/windows/desktop/dd183565(v=vs.85).aspx
    [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Ansi)]
    internal struct DEVMODE
    {
        public const int CCHDEVICENAME = 32;
        public const int CCHFORMNAME = 32;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCHDEVICENAME)]
        [FieldOffset(0)]
        public string dmDeviceName;
        [FieldOffset(32)]
        public Int16 dmSpecVersion;
        [FieldOffset(34)]
        public Int16 dmDriverVersion;
        [FieldOffset(36)]
        public Int16 dmSize;
        [FieldOffset(38)]
        public Int16 dmDriverExtra;
        [FieldOffset(40)]
        public DM dmFields;

        [FieldOffset(44)]
        Int16 dmOrientation;
        [FieldOffset(46)]
        Int16 dmPaperSize;
        [FieldOffset(48)]
        Int16 dmPaperLength;
        [FieldOffset(50)]
        Int16 dmPaperWidth;
        [FieldOffset(52)]
        Int16 dmScale;
        [FieldOffset(54)]
        Int16 dmCopies;
        [FieldOffset(56)]
        Int16 dmDefaultSource;
        [FieldOffset(58)]
        Int16 dmPrintQuality;

        [FieldOffset(44)]
        public POINTL dmPosition;
        [FieldOffset(52)]
        public Int32 dmDisplayOrientation;
        [FieldOffset(56)]
        public Int32 dmDisplayFixedOutput;

        [FieldOffset(60)]
        public short dmColor;
        [FieldOffset(62)]
        public short dmDuplex;
        [FieldOffset(64)]
        public short dmYResolution;
        [FieldOffset(66)]
        public short dmTTOption;
        [FieldOffset(68)]
        public short dmCollate;
        [FieldOffset(72)]
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCHFORMNAME)]
        public string dmFormName;
        [FieldOffset(102)]
        public Int16 dmLogPixels;
        [FieldOffset(104)]
        public Int32 dmBitsPerPel;
        [FieldOffset(108)]
        public Int32 dmPelsWidth;
        [FieldOffset(112)]
        public Int32 dmPelsHeight;
        [FieldOffset(116)]
        public Int32 dmDisplayFlags;
        [FieldOffset(116)]
        public Int32 dmNup;
        [FieldOffset(120)]
        public Int32 dmDisplayFrequency;
    }

    // See: https://msdn.microsoft.com/en-us/library/windows/desktop/dd183569(v=vs.85).aspx
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    internal struct DISPLAY_DEVICE
    {
        [MarshalAs(UnmanagedType.U4)]
        public int cb;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string DeviceName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string DeviceString;
        [MarshalAs(UnmanagedType.U4)]
        public DisplayDeviceStateFlags StateFlags;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string DeviceID;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string DeviceKey;
    }

    // See: https://msdn.microsoft.com/de-de/library/windows/desktop/dd162807(v=vs.85).aspx
    [StructLayout(LayoutKind.Sequential)]
    internal struct POINTL
    {
        long x;
        long y;
    }

    internal enum DISP_CHANGE : int
    {
        Successful = 0,
        Restart = 1,
        Failed = -1,
        BadMode = -2,
        NotUpdated = -3,
        BadFlags = -4,
        BadParam = -5,
        BadDualView = -6
    }

    // http://www.pinvoke.net/default.aspx/Enums/DisplayDeviceStateFlags.html
    [Flags()]
    internal enum DisplayDeviceStateFlags : int
    {
        /// <summary>The device is part of the desktop.</summary>
        AttachedToDesktop = 0x1,
        MultiDriver = 0x2,
        /// <summary>The device is part of the desktop.</summary>
        PrimaryDevice = 0x4,
        /// <summary>Represents a pseudo device used to mirror application drawing for remoting or other purposes.</summary>
        MirroringDriver = 0x8,
        /// <summary>The device is VGA compatible.</summary>
        VGACompatible = 0x10,
        /// <summary>The device is removable; it cannot be the primary display.</summary>
        Removable = 0x20,
        /// <summary>The device has more display modes than its output devices support.</summary>
        ModesPruned = 0x8000000,
        Remote = 0x4000000,
        Disconnect = 0x2000000
    }

    // http://www.pinvoke.net/default.aspx/user32/ChangeDisplaySettingsFlags.html
    [Flags()]
    internal enum DisplaySettingsFlags : int
    {
        CDS_NONE = 0,
        CDS_UPDATEREGISTRY = 0x00000001,
        CDS_TEST = 0x00000002,
        CDS_FULLSCREEN = 0x00000004,
        CDS_GLOBAL = 0x00000008,
        CDS_SET_PRIMARY = 0x00000010,
        CDS_VIDEOPARAMETERS = 0x00000020,
        CDS_ENABLE_UNSAFE_MODES = 0x00000100,
        CDS_DISABLE_UNSAFE_MODES = 0x00000200,
        CDS_RESET = 0x40000000,
        CDS_RESET_EX = 0x20000000,
        CDS_NORESET = 0x10000000
    }

    [Flags()]
    internal enum DM : int
    {
        Orientation = 0x00000001,
        PaperSize = 0x00000002,
        PaperLength = 0x00000004,
        PaperWidth = 0x00000008,
        Scale = 0x00000010,
        Position = 0x00000020,
        NUP = 0x00000040,
        DisplayOrientation = 0x00000080,
        Copies = 0x00000100,
        DefaultSource = 0x00000200,
        PrintQuality = 0x00000400,
        Color = 0x00000800,
        Duplex = 0x00001000,
        YResolution = 0x00002000,
        TTOption = 0x00004000,
        Collate = 0x00008000,
        FormName = 0x00010000,
        LogPixels = 0x00020000,
        BitsPerPixel = 0x00040000,
        PelsWidth = 0x00080000,
        PelsHeight = 0x00100000,
        DisplayFlags = 0x00200000,
        DisplayFrequency = 0x00400000,
        ICMMethod = 0x00800000,
        ICMIntent = 0x01000000,
        MediaType = 0x02000000,
        DitherType = 0x04000000,
        PanningWidth = 0x08000000,
        PanningHeight = 0x10000000,
        DisplayFixedOutput = 0x20000000
    }

}
