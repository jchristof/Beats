using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Devices.Enumeration;
using Windows.Media;
using Windows.Media.Audio;
using Windows.Storage;
using Windows.UI.Core;

namespace Beats {
    public class AudioSystem : IDisposable{

        private AudioGraph AudioGraph { get; set; }
        private AudioDeviceOutputNode DeviceOutput { get; set; }
        private AudioLoader AudioLoader { get; set; }
        private FrameNode FrameNode { get; set; }

        public async Task Create(DeviceInformation outputDevice) {
            AudioGraphSettings settings = new AudioGraphSettings(Windows.Media.Render.AudioRenderCategory.Media);
            settings.PrimaryRenderDevice = outputDevice;
            CreateAudioGraphResult result = await AudioGraph.CreateAsync(settings);

            if (result.Status != AudioGraphCreationStatus.Success) {
                Debug.WriteLine("AudioGraph creation error: " + result.Status);
                return;
            }

            AudioGraph = result.Graph;
            AudioGraph.UnrecoverableErrorOccurred += async (sender, args) => {

                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
                    CoreDispatcherPriority.Normal,
                    () => {
                        //on UI thread
                    });
                Debug.WriteLine(args.Error);
            };

            CreateAudioDeviceOutputNodeResult deviceOutputNodeResult = await AudioGraph.CreateDeviceOutputNodeAsync();
            if (deviceOutputNodeResult.Status != AudioDeviceNodeCreationStatus.Success) {
                Debug.WriteLine("Output device creation error: " + result.Status);
                return;
            }

            DeviceOutput = deviceOutputNodeResult.DeviceOutputNode;
            AudioGraph.Start();
        }

        public async Task LoadAudio(StorageFolder storageFolder) {
            AudioLoader = new AudioLoader(AudioGraph, DeviceOutput, storageFolder);

            await AudioLoader.LoadAudio();

            FrameNode = new FrameNode();
            FrameNode.Create(AudioGraph, DeviceOutput);
            //FrameNode.Start(880);
        }

        public void Play(BeatType beatType) {
            AudioLoader.Play(beatType);
        }

        public void Dispose() {
            AudioLoader?.Dispose();
            DeviceOutput?.Dispose();
            AudioGraph?.Dispose();
        }
    }
}
