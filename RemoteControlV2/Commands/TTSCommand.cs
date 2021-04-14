using RemoteControlV2.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RemoteControlV2.Logging;
using System.Speech.Synthesis;

namespace RemoteControlV2.Commands
{
    class TTSCommand : ICommand
    {
        public string Name => "tts";

        public string Syntax => "Usage: 'tts <text>'";

        public bool Enabled { get; set; } = true;

        public void Execute(string arguments)
        {
            SpeechSynthesizer synth = new SpeechSynthesizer();
            synth.SetOutputToDefaultAudioDevice();
            synth.SpeakAsync(arguments);
            Program.Connection.SendLine("Done!");
        }
    }
}