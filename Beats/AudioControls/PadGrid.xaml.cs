
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Media.Audio;
using Windows.Storage;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Beats.ViewModels;

namespace Beats.AudioControls {
    public sealed partial class PadGrid : IDisposable {
        public PadGrid() {
            InitializeComponent();
            IsHitTestVisible = false;
        }

        private readonly Dictionary<int, AudioFileInputNodeViewModel> AudioMap = new Dictionary<int, AudioFileInputNodeViewModel>();

        private AudioSystem audioSystem;

        //Initialize the pad and load default audio
        public async Task InitGridPad(AudioSystem audioSystem) {
            if (audioSystem == null)
                return;

            this.audioSystem = audioSystem;
            await LoadDefaultAudio();
            IsHitTestVisible = true;
        }

        //load default audio from the intalled proejct 
        public async Task LoadDefaultAudio() {
            StorageFolder audioFolder =
                await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync(@"Assets\audio");
           
            StorageFile file = await audioFolder.GetFileAsync("snare194.wav");
            AudioFileInputNode fileNode = await NewAudioFileInputNode(audioSystem.AudioGraph, audioSystem.DeviceOutput, file);

            AudioMap.Add(0, new AudioFileInputNodeViewModel(fileNode, 0));

            file = await audioFolder.GetFileAsync("KICK 39.WAV");
            fileNode = await NewAudioFileInputNode(audioSystem.AudioGraph, audioSystem.DeviceOutput, file);

            AudioMap.Add(1, new AudioFileInputNodeViewModel(fileNode, 1));

            file = await audioFolder.GetFileAsync("crash-hi.wav");
            fileNode = await NewAudioFileInputNode(audioSystem.AudioGraph, audioSystem.DeviceOutput, file);

            AudioMap.Add(2, new AudioFileInputNodeViewModel(fileNode, 2));

            file = await audioFolder.GetFileAsync("Closed Hat 3.wav");
            fileNode = await NewAudioFileInputNode(audioSystem.AudioGraph, audioSystem.DeviceOutput, file);

            AudioMap.Add(3, new AudioFileInputNodeViewModel(fileNode, 3));
        }

        public async Task LoadAudio(object id, StorageFile file) {
            var fileNode = await NewAudioFileInputNode(audioSystem.AudioGraph, audioSystem.DeviceOutput, file);
            int padId = (int)id;

            if(AudioMap.ContainsKey(padId))
                AudioMap.Remove(padId);

            AudioMap.Add(padId, new AudioFileInputNodeViewModel(fileNode, id));
        }

        private static async Task<AudioFileInputNode> NewAudioFileInputNode(AudioGraph audioGraph, AudioDeviceOutputNode outputNode, StorageFile file) {
            
            CreateAudioFileInputNodeResult fileInputResult = await audioGraph.CreateFileInputNodeAsync(file);
            if (AudioFileNodeCreationStatus.Success != fileInputResult.Status)
                return null;

            AudioFileInputNode node = fileInputResult.FileInputNode;
            node.AddOutgoingConnection(outputNode);
            node.Stop();

            node.Reset();

            return node;
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e) {
            int ordinal = int.Parse((sender as FrameworkElement).Tag as string);
            Play(ordinal);
        }

        public void Play(int padNumber) {
            AudioMap[padNumber].Play();
        }

        private void Grid_KeyDown(object sender, KeyRoutedEventArgs e) {

            switch (e.Key) {
                case VirtualKey.Q: AudioMap[0].Play(); ; break;
                case VirtualKey.A: AudioMap[1].Play(); ; break;
                case VirtualKey.W: AudioMap[2].Play(); ; break;
                case VirtualKey.S: AudioMap[3].Play(); ; break;
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

        public event EventHandler<AudioFileInputNodeViewModel> AudioFileSelected = delegate { };
        private void FirstItem_Click(object sender, RoutedEventArgs e) {
            int ordinal = int.Parse((sender as FrameworkElement).Tag as string);
            AudioFileSelected(sender, AudioMap[ordinal]);
        }

        private void ReleaseAudio() {
            if (AudioMap == null)
                return;

            foreach (KeyValuePair<int, AudioFileInputNodeViewModel> audioFileInputNodeViewModel in AudioMap) {
                audioFileInputNodeViewModel.Value.Dispose();
            }
        }

        public void Dispose() {
            ReleaseAudio();
        }

    }
}
