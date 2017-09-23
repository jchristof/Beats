
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

        private async void PickAudioFile(object sender, RoutedEventArgs e) {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".wav");
            picker.FileTypeFilter.Add(".mp3");

            Windows.Storage.StorageFile file = await picker.PickSingleFileAsync();
            if (file == null)
                return;

            //ViewModel.LoadAudioFile()
        }
    }
}
