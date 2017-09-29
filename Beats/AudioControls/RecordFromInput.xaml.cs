
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
            InitializeComponent();
        }

        private RecordToMemoryModule recordToMemory;
        RecordFromInputViewModel ViewModel => DataContext as RecordFromInputViewModel;

        public async Task CreateAsync(AudioSystem audioSystem) {
            recordToMemory = new RecordToMemoryModule(audioSystem);
            DataContext = new RecordFromInputViewModel();

            var inputDevices = await DeviceInformation.FindAllAsync(DeviceClass.AudioCapture);
            foreach (var device in inputDevices) {
                ViewModel.RecordingDevices.Add(device);
            }

            var defaultId = MediaDevice.GetDefaultAudioCaptureId(AudioDeviceRole.Default);
            ViewModel.SelectedInputDevice = ViewModel.RecordingDevices.FirstOrDefault(d => d.Id == defaultId);
        }

        private bool started;
        private void ButtonBase_OnClick(object sender, RoutedEventArgs e) {
            if (started) {
                recordToMemory.Stop();
                started = false;
            }
            else {
                recordToMemory.Start();
                started = true;
            }

        }

        private async void Create_Click(object sender, RoutedEventArgs e) {
            Create.IsEnabled = false;
            await recordToMemory.CreateAsync(ViewModel.SelectedInputDevice);
            Transport.IsEnabled = true;
        }
    }
}
