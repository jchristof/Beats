

using System.Collections.Generic;
using System.Runtime.InteropServices;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media;
using Windows.Media.Effects;
using Windows.Media.MediaProperties;

namespace CustomEffects {
    [ComImport]
    [Guid("5B0D3235-4DBA-4D44-865E-8F1D0E4FD04D")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    unsafe interface IMemoryBufferByteAccess {
        void GetBuffer(out byte* buffer, out uint capacity);
    }

    public sealed class TranceGateEffect : IBasicAudioEffect {

        private AudioEncodingProperties currentEncodingProperties;

        //support 44.1kHz and 48kHz mono and stereo float
        public IReadOnlyList<AudioEncodingProperties> SupportedEncodingProperties => new List<AudioEncodingProperties> {
            CreateFloatProps(44100,1),
            CreateFloatProps(48000,1),
            CreateFloatProps(44100,2),
            CreateFloatProps(48000,2),
        };

        public bool UseInputFrameForOutput => false;
        private float[] Pattern => (float[])propertySet["Pattern"];
        private float Tempo => (float)propertySet["Tempo"];

        private int patternStepsPerBeat = 8;
        private int samplesperslice;
        private int currentActiveSlice;

        public TranceGateEffect() {}

        private AudioEncodingProperties CreateFloatProps(uint sampleRate, uint channels) {
            var encodingProps = AudioEncodingProperties.CreatePcm(sampleRate, channels, 32);
            encodingProps.Subtype = MediaEncodingSubtypes.Float;
            return encodingProps;
        }
        private int currentActiveSampleIndex;
        private IPropertySet propertySet;

        public void SetProperties(IPropertySet configuration) {
            propertySet = configuration;
        }

        public void SetEncodingProperties(AudioEncodingProperties encodingProperties) {
            currentEncodingProperties = encodingProperties;
            var sampleRate = currentEncodingProperties.SampleRate;
            var channels = (int)currentEncodingProperties.ChannelCount;
            var samplesperbeat = channels * (int)(sampleRate / (Tempo / 60));
            samplesperslice = samplesperbeat / patternStepsPerBeat;
            currentActiveSampleIndex = 0;
            currentActiveSlice = 0;
        }

        public unsafe void ProcessFrame(ProcessAudioFrameContext context) {
            var inputFrame = context.InputFrame;
            var outputFrame = context.OutputFrame;

            using (AudioBuffer inputBuffer = inputFrame.LockBuffer(AudioBufferAccessMode.Read), outputBuffer = outputFrame.LockBuffer(AudioBufferAccessMode.Write))
            using (IMemoryBufferReference inputReference = inputBuffer.CreateReference(), outputReference = outputBuffer.CreateReference()) {
                byte* inputData, outputData;
                uint inCapacity, outCapacity;

                ((IMemoryBufferByteAccess)inputReference).GetBuffer(out inputData, out inCapacity);
                ((IMemoryBufferByteAccess)outputReference).GetBuffer(out outputData, out outCapacity);

                float* inputDataInFloat = (float*)inputData;
                float* outputDataInFloat = (float*)outputData;

                int dataInFloatLength = (int)inputBuffer.Length / sizeof(float);
                var pattern = Pattern;

                for (int i = 0; i < dataInFloatLength; i++) {
                    outputDataInFloat[i] = inputDataInFloat[i] * pattern[currentActiveSlice];
                    currentActiveSampleIndex++;

                    if (currentActiveSampleIndex == samplesperslice) {

                        //wrap around after one second of samples
                        currentActiveSampleIndex = 0;
                        currentActiveSlice++;
                        if (currentActiveSlice == pattern.Length)
                            currentActiveSlice = 0;
                    }
                }
            }

        }

        public void Close(MediaEffectClosedReason reason) {
            
        }

        public void DiscardQueuedFrames() {
            currentActiveSampleIndex = 0;
        }
    }
}
