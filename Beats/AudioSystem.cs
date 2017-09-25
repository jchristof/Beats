using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Devices.Enumeration;
using Windows.Media.Audio;
using Windows.Storage;
using Windows.UI.Core;

namespace Beats {
    public class AudioSystem : IDisposable{

        public AudioGraph AudioGraph { get; set; }
        public AudioDeviceOutputNode DeviceOutput { get; set; }
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

            var echo = new EchoEffectDefinition(AudioGraph);
            echo.Delay = 409;
            echo.Feedback = 0.5;
            echo.WetDryMix = 0.6;
            DeviceOutput.EffectDefinitions.Add(echo);
            AudioGraph.Start();
        }

        public async Task LoadAudio(StorageFolder storageFolder) {
            FrameNode = new FrameNode();
            FrameNode.Create(AudioGraph, DeviceOutput);
            //FrameNode.Start(880);
        }

        public void Dispose() {
            DeviceOutput?.Dispose();
            AudioGraph?.Dispose();
        }
    }
}
