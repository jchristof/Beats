
using System;
using Windows.Devices.Enumeration;
using Windows.Devices.Midi;
using Windows.Media;
using Windows.Media.Audio;
using Windows.Media.Render;
using Windows.UI.Xaml;

namespace Wavetable_Synth {

    public sealed partial class MainPage {
        public MainPage() {
            InitializeComponent();
        }

        private AudioSystem.AudioSystem audioSystem;
        private AudioFrameInputNode frameInputNode;
        private MidiInPort midiInPort;

        private async void MainPage_OnLoaded(object sender, RoutedEventArgs e) {
            audioSystem = new AudioSystem.AudioSystem();

            await audioSystem.Create(
                new AudioGraphSettings(AudioRenderCategory.Media) {
                    DesiredRenderDeviceAudioProcessing = AudioProcessing.Raw,
                    QuantumSizeSelectionMode = QuantumSizeSelectionMode.LowestLatency
                });

            var nodeEncodingProperties = audioSystem.AudioGraph.EncodingProperties;
            nodeEncodingProperties.ChannelCount = 1;

            frameInputNode = audioSystem.AudioGraph.CreateFrameInputNode(nodeEncodingProperties);
            frameInputNode.AddOutgoingConnection(audioSystem.DeviceOutput);
            frameInputNode.Stop();
            frameInputNode.QuantumStarted += FrameInputNode_QuantumStarted;
        }

        private void FrameInputNode_QuantumStarted(AudioFrameInputNode sender, FrameInputNodeQuantumStartedEventArgs args) {
            var numSamplesNeeded = (uint)args.RequiredSamples;
            if (numSamplesNeeded == 0)
                return;

            var audioData = GenerateAudioData(numSamplesNeeded);
            frameInputNode.AddFrame(audioData);
        }

        private AudioFrame GenerateAudioData(uint numSamplesNeeded) {
            throw new NotImplementedException();
        }

        private async void MidiInSelector_OnDeviceSelectedEvent(object sender, DeviceInformation e) {
            midiInPort = await MidiInPort.FromIdAsync(e.Id);

            if (midiInPort == null) {
                System.Diagnostics.Debug.WriteLine("Unable to create MidiInPort from input device");
                return;
            }
            midiInPort.MessageReceived += MidiInPortOnMessageReceived;
        }

        private double desiredFrequency;
        private int activeNotes;

        private void MidiInPortOnMessageReceived(MidiInPort sender, MidiMessageReceivedEventArgs args) {
            IMidiMessage receivedMidiMessage = args.Message;

            if (receivedMidiMessage.Type == MidiMessageType.NoteOn) {
                MidiNoteOnMessage noteOnMessage = (MidiNoteOnMessage)receivedMidiMessage;
                desiredFrequency = 440.0 * Math.Pow(2.0, (noteOnMessage.Note - 69.0) / 12);
                activeNotes++;
                if (activeNotes == 1)
                    frameInputNode.Start();

            }
            else if (receivedMidiMessage.Type == MidiMessageType.NoteOff) {
                MidiNoteOffMessage noteOffMessage = (MidiNoteOffMessage)receivedMidiMessage;
                activeNotes--;
                if(activeNotes == 0)
                    frameInputNode.Stop();
            }
        }

    }
}
