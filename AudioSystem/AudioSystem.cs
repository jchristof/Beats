using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Media.Audio;
using Windows.UI.Core;

namespace AudioSystem {
    public class AudioSystem : IDisposable {

        public AudioGraph AudioGraph { get; set; }
        public AudioDeviceOutputNode DeviceOutput { get; set; }

        public async Task Create(AudioGraphSettings settings) {
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

        public void Dispose() {
            DeviceOutput?.Dispose();
            AudioGraph?.Dispose();
        }
    }
}
