
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Media.SpeechSynthesis;
using Windows.Storage.Streams;

namespace Beats {
    public class SpeechNode : IDisposable{

        public SpeechNode() {
            synthesizer = new SpeechSynthesizer();
            VoiceInformation voiceInfo =
            (
                from voice in SpeechSynthesizer.AllVoices
                where voice.Gender == VoiceGender.Female
                select voice
            ).FirstOrDefault() ?? SpeechSynthesizer.DefaultVoice;

            synthesizer.Voice = voiceInfo;
        }

        private SpeechSynthesizer synthesizer;

        public async Task<IRandomAccessStream> TextToStream(string phrase) {          
            return await synthesizer.SynthesizeTextToStreamAsync(phrase);
        }

        public void Dispose() {
            synthesizer.Dispose();
        }
    }
}
