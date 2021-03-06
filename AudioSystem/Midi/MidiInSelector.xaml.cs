﻿using System;
using System.Linq;
using Windows.Devices.Enumeration;
using Windows.Devices.Midi;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace AudioSystem.Midi {
    public sealed partial class MidiInSelector {
        public MidiInSelector() {
            InitializeComponent();
        }

        private MidiDeviceWatcher inputDeviceWatcher;

        public event EventHandler<DeviceInformation> DeviceSelectedEvent = delegate { };

        private void MidiSelector_OnLoaded(object sender, RoutedEventArgs e) {
            inputDeviceWatcher = new MidiDeviceWatcher(MidiInPort.GetDeviceSelector(), MidiInPortListBox, Dispatcher);
            inputDeviceWatcher.StartWatcher();
        }
    
        private void SelectionChanged(object sender, SelectionChangedEventArgs e) {
            var deviceInformationCollection = inputDeviceWatcher.DeviceInformationCollection;

            if (!deviceInformationCollection.Any())
                return;

            DeviceInformation devInfo = deviceInformationCollection?[MidiInPortListBox.SelectedIndex];

            if (devInfo == null) {
                return;
            }

            DeviceSelectedEvent.Invoke(this, devInfo);
        }

    }
}
