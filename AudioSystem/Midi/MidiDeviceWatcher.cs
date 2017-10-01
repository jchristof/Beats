using System;
using System.Linq;
using Windows.Devices.Enumeration;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;

namespace AudioSystem.Midi
{
    public class MidiDeviceWatcher
    {
        public MidiDeviceWatcher(string midiDeviceSelectorString, ComboBox deviceListControl, CoreDispatcher dispatcher) {
            this.midiDeviceSelectorString = midiDeviceSelectorString;
            this.deviceListControl = deviceListControl;
            coreDispatcher = dispatcher;

            deviceWatcher = DeviceInformation.CreateWatcher(midiDeviceSelectorString);
            deviceWatcher.Added += DeviceWatcher_Added;
            deviceWatcher.Removed += DeviceWatcher_Removed;
            deviceWatcher.Updated += DeviceWatcher_Updated;
            deviceWatcher.EnumerationCompleted += DeviceWatcher_EnumerationCompleted;
        }

        private DeviceWatcher deviceWatcher;
        private readonly ComboBox deviceListControl;
        private readonly CoreDispatcher coreDispatcher;
        private readonly string midiDeviceSelectorString;
        public DeviceInformationCollection DeviceInformationCollection { get; set; }

        public void StartWatcher() {
            deviceWatcher.Start();
        }
        public void StopWatcher() {
            deviceWatcher.Stop();
        }

        ~MidiDeviceWatcher() {
            deviceWatcher.Added -= DeviceWatcher_Added;
            deviceWatcher.Removed -= DeviceWatcher_Removed;
            deviceWatcher.Updated -= DeviceWatcher_Updated;

            deviceWatcher = null;
        }

        private async void DeviceWatcher_Removed(DeviceWatcher sender, DeviceInformationUpdate args) {
            await coreDispatcher.RunAsync(CoreDispatcherPriority.High, UpdateDevices);
        }

        private async void DeviceWatcher_Added(DeviceWatcher sender, DeviceInformation args) {
            await coreDispatcher.RunAsync(CoreDispatcherPriority.High, UpdateDevices);
        }

        private async void DeviceWatcher_EnumerationCompleted(DeviceWatcher sender, object args) {
            await coreDispatcher.RunAsync(CoreDispatcherPriority.High, UpdateDevices);
        }

        private async void DeviceWatcher_Updated(DeviceWatcher sender, DeviceInformationUpdate args) {
            await coreDispatcher.RunAsync(CoreDispatcherPriority.High, UpdateDevices);
        }

        private async void UpdateDevices() {
            // Get a list of all MIDI devices
            DeviceInformationCollection = await DeviceInformation.FindAllAsync(midiDeviceSelectorString);

            deviceListControl?.Items?.Clear();

            if (!DeviceInformationCollection.Any()) {
                deviceListControl?.Items?.Add("No MIDI devices found!");
            }

            foreach (var deviceInformation in DeviceInformationCollection) {
                deviceListControl?.Items?.Add(deviceInformation.Name);
            }
        }
    }


}
