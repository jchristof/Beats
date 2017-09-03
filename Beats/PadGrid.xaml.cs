
using System;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;

namespace Beats {
    public sealed partial class PadGrid {
        public PadGrid() {
            InitializeComponent();
            InitAudioGraph();
        }

        private AudioSystem audioSystem = new AudioSystem();

        //audioGraph.Dispose();

        private async void InitAudioGraph() {
            await audioSystem.Create();

            StorageFolder audioFolder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync(@"Assets\audio");
            await audioSystem.LoadAudio(audioFolder);
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e) {
            int ordinal = int.Parse((sender as ToggleButton).Tag as string);
            audioSystem.Play((BeatType)ordinal);
        }

    }
}
