
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Beats.ViewModels;

namespace Beats.AudioControls {
    public sealed partial class AudioNodePropertyModifierControl {
        public AudioNodePropertyModifierControl() {
            InitializeComponent();
        }

        private AudioFileInputNodeViewModel ViewModel => DataContext as AudioFileInputNodeViewModel;

        private void AudioNodePropertyModifierControl_OnRightTapped(object sender, RightTappedRoutedEventArgs e) {
            MenuFlyout myFlyout = new MenuFlyout();
            MenuFlyoutItem firstItem = new MenuFlyoutItem { Text = "Reset"};

            firstItem.Click += ResetAudio_Click;
            myFlyout?.Items?.Add(firstItem);

            myFlyout.ShowAt(sender as UIElement, e.GetPosition(sender as UIElement));
        }

        private void ResetAudio_Click(object sender, RoutedEventArgs e) {
            ViewModel?.Reset();
        }

        public event EventHandler<object> PickNewAudioFile = delegate { };

        private void PickNewAudioFileDialog(object sender, RoutedEventArgs e) {
            PickNewAudioFile(ViewModel, e);
        }
    }
}
