
using System;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Storage;
using Beats.Events;

namespace Beats {
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage {
        public MainPage() {
            InitializeComponent();
        }

        private AudioSystem audioSystem = new AudioSystem();

        private async Task InitAudioGraph(DeviceInformation outputDevice) {
            await audioSystem.Create(outputDevice);

            StorageFolder audioFolder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync(@"Assets\audio");
            await audioSystem.LoadAudio(audioFolder);
        }

        private async void OutputSelectionControl_OnOutputDeviceSelectionChanged(object sender, OutputDeviceChangedEvent e) {
            audioSystem?.Dispose();

            audioSystem = new AudioSystem();
            await InitAudioGraph(e.OutputDevice);
            await PadGrid.InitGridPad(audioSystem);
        }

    }
}
