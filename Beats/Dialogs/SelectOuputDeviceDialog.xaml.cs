using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.Devices.Enumeration;
using Windows.UI.Xaml.Controls;
using Beats.Annotations;
using Beats.Events;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Beats.Dialogs {
    public sealed partial class SelectOuputDeviceDialog : INotifyPropertyChanged{
        public SelectOuputDeviceDialog() {
            this.InitializeComponent();
            DataContext = this;
        }

        public DeviceInformation Result { get; private set; }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args) {
        }

        private void OutputSelectionControl_OnOutputDeviceSelectionChanged(object sender, OutputDeviceChangedEvent e) {
            Result = e.OutputDevice;
            HasSelection = true;
        }

        private bool hasSelection;
        public bool HasSelection {
            get => hasSelection;

            set {
                if (hasSelection == value)
                    return;

                hasSelection = value;
                RaisePropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void RaisePropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
