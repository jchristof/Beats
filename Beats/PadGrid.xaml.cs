
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Beats {
    public sealed partial class PadGrid {
        public PadGrid() {
            InitializeComponent();
            IsHitTestVisible = false;
        }

        public void InitGridPad(AudioSystem audioSystem) {
            if (audioSystem == null)
                return;

            this.audioSystem = audioSystem;
            IsHitTestVisible = true;
        }

        private AudioSystem audioSystem;

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e) {
            int ordinal = int.Parse((sender as FrameworkElement).Tag as string);
            audioSystem.Play((BeatType)ordinal);
        }

        private void Grid_KeyDown(object sender, KeyRoutedEventArgs e) {

            switch (e.Key) {
                case VirtualKey.Q: audioSystem.Play((BeatType)0); break;
                case VirtualKey.A: audioSystem.Play((BeatType)1); break;
                case VirtualKey.W: audioSystem.Play((BeatType)2); break;
                case VirtualKey.S: audioSystem.Play((BeatType)3); break;
            }
        }

        private void FrameworkElement_OnLoaded(object sender, RoutedEventArgs e) {
            var actualWidth = (sender as Grid).ActualWidth;
            var actualHeight = (sender as Grid).ActualHeight;

            if (actualWidth < actualHeight)
                Height = ActualWidth;
            else
                Width = ActualHeight;

        }

        private void FrameworkElement_OnSizeChanged(object sender, SizeChangedEventArgs e) {
            var actualWidth = (sender as Grid).ActualWidth;
            var actualHeight = (sender as Grid).ActualHeight;

            if (actualWidth < actualHeight)
                Height = ActualWidth;
            else
                Width = ActualHeight;
        }

    }
}
