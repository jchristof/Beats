using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Media.Audio;
using Windows.Storage;

namespace Beats {
    public class AudioSystem {

        private AudioGraph AudioGraph { get; set; }
        private AudioDeviceOutputNode DeviceOutput { get; set; }
        private AudioLoader AudioLoader { get; set; }

        public async Task Create() {
            AudioGraphSettings settings = new AudioGraphSettings(Windows.Media.Render.AudioRenderCategory.Media);

            CreateAudioGraphResult result = await AudioGraph.CreateAsync(settings);
            if (result.Status != AudioGraphCreationStatus.Success) {
                //ShowErrorMessage("AudioGraph creation error: " + result.Status.ToString());
            }

            AudioGraph = result.Graph;

            CreateAudioDeviceOutputNodeResult deviceOutputNodeResult = await AudioGraph.CreateDeviceOutputNodeAsync();
            if (deviceOutputNodeResult.Status != AudioDeviceNodeCreationStatus.Success) {
                // Cannot create device output node

                return;
            }

            DeviceOutput = deviceOutputNodeResult.DeviceOutputNode;
            AudioGraph.Start();
        }

        public async Task LoadAudio(StorageFolder storageFolder) {
            AudioLoader = new AudioLoader(AudioGraph, storageFolder);

            await AudioLoader.LoadAudio();

            foreach (KeyValuePair<BeatType, AudioFileInputNode> audioFileInputNode in AudioLoader.BeatMap) {
                audioFileInputNode.Value.AddOutgoingConnection(DeviceOutput);
            }
        }

        public void Play(BeatType beatType) {
            AudioLoader.BeatMap[beatType].Reset();
            AudioLoader.BeatMap[beatType].Start();
        }
    }
}
