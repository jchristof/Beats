using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Media.Audio;
using Windows.Storage;

namespace Beats {
    public class AudioLoader : IDisposable{
        public Dictionary<BeatType, AudioFileInputNode> BeatMap = new Dictionary<BeatType, AudioFileInputNode>();
        private readonly AudioGraph audioGraph;
        private readonly StorageFolder audioFolder;

        public AudioLoader(AudioGraph audioGraph, StorageFolder audioFolder) {
            this.audioGraph = audioGraph ?? throw new ArgumentNullException(nameof(audioGraph));
            this.audioFolder = audioFolder ?? throw new ArgumentNullException(nameof(audioFolder));
        }

        public async Task LoadAudio() {
            BeatMap.Add(BeatType.Snare, await LoadAudio("snare194.wav"));
            BeatMap.Add(BeatType.Kick, await LoadAudio("KICK 39.WAV"));
            BeatMap.Add(BeatType.Crash, await LoadAudio("crash-hi.wav"));
            BeatMap.Add(BeatType.Hat, await LoadAudio("Closed Hat 3.wav"));
        }

        private async Task<AudioFileInputNode> LoadAudio(string filename) {
            StorageFile file = await audioFolder.GetFileAsync(filename);

            CreateAudioFileInputNodeResult fileInputResult = await audioGraph.CreateFileInputNodeAsync(file);
            if (AudioFileNodeCreationStatus.Success == fileInputResult.Status)
                return fileInputResult.FileInputNode;
            
            throw new InvalidOperationException($"Unable to load audio file {filename}");
        }

        public void Dispose() {
            foreach (KeyValuePair<BeatType, AudioFileInputNode> audioFileInputNode in BeatMap) {
                audioFileInputNode.Value.Dispose();
            }
        }

    }
}
