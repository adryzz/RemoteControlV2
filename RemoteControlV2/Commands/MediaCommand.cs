using RemoteControlV2.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RemoteControlV2.Commands
{
    class MediaCommand : ICommand
    {
        public string Name => "media";

        public string Syntax => "Usage: 'media pause' or 'media next'";

        public bool Enabled { get; set; } = true;

        public void Execute(string arguments)
        {
            string[] arr = arguments.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            switch(arr[0])
            {
                case "play":
                    {
                        keybd_event(VK_MEDIA_PLAY, 0, KEYEVENTF_EXTENDEDKEY, IntPtr.Zero);
                        keybd_event(VK_MEDIA_PLAY, 0, KEYEVENTF_KEYUP, IntPtr.Zero);
                        break;
                    }
                case "pause":
                    {
                        keybd_event(VK_MEDIA_PAUSE, 0, KEYEVENTF_EXTENDEDKEY, IntPtr.Zero);
                        keybd_event(VK_MEDIA_PAUSE, 0, KEYEVENTF_KEYUP, IntPtr.Zero);
                        break;
                    }
                case "playpause":
                    {
                        keybd_event(VK_MEDIA_PLAY_PAUSE, 0, KEYEVENTF_EXTENDEDKEY, IntPtr.Zero);
                        keybd_event(VK_MEDIA_PLAY_PAUSE, 0, KEYEVENTF_KEYUP, IntPtr.Zero);
                        break;
                    }
                case "previous":
                    {
                        keybd_event(VK_MEDIA_PREV_TRACK, 0, KEYEVENTF_EXTENDEDKEY, IntPtr.Zero);
                        keybd_event(VK_MEDIA_PREV_TRACK, 0, KEYEVENTF_KEYUP, IntPtr.Zero);
                        break;
                    }
                case "next":
                    {
                        keybd_event(VK_MEDIA_NEXT_TRACK, 0, KEYEVENTF_EXTENDEDKEY, IntPtr.Zero);
                        keybd_event(VK_MEDIA_NEXT_TRACK, 0, KEYEVENTF_KEYUP, IntPtr.Zero);
                        break;
                    }
                case "stop":
                    {
                        keybd_event(VK_MEDIA_STOP, 0, KEYEVENTF_EXTENDEDKEY, IntPtr.Zero);
                        keybd_event(VK_MEDIA_STOP, 0, KEYEVENTF_KEYUP, IntPtr.Zero);
                        break;
                    }
                case "fastforward":
                    {
                        keybd_event(VK_MEDIA_FAST_FORWARD, 0, KEYEVENTF_EXTENDEDKEY, IntPtr.Zero);
                        keybd_event(VK_MEDIA_FAST_FORWARD, 0, KEYEVENTF_KEYUP, IntPtr.Zero);
                        break;
                    }
                case "rewind":
                    {
                        keybd_event(VK_MEDIA_REWIND, 0, KEYEVENTF_EXTENDEDKEY, IntPtr.Zero);
                        keybd_event(VK_MEDIA_REWIND, 0, KEYEVENTF_KEYUP, IntPtr.Zero);
                        break;
                    }
            }
            Program.Connection.SendLine("Done!");
        }

        [DllImport("user32.dll", SetLastError = true)]
        static extern void keybd_event(byte virtualKey, byte scanCode, uint flags, IntPtr extraInfo);
        const int VK_MEDIA_NEXT_TRACK = 0xB0;
        const int VK_MEDIA_PREV_TRACK = 0xB1;
        const int VK_MEDIA_PLAY_PAUSE = 0xB3;
        const int VK_MEDIA_PLAY = 0xFA;
        const int VK_MEDIA_PAUSE = 0x13;
        const int VK_MEDIA_STOP = 0xB2;
        const int VK_MEDIA_FAST_FORWARD = 0x31;
        const int VK_MEDIA_REWIND = 0x32;
        const int KEYEVENTF_EXTENDEDKEY = 0x0001; //Key down flag
        const int KEYEVENTF_KEYUP = 0x0002; //Key up flag
    }
}
