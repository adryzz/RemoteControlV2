using AudioSwitcher.AudioApi.CoreAudio;
using RemoteControlV2.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteControlV2.Commands
{
    class VolumeCommand : ICommand
    {
        public string Name => "volume";

        public string Syntax => "Usage: 'volume <percentage>' or 'volume <device> <percentage>'";

        public bool Enabled { get; set; } = true;

        CoreAudioController controller = new CoreAudioController();

        public void Execute(string arguments)
        {
            arguments = arguments.Replace("%", "");
            string[] arr = arguments.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (arr.Length < 2)
            {
                var value = CommandParser.Int32Parser(arr[0]);
                if (!value.HasValue)
                {
                    throw new ArgumentException();
                }
                controller.DefaultPlaybackDevice.Volume = value.Value;
                Program.Connection.SendLine($"Volume set to {value}%");
            }
            else
            {
                var number = CommandParser.Int32Parser(arr[0]);
                if (number.HasValue)
                {
                    var volume = CommandParser.Int32Parser(arr[1]);
                    if (!volume.HasValue)
                    {
                        throw new ArgumentException();
                    }
                    List<CoreAudioDevice> dev = controller.GetPlaybackDevices().ToList();
                    dev[number.Value].Volume = volume.Value;
                    Program.Connection.SendLine($"Volume of device {number} set to {volume}%");
                }
                else
                {
                    string name = "";
                    for(int i = 0; i < arr.Length - 1; i++)
                    {
                        name += arr[i] + " ";
                    }
                    name = name.Remove(name.Length-1).ToLower();
                    var volume = CommandParser.Int32Parser(arr[arr.Length-1]);
                    if (!volume.HasValue)
                    {
                        throw new ArgumentException();
                    }
                    List<CoreAudioDevice> dev = controller.GetPlaybackDevices().ToList();
                    foreach (CoreAudioDevice d in dev)
                    {
                        if (d.Name.ToLower().Contains(name) || d.Name.ToLower().Contains(name) || d.InterfaceName.ToLower().Contains(name))
                        {
                            d.Volume = volume.Value;
                            Program.Connection.SendLine($"Volume of device {d.FullName} set to {volume}%");
                        }
                    }
                }
            }
        }
    }
}
