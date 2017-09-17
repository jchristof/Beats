using System;
using Windows.Devices.Enumeration;

namespace Beats.Events {
    public class OutputDeviceChangedEvent : EventArgs {

        public DeviceInformation OutputDevice { get; set; }

    }
}
