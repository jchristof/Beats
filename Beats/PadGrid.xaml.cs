
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Media.Audio;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Beats {
    public sealed partial class PadGrid : UserControl {
        public PadGrid() {
            this.InitializeComponent();
            InitAudioGraph();
        }

        AudioGraph audioGraph;

        private AudioFileInputNode fileInput;
        private AudioDeviceOutputNode deviceOutput;

        //audioGraph.Dispose();

        private async Task InitAudioGraph() {

            AudioGraphSettings settings = new AudioGraphSettings(Windows.Media.Render.AudioRenderCategory.Media);

            CreateAudioGraphResult result = await AudioGraph.CreateAsync(settings);
            if (result.Status != AudioGraphCreationStatus.Success) {
                //ShowErrorMessage("AudioGraph creation error: " + result.Status.ToString());
            }

            audioGraph = result.Graph;

            CreateAudioDeviceOutputNodeResult deviceOutputNodeResult = await audioGraph.CreateDeviceOutputNodeAsync();
            if (deviceOutputNodeResult.Status != AudioDeviceNodeCreationStatus.Success) {
                // Cannot create device output node

                return;
            }

            deviceOutput = deviceOutputNodeResult.DeviceOutputNode;

            var audtioFolder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync(@"Assets\audio");
            StorageFile file = await audtioFolder.GetFileAsync("kick 03.wav");

            CreateAudioFileInputNodeResult fileInputResult = await audioGraph.CreateFileInputNodeAsync(file);
            if (AudioFileNodeCreationStatus.Success != fileInputResult.Status) {
                // Cannot read input file
                return;
            }

            fileInput = fileInputResult.FileInputNode;
            fileInput.AddOutgoingConnection(deviceOutput);
            audioGraph.Start();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e) {
            throw new System.NotImplementedException();
        }

    }
}
