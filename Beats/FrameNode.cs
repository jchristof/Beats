using System;
using System.Runtime.InteropServices;
using Windows.Foundation;
using Windows.Media;
using Windows.Media.Audio;
using Windows.Media.MediaProperties;

namespace Beats {
    public class FrameNode {
        [ComImport]
        [Guid("5B0D3235-4DBA-4D44-865E-8F1D0E4FD04D")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]

        unsafe interface IMemoryBufferByteAccess {
            void GetBuffer(out byte* buffer, out uint capacity);
        }

        public double theta = 0;
        private AudioFrameInputNode frameInputNode;
        private uint sampleRate;

        public void Create(AudioGraph audioGraph, AudioDeviceOutputNode deviceOutputNode) {
            sampleRate = audioGraph.EncodingProperties.SampleRate;

            AudioEncodingProperties nodeEncodingProperties = audioGraph.EncodingProperties;
            nodeEncodingProperties.ChannelCount = 1;
            frameInputNode = audioGraph.CreateFrameInputNode(nodeEncodingProperties);
            frameInputNode.AddOutgoingConnection(deviceOutputNode);
            frameInputNode.Stop();
            frameInputNode.QuantumStarted += node_QuantumStarted;
        }

        public void Start() {
            frameInputNode.Start();
        }

        private void node_QuantumStarted(AudioFrameInputNode sender, FrameInputNodeQuantumStartedEventArgs args) {
            // GenerateAudioData can provide PCM audio data by directly synthesizing it or reading from a file.
            // Need to know how many samples are required. In this case, the node is running at the same rate as the rest of the graph
            // For minimum latency, only provide the required amount of samples. Extra samples will introduce additional latency.
            uint numSamplesNeeded = (uint)args.RequiredSamples;

            if (numSamplesNeeded != 0) {
                AudioFrame audioData = GenerateAudioData(numSamplesNeeded);
                frameInputNode.AddFrame(audioData);
            }
        }

        private unsafe AudioFrame GenerateAudioData(uint samples) {
            // Buffer size is (number of samples) * (size of each sample)
            // We choose to generate single channel (mono) audio. For multi-channel, multiply by number of channels
            uint bufferSize = samples * sizeof(float);
            AudioFrame frame = new AudioFrame(bufferSize);

            using (AudioBuffer buffer = frame.LockBuffer(AudioBufferAccessMode.Write))
            using (IMemoryBufferReference reference = buffer.CreateReference()) {
                byte* dataInBytes;
                uint capacityInBytes;
                float* dataInFloat;

                // Get the buffer from the AudioFrame
                ((IMemoryBufferByteAccess)reference).GetBuffer(out dataInBytes, out capacityInBytes);

                // Cast to float since the data we are generating is float
                dataInFloat = (float*)dataInBytes;

                float freq = 880; // choosing to generate frequency of 1kHz
                float amplitude = 0.3f;

                double sampleIncrement = freq * Math.PI / sampleRate;

                // Generate a 1kHz sine wave and populate the values in the memory buffer
                for (int i = 0; i < samples; i++) {
                    double sinValue = amplitude * Math.Sin(theta);
                    dataInFloat[i] = (float)sinValue;
                    theta += sampleIncrement;
                }
            }

            return frame;
        }
    }
}
