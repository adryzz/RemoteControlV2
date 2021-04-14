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

        public string Syntax => "Usage: 'mute <state>'";

        public bool Enabled { get; set; } = true;

        CoreAudioController controller = new CoreAudioController();

        public void Execute(string arguments)
        {
            var state = CommandParser.BooleanParser(arguments);
            if (!state.HasValue)
            {
                throw new ArgumentException();
            }
            controller.DefaultPlaybackDevice.Mute(state.Value);
            Program.Connection.SendLine("Mute set to " + state.ToString());
        }
    }
}
