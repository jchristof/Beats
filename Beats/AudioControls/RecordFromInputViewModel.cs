
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.Devices.Enumeration;
using Beats.Annotations;

namespace Beats.AudioControls {
    public class RecordFromInputViewModel : INotifyPropertyChanged {

        public RecordFromInputViewModel() {
            TransportAction = "Record";
        }

        public ObservableCollection<DeviceInformation> RecordingDevices { get; } = new ObservableCollection<DeviceInformation>();

        private DeviceInformation selectedInputDevice;

        public DeviceInformation SelectedInputDevice {
            get => selectedInputDevice;
            set {
                if (selectedInputDevice == value)
                    return;

                selectedInputDevice = value;
                RaisePropertyChanged();
            }
        }

        private string transportAction;
        public string TransportAction {
            get => transportAction;
            set {
                if (transportAction == value)
                    return;
                transportAction = value;
                RaisePropertyChanged();
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
