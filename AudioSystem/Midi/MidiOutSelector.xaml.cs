
using System;
using System.Linq;
using Windows.Devices.Enumeration;
using Windows.Devices.Midi;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace AudioSystem.Midi {
    public sealed partial class MidiOutSelector {
        public MidiOutSelector() {
            InitializeComponent();
        }

        private MidiDeviceWatcher outputDeviceWatcher;

        public event EventHandler<DeviceInformation> DeviceSelectedEvent = delegate { };

        private void MidiSelector_OnLoaded(object sender, RoutedEventArgs e) {
            outputDeviceWatcher = new MidiDeviceWatcher(MidiOutPort.GetDeviceSelector(), MidiOutPortSelector, Dispatcher);
            outputDeviceWatcher.StartWatcher();
        }

        private void SelectionChanged(object sender, SelectionChangedEventArgs e) {
            var deviceInformationCollection = outputDeviceWatcher.DeviceInformationCollection;

            if (!deviceInformationCollection.Any())
                return;

            DeviceInformation devInfo = deviceInformationCollection?[MidiOutPortSelector.SelectedIndex];

            if (devInfo == null) {
                return;
            }

            DeviceSelectedEvent.Invoke(this, devInfo);
        }
    }
}
