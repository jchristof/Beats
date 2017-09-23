
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Beats.ViewModels;

namespace Beats.AudioControls {
    public sealed partial class PadGrid {
        public PadGrid() {
            InitializeComponent();
            IsHitTestVisible = false;
        }

        public async Task InitGridPad(AudioSystem audioSystem) {
            if (audioSystem == null)
                return;

            this.audioSystem = audioSystem;
            await LoadDefaultAudio();
            IsHitTestVisible = true;
        }

        public async Task LoadDefaultAudio() {
            StorageFolder audioFolder =
                await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync(@"Assets\audio");

            AudioFileInputNodeViewModel fileNode = new AudioFileInputNodeViewModel();
            await fileNode.LoadAudio(audioSystem.AudioGraph, audioSystem.DeviceOutput, audioFolder, "snare194.wav");

            BeatMap.Add(0, fileNode);

            fileNode = new AudioFileInputNodeViewModel();
            await fileNode.LoadAudio(audioSystem.AudioGraph, audioSystem.DeviceOutput, audioFolder, "KICK 39.WAV");

            BeatMap.Add(1, fileNode);

            fileNode = new AudioFileInputNodeViewModel();
            await fileNode.LoadAudio(audioSystem.AudioGraph, audioSystem.DeviceOutput, audioFolder, "crash-hi.wav");

            BeatMap.Add(2, fileNode);

            fileNode = new AudioFileInputNodeViewModel();
            await fileNode.LoadAudio(audioSystem.AudioGraph, audioSystem.DeviceOutput, audioFolder, "Closed Hat 3.wav");

            BeatMap.Add(3, fileNode);
        }

        private readonly Dictionary<int, AudioFileInputNodeViewModel> BeatMap = new Dictionary<int, AudioFileInputNodeViewModel>();

        private AudioSystem audioSystem;

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e) {
            int ordinal = int.Parse((sender as FrameworkElement).Tag as string);
            Play(ordinal);
        }

        public void Play(int padNumber) {
            BeatMap[padNumber].Play();
        }

        private void Grid_KeyDown(object sender, KeyRoutedEventArgs e) {

            switch (e.Key) {
                case VirtualKey.Q: BeatMap[0].Play(); ; break;
                case VirtualKey.A: BeatMap[1].Play(); ; break;
                case VirtualKey.W: BeatMap[2].Play(); ; break;
                case VirtualKey.S: BeatMap[3].Play(); ; break;
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

        private void Button_RightTapped(object sender, RightTappedRoutedEventArgs e) {
            MenuFlyout myFlyout = new MenuFlyout();
            MenuFlyoutItem firstItem = new MenuFlyoutItem { Text = "Settings" };
            firstItem.Tag = (sender as Button).Tag;
            firstItem.Click += FirstItem_Click;
            myFlyout.Items.Add(firstItem);

            //the code can show the flyout in your mouse click 
            myFlyout.ShowAt(sender as UIElement, e.GetPosition(sender as UIElement));
        }

        private void FirstItem_Click(object sender, RoutedEventArgs e) {

        }
    }
}
