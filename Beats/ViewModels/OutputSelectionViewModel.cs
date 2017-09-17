
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Beats.Annotations;

namespace Beats.ViewModels {
    public class OutputSelectionViewModel : INotifyPropertyChanged {

        public OutputSelectionViewModel() {
            Devices = new ObservableCollection<DeviceInformation>();
        }

        public async Task InitializeAsync() {
            var outputDevices = await DeviceInformation.FindAllAsync(DeviceClass.AudioRender);
            foreach (var device in outputDevices) {
                Devices.Add(device);   
            }

            SelectedDevice = Devices.FirstOrDefault(d => d.IsDefault);
        }
        private DeviceInformation seletedDevice;

        public DeviceInformation SelectedDevice {
            get => seletedDevice;
            set {
                if (seletedDevice == value)
                    return;

                seletedDevice = value;
                RaisePropertyChanged();
            }
        }
        public ObservableCollection<DeviceInformation> Devices { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
