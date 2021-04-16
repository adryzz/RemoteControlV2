using RemoteControlV2.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AudioSwitcher.AudioApi.CoreAudio;

namespace RemoteControlV2.Commands
{
    class MuteCommand : ICommand
    {
        public string Name => "mute";

        public string Syntax => "Usage: 'mute <state>' or 'mute <device> state'";

        public bool Enabled { get; set; } = true;

        CoreAudioController controller = new CoreAudioController();

        public void Execute(string arguments)
        {
            string[] arr = arguments.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (arr.Length < 2)
            {
                var state = CommandParser.BooleanParser(arguments);
                if (!state.HasValue)
                {
                    throw new ArgumentException();
                }
                controller.DefaultPlaybackDevice.Mute(state.Value);
                Program.Connection.SendLine("Mute set to " + state.ToString());
            }
            else
            {
                var number = CommandParser.Int32Parser(arr[0]);
                if (number.HasValue)
                {
                    var state = CommandParser.BooleanParser(arr[1]);
                    if (!state.HasValue)
                    {
                        throw new ArgumentException();
                    }
                    List<CoreAudioDevice> dev = controller.GetPlaybackDevices().ToList();
                    dev[number.Value].Mute(state.Value);
                    Program.Connection.SendLine($"Mute of device {number} set to {state}");
                }
                else
                {
                    string name = "";
                    for (int i = 0; i < arr.Length - 1; i++)
                    {
                        name += arr[i] + " ";
                    }
                    name = name.Remove(name.Length - 1).ToLower();
                    var state = CommandParser.BooleanParser(arr[1]);
                    if (!state.HasValue)
                    {
                        throw new ArgumentException();
                    }
                    List<CoreAudioDevice> dev = controller.GetPlaybackDevices().ToList();
                    foreach (CoreAudioDevice d in dev)
                    {
                        if (d.Name.ToLower().Contains(name) || d.Name.ToLower().Contains(name) || d.InterfaceName.ToLower().Contains(name))
                        {
                            d.Mute(state.Value);
                            Program.Connection.SendLine($"Mute of device {d.FullName} set to {state}");
                        }
                    }
                }
            }
        }
    }
}
