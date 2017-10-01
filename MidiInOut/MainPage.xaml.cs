
using System;
using Windows.ApplicationModel.Core;
using Windows.Devices.Enumeration;
using Windows.Devices.Midi;
using Windows.UI.Core;

namespace MidiInOut
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage 
    {
        public MainPage()
        {
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

        private async void MidiInPort_MessageReceived(MidiInPort sender, MidiMessageReceivedEventArgs args) {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal,
                    () => {
                        IMidiMessage receivedMidiMessage = args.Message;

                        System.Diagnostics.Debug.WriteLine(receivedMidiMessage.Timestamp.ToString());

                        if (receivedMidiMessage.Type == MidiMessageType.NoteOn) {
                            MidiNoteOnMessage noteOnMessage = (MidiNoteOnMessage)receivedMidiMessage;
                            midiOutPort?.SendMessage(noteOnMessage);
                            TextBox.Text = $"Note on {noteOnMessage.Channel} {noteOnMessage.Note} {noteOnMessage.Velocity}" + Environment.NewLine + TextBox.Text;
                        }
                        else if (receivedMidiMessage.Type == MidiMessageType.NoteOff) {
                            MidiNoteOffMessage noteOffMessage = (MidiNoteOffMessage)receivedMidiMessage;
                            midiOutPort?.SendMessage(noteOffMessage);
                            TextBox.Text = $"Note off {noteOffMessage.Channel} {noteOffMessage.Note} " + Environment.NewLine + TextBox.Text;
                        }
                    });
        }

        private async void MidiOutSelector_OnDeviceSelectedEvent(object sender, DeviceInformation e) {

            midiOutPort = await MidiOutPort.FromIdAsync(e.Id);

            if (midiOutPort == null) {
                System.Diagnostics.Debug.WriteLine("Unable to create MidiOutPort from output device");
            }
        }

    }
}
