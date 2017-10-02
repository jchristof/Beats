
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using Windows.ApplicationModel.Core;
using Windows.Devices.Enumeration;
using Windows.Devices.Midi;
using Windows.UI.Core;
using MidiInOut.Annotations;

namespace MidiInOut {
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : INotifyPropertyChanged{
        public MainPage() {
            InitializeComponent();
        }

        private MidiInPort midiInPort;
        private IMidiOutPort midiOutPort;

        private async void MidiInSelector_OnDeviceSelectedEvent(object sender, DeviceInformation e) {
            midiInPort = await MidiInPort.FromIdAsync(e.Id);

            if (midiInPort == null) {
                System.Diagnostics.Debug.WriteLine("Unable to create MidiInPort from input device");
                return;
            }
            midiInPort.MessageReceived += MidiInPort_MessageReceived;
        }

        private async void MidiOutSelector_OnDeviceSelectedEvent(object sender, DeviceInformation e) {

            midiOutPort = await MidiOutPort.FromIdAsync(e.Id);
            if (midiOutPort == null) {
                System.Diagnostics.Debug.WriteLine("Unable to create MidiOutPort from output device");
            }
        }

        private async void MidiInPort_MessageReceived(MidiInPort sender, MidiMessageReceivedEventArgs args) {
            midiOutPort?.SendMessage(args.Message);

            if(!logCommands)
                return;
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal,
                    () => {
                        IMidiMessage receivedMidiMessage = args.Message;

                        System.Diagnostics.Debug.WriteLine(receivedMidiMessage.Timestamp.ToString());
                        StringBuilder builder = new StringBuilder();
                        builder.Append(receivedMidiMessage.Type);
                        if (receivedMidiMessage.Type == MidiMessageType.NoteOn) {
                            MidiNoteOnMessage noteOnMessage = (MidiNoteOnMessage)receivedMidiMessage;
                            builder.Append($"Note on {noteOnMessage.Channel} {noteOnMessage.Note} {noteOnMessage.Velocity}");
                            
                        }
                        else if (receivedMidiMessage.Type == MidiMessageType.NoteOff) {
                            MidiNoteOffMessage noteOffMessage = (MidiNoteOffMessage)receivedMidiMessage;
                            builder.Append($"Note off {noteOffMessage.Channel} {noteOffMessage.Note} ");
                        }

                        TextBox.Text = builder + Environment.NewLine + TextBox.Text;
                    });
        }

        private bool logCommands;

        public bool LogCommands {
            get => logCommands;
            set {
                if (logCommands == value)
                    return;

                logCommands = value;
                RaisePropertyChanged();
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void RaisePropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void IntPicker_OnValueChanged(object sender, int e) {
            var m = new MidiProgramChangeMessage(0, (byte)e);
            midiOutPort?.SendMessage(m);
        }

    }
}
