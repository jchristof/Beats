
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.Media.Audio;
using Windows.Storage;
using Beats.Annotations;

namespace Beats.ViewModels {
    public class AudioFileInputNodeViewModel : INotifyPropertyChanged, IDisposable {

        public AudioFileInputNode AudioFileInputNode { get; private set; }

        public async Task LoadAudio(AudioGraph audioGraph, AudioDeviceOutputNode outputNode, StorageFolder audioFolder, string filename) {
            StorageFile file = await audioFolder.GetFileAsync(filename);

            CreateAudioFileInputNodeResult fileInputResult = await audioGraph.CreateFileInputNodeAsync(file);
            if (AudioFileNodeCreationStatus.Success != fileInputResult.Status)
                return;

            AudioFileInputNode = fileInputResult.FileInputNode;
            AudioFileInputNode.AddOutgoingConnection(outputNode);
            AudioFileInputNode.Stop();

            AudioFileInputNode.Reset();

            Duration = AudioFileInputNode.Duration;
        }

        public void Dispose() {
            AudioFileInputNode.Dispose();
            AudioFileInputNode = null;
        }

        public void Play() {
            if (AudioFileInputNode == null)
                return;

            AudioFileInputNode.Reset();
            //AudioFileInputNode.StartTime = StartTime;
            //AudioFileInputNode.EndTime = EndTime;
            AudioFileInputNode.Start();
        }

        private TimeSpan startTime;

        public TimeSpan StartTime {
            get => startTime;
            set {
                if (startTime.Equals(value))
                    return;
                startTime = value;
                RaisePropertyChanged();
            }
        }

        private TimeSpan endTime;

        public TimeSpan EndTime {
            get => endTime;
            set {
                if (startTime.Equals(value))
                    return;

                startTime = value;
                RaisePropertyChanged();
            }
        }

        private double volume = 1.0;
        public double Volume {
            get => volume;

            set {
                if (volume.Equals(value))
                    return;

                volume = value;

                if (AudioFileInputNode == null)
                    return;

                AudioFileInputNode.OutgoingGain = value / 100.0;
            }
        }

        private TimeSpan duration;

        public TimeSpan Duration {
            get => duration;
            set {
                if (duration.Equals(value))
                    return;

                duration = value;
                RaisePropertyChanged();
            }
        }

        private double playbackSpeed;

        public double PlaybackSpeed {
            get => playbackSpeed;
            set {
                if (playbackSpeed.Equals(value))
                    return;
                playbackSpeed = value;
                RaisePropertyChanged();
                if (AudioFileInputNode == null)
                    return;

                AudioFileInputNode.PlaybackSpeedFactor = value / 100.0;

            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
