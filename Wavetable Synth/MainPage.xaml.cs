
using System;
using System.Runtime.InteropServices;
using Windows.Devices.Enumeration;
using Windows.Devices.Midi;
using Windows.Media;
using Windows.Media.Audio;
using Windows.Media.Render;
using Windows.UI.Xaml;

namespace Wavetable_Synth {

    [ComImport]
    [Guid("5B0D3235-4DBA-4D44-865E-8F1D0E4FD04D")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]

    unsafe interface IMemoryBufferByteAccess {

        void GetBuffer(out byte* buffer, out uint capacity);

    }

    public sealed partial class MainPage {
        public MainPage() {
            InitializeComponent();          
        }

        private AudioSystem.AudioSystem audioSystem;
        private AudioFrameInputNode frameInputNode;
        private MidiInPort midiInPort;
        private float volume = .5f;

        private float[] sinTable;
        private float[] sawTable;
        private float[] selectedWaveTable;
        private double theta = 0;

        private void CreateWaveTables() {
            var samepleRate = (int)audioSystem.AudioGraph.EncodingProperties.SampleRate;
            sinTable = new float[samepleRate];
            sawTable = new float[samepleRate];

            for (int n = 0; n < sinTable.Length; n++) {
                sinTable[n] = (float)Math.Sin(2 * Math.PI * n / samepleRate);
                sawTable[n] = 2f * ((float)n / samepleRate) - 1.0f;
            }

            selectedWaveTable = sinTable;
        }
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

            CreateWaveTables();
        }

        private void FrameInputNode_QuantumStarted(AudioFrameInputNode sender, FrameInputNodeQuantumStartedEventArgs args) {
            var numSamplesNeeded = (uint)args.RequiredSamples;
            if (numSamplesNeeded == 0)
                return;

            var audioData = GenerateAudioData(numSamplesNeeded);
            frameInputNode.AddFrame(audioData);
        }

        unsafe private AudioFrame GenerateAudioData(uint samples) {
            var bufferSize = samples * sizeof(float);
            var frame = new AudioFrame(bufferSize);

            using (var buffer = frame.LockBuffer(AudioBufferAccessMode.Write))
            using (var reference = buffer.CreateReference()) {

                ((IMemoryBufferByteAccess)reference).GetBuffer(out byte* bufferBytePointer, out uint capacityInBytes);

                var bufferFloatPointer = (float*)bufferBytePointer;
                var amplitude = volume;

                for (int i = 0; i < samples; i++) {
                    var index = (int)theta % selectedWaveTable.Length;
                    bufferFloatPointer[i] = amplitude * selectedWaveTable[index];
                    theta += desiredFrequency;
                }

                return frame;
            }
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
