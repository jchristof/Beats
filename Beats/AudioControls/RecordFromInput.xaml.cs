
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Media.Devices;
using Windows.UI.Xaml;

namespace Beats.AudioControls
{
    public sealed partial class RecordFromInput 
    {
        public RecordFromInput()
        {
            this.InitializeComponent();
            DataContext = new RecordFromInputViewModel();
            Create();
        }

        RecordFromInputViewModel ViewModel => DataContext as RecordFromInputViewModel;

        public async void Create() {
            
            var inputDevices = await DeviceInformation.FindAllAsync(DeviceClass.AudioCapture);
            foreach (var device in inputDevices) {
                ViewModel.RecordingDevices.Add(device);
            }

            var defaultId = MediaDevice.GetDefaultAudioCaptureId(AudioDeviceRole.Default);
            ViewModel.SelectedInputDevice = ViewModel.RecordingDevices.FirstOrDefault(d => d.Id == defaultId);
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e) {
            
        }

    }
}
