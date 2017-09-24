
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using Windows.Media.Audio;
using Beats.Annotations;

namespace Beats.ViewModels {
    public class AudioFileInputNodeViewModel : INotifyPropertyChanged, IDisposable {

        public AudioFileInputNodeViewModel(AudioFileInputNode audioFileInputNode, object id) {
            this.audioFileInputNode = audioFileInputNode;

            Duration = audioFileInputNode.Duration;
            StartPosition = TimeSpan.Zero;
            EndPosition = audioFileInputNode.Duration;
            PlaybackSpeed = audioFileInputNode.PlaybackSpeedFactor * 100;
            Volume = audioFileInputNode.OutgoingGain * 100;
            
            Filename = Path.GetFileName(audioFileInputNode.SourceFile.DisplayName);
            Id = id;
        }

        public object Id { get; private set; }

        private AudioFileInputNode audioFileInputNode;

        public void Reset() {
            StartPosition = TimeSpan.Zero;
            EndPosition = audioFileInputNode.Duration;
            PlaybackSpeed = 100;
            Volume = 100;
        }

        public void Dispose() {
            audioFileInputNode?.Stop();
            audioFileInputNode?.Dispose();
            audioFileInputNode = null;
        }

        public void Play() {
            if (audioFileInputNode == null)
                return;

            audioFileInputNode.Reset();
            audioFileInputNode.StartTime = startPosition;
            audioFileInputNode.EndTime = endPosition;
            audioFileInputNode.Start();
        }


        private string filename;

        public string Filename {
            get => filename;
            set {
                if (value == filename)
                    return;
                filename = value;
                RaisePropertyChanged();
            }
        }

        private TimeSpan startPosition;

        public TimeSpan StartPosition {
            get => startPosition;
            set {
                if (startPosition.Equals(value))
                    return;
                startPosition = value;
                RaisePropertyChanged();
            }
        }

        private TimeSpan endPosition;

        public TimeSpan EndPosition {
            get => endPosition;
            set {
                if (endPosition.Equals(value))
                    return;

                endPosition = value;
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

                if (audioFileInputNode == null)
                    return;

                audioFileInputNode.OutgoingGain = value / 100.0;
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
                if (audioFileInputNode == null)
                    return;

                audioFileInputNode.PlaybackSpeedFactor = value / 100.0;

            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
