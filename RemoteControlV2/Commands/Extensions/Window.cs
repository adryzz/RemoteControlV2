using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RemoteControlV2.Commands.Extensions
{
    /// <summary>
    /// Allows easy management of windows
    /// </summary>
    public class Window
    {
        /// <summary>
        /// The handle that identifies the window
        /// </summary>
        public IntPtr Handle { get; }

        /// <summary>
        /// The window title
        /// </summary>
        public string Title {
            get
            {
                return GetWindowTitle(Handle); 
            }
            set
            {
                SetWindowText(Handle, value);
            }
        }

        /// <summary>
        /// The length of the window title (it can be very big)
        /// </summary>
        public int TitleLength {
            get
            {
                return GetWindowTextLength(Handle);
            }
        }

        /// <summary>
        /// The opacity of the window (0-255)
        /// </summary>
        public byte Opacity
        {
            get
            {
                uint crKey;
                byte bAlpha;
                uint dwFlags;
                GetLayeredWindowAttributes(Handle, out crKey, out bAlpha, out dwFlags);
                return bAlpha;
            }
            set
            {
                SetLayeredWindowAttributes(Handle, 0, value, LWA_ALPHA);
            }
        }

        /// <summary>
        /// Wheter or not the window is always on top
        /// </summary>
        public bool TopMost
        {
            get
            {
                int exStyle = GetWindowLong(Handle, GWL_EXSTYLE);
                return (exStyle & WS_EX_TOPMOST) == WS_EX_TOPMOST;
            }
            set
            {
                if (value)
                {
                    SetWindowPos(Handle, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE);
                }
                else
                {
                    SetWindowPos(Handle, HWND_NOTOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE);
                }
            }
        }

        /// <summary>
        /// Wheter or not the window is visible
        /// </summary>
        public bool Visible
        {
            get
            {
                return IsWindowVisible(Handle);
            }
            set
            {
                ShowWindow(Handle, value ? ShowWindowOptions.HIDE : ShowWindowOptions.SHOW);
            }
        }

        /// <summary>
        /// Wheter or not the window is enabled
        /// </summary>
        public bool Enabled
        {
            get
            {
                return IsWindowEnabled(Handle);
            }
            set
            {
                EnableWindow(Handle, value);
            }
        }

        /// <summary>
        /// The size of the window
        /// </summary>
        public Size Size;

        public static Window GetWindowFromTitle(string title)
        {
            return null;
        }

        public static Window GetActiveWindow()
        {
            return null;
        }

        public Window(IntPtr handle)
        {
            Handle = handle;
        }

        public void Close()
        {

        }

        public bool Exists()
        {
            return true;
        }

        public void Maximize()
        {
            ShowWindow(Handle, ShowWindowOptions.MAXIMIZE);
        }

        public void Minimize()
        {
            ShowWindow(Handle, ShowWindowOptions.MINIMIZE);
        }

        public void BringToFront()
        {

        }

        #region WinAPIs

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern int GetWindowTextLength(IntPtr hWnd);

        static string GetWindowTitle(IntPtr hWnd)
        {
            var length = GetWindowTextLength(hWnd) + 1;
            var title = new StringBuilder(length);
            GetWindowText(hWnd, title, length);
            return title.ToString();
        }

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool SetWindowText(IntPtr hwnd, String lpString);

        [DllImport("user32.dll")]
        static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern bool GetLayeredWindowAttributes(IntPtr hwnd, out uint crKey, out byte bAlpha, out uint dwFlags);

        const int GWL_EXSTYLE = -20;
        const int WS_EX_LAYERED = 0x80000;
        const int LWA_ALPHA = 0x2;
        const int LWA_COLORKEY = 0x1;

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);
        const UInt32 SWP_NOSIZE = 0x0001;
        const UInt32 SWP_NOMOVE = 0x0002;
        const UInt32 SWP_SHOWWINDOW = 0x0040;

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        const int WS_EX_TOPMOST = 0x0008;

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern bool IsWindowVisible(IntPtr hWnd);

        public enum ShowWindowOptions
        {
            FORCEMINIMIZE = 11,
            HIDE = 0,
            MAXIMIZE = 3,
            MINIMIZE = 6,
            RESTORE = 9,
            SHOW = 5,
            SHOWDEFAULT = 10,
            SHOWMAXIMIZED = 3,
            SHOWMINIMIZED = 2,
            SHOWMINNOACTIVE = 7,
            SHOWNA = 8,
            SHOWNOACTIVATE = 4,
            SHOWNORMAL = 1
        }

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern bool ShowWindow(IntPtr hWnd, ShowWindowOptions nCmdShow);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern bool IsWindowEnabled(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern bool EnableWindow(IntPtr hWnd, bool bEnable);
        #endregion
    }
}
