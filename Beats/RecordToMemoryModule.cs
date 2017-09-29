

using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Media;
using Windows.Media.Audio;
using Windows.Media.Capture;

namespace Beats
{
    public class RecordToMemoryModule {

        public RecordToMemoryModule(AudioSystem audioSystem) {
            this.audioSystem = audioSystem;
        }

        private AudioSystem audioSystem;
        AudioFrameOutputNode frameOutputNode;
        AudioFrameInputNode frameInputNode;
        private AudioDeviceInputNode deviceInputNode;

        public async Task CreateAsync(DeviceInformation deviceInformation) {
            CreateAudioDeviceInputNodeResult deviceInputNodeResult = 
                await audioSystem.AudioGraph.CreateDeviceInputNodeAsync(MediaCategory.Other, audioSystem.AudioGraph.EncodingProperties, deviceInformation);

            if (deviceInputNodeResult.Status != AudioDeviceNodeCreationStatus.Success) {
                Debug.WriteLine("Unable to create input device");
                return;
            }

            deviceInputNode = deviceInputNodeResult.DeviceInputNode;

            frameOutputNode = audioSystem.AudioGraph.CreateFrameOutputNode();
            deviceInputNode.AddOutgoingConnection(frameOutputNode);
            deviceInputNode.Stop();
            frameOutputNode.Stop();

            audioSystem.AudioGraph.QuantumStarted += AudioGraph_QuantumStarted;
            audioSystem.AudioGraph.UnrecoverableErrorOccurred += AudioGraph_UnrecoverableErrorOccurred;

            //memory to audio out
            frameInputNode = audioSystem.AudioGraph.CreateFrameInputNode();
            frameInputNode.AddOutgoingConnection(audioSystem.DeviceOutput);
            frameInputNode.QuantumStarted += FrameInputNode_QuantumStarted;
            frameInputNode.Stop();
        }

        public void Start() {
            deviceInputNode.Start();
            frameOutputNode.Start();
        }

        public void Stop() {
            deviceInputNode.Stop();
            frameOutputNode.Stop();
            frameInputNode.Start();
        }

        private async void AudioGraph_UnrecoverableErrorOccurred(AudioGraph sender, AudioGraphUnrecoverableErrorOccurredEventArgs args) {
//            await Windows.UI.Core.CoreDispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () => {
//                
//            }
         }

        uint maxCapacityEncountered;
        uint lastCapacityEncountered;
        uint maxIndexInBytes;
        byte[] byteBuffer = new byte[120 * 48000 * 2 * 4];
        volatile int audioFrameCount;

        private unsafe void AudioGraph_QuantumStarted(AudioGraph graph, object args) {
            AudioFrame frame = frameOutputNode.GetFrame();

            using (AudioBuffer buffer = frame.LockBuffer(AudioBufferAccessMode.Read))
            using (IMemoryBufferReference reference = buffer.CreateReference()) {
                byte* dataInBytes;
                uint capacityInBytes;

                // Get the buffer from the AudioFrame
                ((IMemoryBufferByteAccess)reference).GetBuffer(out dataInBytes, out capacityInBytes);

                if (capacityInBytes == 0) {
                    // we don't count zero-byte frames... and why do they ever happen???
                    return;
                }

                // Must be multiple of 8 (2 channels, 4 bytes/float)
                Debug.Assert((capacityInBytes & 0x7) == 0);

                lastCapacityEncountered = capacityInBytes;
                maxCapacityEncountered = Math.Max(maxCapacityEncountered, capacityInBytes);

                uint bufferStart = 0;
                if (maxIndexInBytes == 0) {
                    // if maxCapacityEncountered is greater than the audio graph buffer size, 
                    // then the audio graph decided to give us a big backload of buffer content
                    // as its first callback.  Not sure why it does this, but we don't want it,
                    // so take only the tail of the buffer.
                    if (maxCapacityEncountered > (graph.LatencyInSamples << 3)) {
                        bufferStart = capacityInBytes - (uint)(graph.LatencyInSamples << 3);
                        capacityInBytes = (uint)(graph.LatencyInSamples << 3);
                    }
                }

                // if we fill up, just spin forever.  don't try to exit recording from audio thread,
                // we don't bother with two-way signaling.
                if (maxIndexInBytes + capacityInBytes > byteBuffer.Length) {
                    Debug.Assert(byteBuffer.Length >= maxIndexInBytes);
                    capacityInBytes = (uint)byteBuffer.Length - maxIndexInBytes;
                }

                fixed (byte* bufferOfBytes = byteBuffer) {
                    long* dest = ((long*)bufferOfBytes) + (maxIndexInBytes / sizeof(long));
                    long* src = (long*)dataInBytes;
                    long* srcEnd = src + (capacityInBytes / sizeof(long));

                    while (src < srcEnd) {
                        *dest++ = *src++;
                    }
                }
                maxIndexInBytes += capacityInBytes;
            }

            audioFrameCount++;
        }

