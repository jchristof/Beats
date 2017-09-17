
using System;
using Windows.Devices.Enumeration;
using Windows.UI.Xaml.Controls;
using Beats.Events;
using Beats.ViewModels;

namespace Beats.ApplicationControls {
    public sealed partial class OutputSelectionControl {
        public OutputSelectionControl() {
            InitializeComponent();
            Loaded += async (sender, args) => {
                var viewModel = new OutputSelectionViewModel();
                DataContext = viewModel;
                await viewModel.InitializeAsync();
            };
        }

        public event EventHandler<OutputDeviceChangedEvent> OutputDeviceSelectionChanged = delegate { };

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e) {
            OutputDeviceSelectionChanged(sender, new OutputDeviceChangedEvent { OutputDevice = e.AddedItems[0] as DeviceInformation });
        }

    }
}
