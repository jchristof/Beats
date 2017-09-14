using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Media;
using Windows.Media.Audio;
using Windows.Storage;

namespace Beats {
    public class AudioSystem : IDisposable{

        private AudioGraph AudioGraph { get; set; }
        private AudioDeviceOutputNode DeviceOutput { get; set; }
        private AudioLoader AudioLoader { get; set; }
        private FrameNode FrameNode { get; set; }

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
                audioFileInputNode.Value.Stop();
            }

            FrameNode = new FrameNode();
            FrameNode.Create(AudioGraph, DeviceOutput);
            //FrameNode.Start(880);
        }

        public void Play(BeatType beatType) {
            AudioLoader.BeatMap[beatType].Reset();
            AudioLoader.BeatMap[beatType].Start();
        }

        public void Dispose() {
            AudioLoader.Dispose();
            DeviceOutput.Dispose();
            AudioGraph.Dispose();
        }
    }
}
