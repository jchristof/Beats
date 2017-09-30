using System;
using Windows.Devices.Enumeration;
using Windows.UI.Xaml.Controls;

namespace MidiInOut
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
           InitializeComponent();
        }

        private void MidiInSelector_OnDeviceSelectedEvent(object sender, DeviceInformation e) {
           
        }

    }
}
