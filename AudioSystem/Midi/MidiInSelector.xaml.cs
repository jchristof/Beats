using System;
using System.Linq;
using Windows.Devices.Enumeration;
using Windows.Devices.Midi;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace AudioSystem.Midi {
    public sealed partial class MidiInSelector : UserControl {
        public MidiInSelector() {
            InitializeComponent();
        }

        private MidiDeviceWatcher inputDeviceWatcher;
        private MidiInPort midiInPort;

        public event EventHandler<DeviceInformation> DeviceSelectedEvent = delegate { };

        private void MidiSelector_OnLoaded(object sender, RoutedEventArgs e) {

            inputDeviceWatcher =
                new MidiDeviceWatcher(MidiInPort.GetDeviceSelector(), midiInPortListBox, Dispatcher);

            inputDeviceWatcher.StartWatcher();
        }

        
        private async void midiInPortListBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            var deviceInformationCollection = inputDeviceWatcher.DeviceInformationCollection;

            if (!deviceInformationCollection.Any())
                return;

            DeviceInformation devInfo = deviceInformationCollection?[midiInPortListBox.SelectedIndex];

            if (devInfo == null) {
                return;
            }

            DeviceSelectedEvent.Invoke(this, devInfo);

//            midiInPort = await MidiInPort.FromIdAsync(devInfo.Id);
//
//            if (midiInPort == null) {
//                System.Diagnostics.Debug.WriteLine("Unable to create MidiInPort from input device");
//                return;
//            }
//            midiInPort.MessageReceived += MidiInPort_MessageReceived;
        }
    }
}
