
using System;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Media.Audio;
using Windows.Storage;
using Windows.UI.Xaml;
using Beats.Dialogs;
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
            AudioGraphSettings settings = new AudioGraphSettings(Windows.Media.Render.AudioRenderCategory.Media);
            settings.PrimaryRenderDevice = outputDevice;

            await audioSystem.Create(settings);

            StorageFolder audioFolder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync(@"Assets\audio");
            await audioSystem.LoadAudio(audioFolder);

            await RecordFromInput.CreateAsync(audioSystem);
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

            StorageFile file = await picker.PickSingleFileAsync();
            if (file == null)
                return;


            await PadGrid.LoadAudio((sender as AudioFileInputNodeViewModel).Id, file);
        }

        private async void MainPage_OnLoaded(object sender, RoutedEventArgs e) {
            var dialog = new SelectOuputDeviceDialog();
            await dialog.ShowAsync();

            await InitAudioGraph(dialog.Result);
            await PadGrid.InitGridPad(audioSystem);
        }

    }
}
