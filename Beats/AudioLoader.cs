using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Media.Audio;
using Windows.Storage;
using Beats.ViewModels;

namespace Beats {
    public class AudioLoader : IDisposable {
        private readonly Dictionary<BeatType, AudioFileInputNodeViewModel> BeatMap = new Dictionary<BeatType, AudioFileInputNodeViewModel>();
        private readonly AudioGraph audioGraph;
        private readonly StorageFolder audioFolder;
        private readonly AudioDeviceOutputNode deviceOutput;

        public AudioLoader(AudioGraph audioGraph, AudioDeviceOutputNode deviceOutput, StorageFolder audioFolder) {
            this.audioGraph = audioGraph ?? throw new ArgumentNullException(nameof(audioGraph));
            this.deviceOutput = deviceOutput ?? throw new ArgumentException(nameof(deviceOutput));
            this.audioFolder = audioFolder ?? throw new ArgumentNullException(nameof(audioFolder));
        }

        public async Task LoadAudio() {
            AudioFileInputNodeViewModel fileNode = new AudioFileInputNodeViewModel();
            await fileNode.LoadAudio(audioGraph, deviceOutput, audioFolder, "snare194.wav");

            BeatMap.Add(BeatType.Snare, fileNode);

            fileNode = new AudioFileInputNodeViewModel();
            await fileNode.LoadAudio(audioGraph, deviceOutput, audioFolder, "KICK 39.WAV");

            BeatMap.Add(BeatType.Kick, fileNode);

            fileNode = new AudioFileInputNodeViewModel();
            await fileNode.LoadAudio(audioGraph, deviceOutput, audioFolder, "crash-hi.wav");

            BeatMap.Add(BeatType.Crash, fileNode);

            fileNode = new AudioFileInputNodeViewModel();
            await fileNode.LoadAudio(audioGraph, deviceOutput, audioFolder, "Closed Hat 3.wav");

            BeatMap.Add(BeatType.Hat, fileNode);
        }

        public void Play(BeatType beatType) {
            BeatMap[beatType].Play();
        }


        public void Dispose() {
            foreach (KeyValuePair<BeatType, AudioFileInputNodeViewModel> audioFileInputNode in BeatMap) {
                audioFileInputNode.Value.Dispose();
            }
        }

    }
}
