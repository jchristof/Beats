

using System;
using Windows.Storage;

namespace Beats
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage 
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private AudioSystem audioSystem = new AudioSystem();

        private async void InitAudioGraph()
        {
            await audioSystem.Create();

            StorageFolder audioFolder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync(@"Assets\audio");
            await audioSystem.LoadAudio(audioFolder);
        }
    }
}