        static AudioFrame s_audioFrame = new Windows.Media.AudioFrame(1024 * 1024);
        //static float[] rawAudio = new float[1024 * 1024 * 20];

        private int s_zeroByteOutgoingFrameCount;
        // Current record/play index into the buffer.
        uint currentIndexInBytes;

        private unsafe void FrameInputNode_QuantumStarted(AudioFrameInputNode sender, FrameInputNodeQuantumStartedEventArgs args) {
            uint requiredSamples = (uint)args.RequiredSamples;

            if (requiredSamples == 0) {
                s_zeroByteOutgoingFrameCount++;
                return;
            }

            using (AudioBuffer buffer = s_audioFrame.LockBuffer(AudioBufferAccessMode.Write))
            using (IMemoryBufferReference reference = buffer.CreateReference()) {
                byte* dataInBytes;
                uint capacityInBytes;

                // Get the buffer from the AudioFrame
                ((IMemoryBufferByteAccess)reference).GetBuffer(out dataInBytes, out capacityInBytes);

                // Return just the required number of samples.
                uint bytesRemaining = requiredSamples << 3;
                // Or, alternatively, comment out the line above and uncomment this one, to return a full buffer's worth of data.
                // This results in far fewer callbacks to the application, which is desirable if there is no need for low-latency
                // varying audio rendering.
                // uint bytesRemaining = capacityInBytes;

                buffer.Length = bytesRemaining;

                uint positionInBytes = 0;

                // As long as there are more bytes remaining in the dataInBytes buffer, copy from
                // our buffer (wrapping back to the start if the end is reached, i.e. looping).
                while (bytesRemaining > 0) {
                    uint bytesInThisIteration = bytesRemaining;
                    if (currentIndexInBytes + bytesRemaining > maxIndexInBytes) {
                        bytesInThisIteration = maxIndexInBytes - currentIndexInBytes;
                        if (bytesInThisIteration == 0) {
                            // wraparound; next iteration will get more bytes
                            currentIndexInBytes = 0;
                            continue;
                        }
                    }

                    // Now copy bytesInThisIteration bytes into the output.
                    fixed (byte* bufOfBytes = byteBuffer) {
                        // CopyMemory(dataInBytes + positionInBytes, bufOfBytes + currentIndexInBytes, bytesInThisIteration);

                        long* dest = ((long*)dataInBytes) + (positionInBytes / sizeof(long));
                        long* src = ((long*)bufOfBytes) + (currentIndexInBytes / sizeof(long));
                        long* srcEnd = ((long*)src) + (bytesInThisIteration / sizeof(long));

                        while (src < srcEnd) {
                            *dest++ = *src++;
                        }
                    }

                    //Buffer.BlockCopy(byteBuffer, 0, rawAudio, 0, byteBuffer.Length);
                    currentIndexInBytes += bytesInThisIteration;
                    positionInBytes += bytesInThisIteration;

                    bytesRemaining -= bytesInThisIteration;
                }
            }

            frameInputNode.AddFrame(s_audioFrame);
        }
    }
}
