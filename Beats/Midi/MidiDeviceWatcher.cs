﻿
using System;
using System.Linq;
using Windows.Devices.Enumeration;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;

namespace Beats.Midi
{
    public class MidiDeviceWatcher
    {
        public MidiDeviceWatcher(string midiDeviceSelectorString, ComboBox midiDeviceListBox, CoreDispatcher dispatcher) {
            deviceListBox = midiDeviceListBox;
            coreDispatcher = dispatcher;

            deviceSelectorString = midiDeviceSelectorString;

            deviceWatcher = DeviceInformation.CreateWatcher(deviceSelectorString);
            deviceWatcher.Added += DeviceWatcher_Added;
            deviceWatcher.Removed += DeviceWatcher_Removed;
            deviceWatcher.Updated += DeviceWatcher_Updated;
            deviceWatcher.EnumerationCompleted += DeviceWatcher_EnumerationCompleted;
        }

        DeviceWatcher deviceWatcher;
        string deviceSelectorString;
        ComboBox deviceListBox;
        CoreDispatcher coreDispatcher;

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
            await coreDispatcher.RunAsync(CoreDispatcherPriority.High, () =>
            {
                // Update the device list
                UpdateDevices();
            });
        }

        private async void DeviceWatcher_Added(DeviceWatcher sender, DeviceInformation args) {
            await coreDispatcher.RunAsync(CoreDispatcherPriority.High, () =>
            {
                // Update the device list
                UpdateDevices();
            });
        }

        private async void DeviceWatcher_EnumerationCompleted(DeviceWatcher sender, object args) {
            await coreDispatcher.RunAsync(CoreDispatcherPriority.High, () =>
            {
                // Update the device list
                UpdateDevices();
            });
        }

        private async void DeviceWatcher_Updated(DeviceWatcher sender, DeviceInformationUpdate args) {
            await coreDispatcher.RunAsync(CoreDispatcherPriority.High, () =>
            {
                // Update the device list
                UpdateDevices();
            });
        }

        private async void UpdateDevices() {
            // Get a list of all MIDI devices
            this.DeviceInformationCollection = await DeviceInformation.FindAllAsync(deviceSelectorString);

            deviceListBox.Items.Clear();

            if (!this.DeviceInformationCollection.Any()) {
                deviceListBox.Items.Add("No MIDI devices found!");
            }

            foreach (var deviceInformation in this.DeviceInformationCollection) {
                deviceListBox.Items.Add(deviceInformation.Name);
            }
        }
    }


}
