
using System;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Storage;
using Beats.Events;
using Beats.ViewModels;

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

        private void PadGrid_OnAudioFileSelected(object sender, AudioFileInputNodeViewModel e) {
            AudioNodePropertyModifierControl.DataContext = e;
        }

        private async void AudioNodePropertyModifierControl_OnPickNewAudioFile(object sender, object e) {
                
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".wav");
            picker.FileTypeFilter.Add(".mp3");

            Windows.Storage.StorageFile file = await picker.PickSingleFileAsync();
            if (file == null)
                return;


            await PadGrid.LoadAudio((sender as AudioFileInputNodeViewModel).Id, file);
        }

    }
}
