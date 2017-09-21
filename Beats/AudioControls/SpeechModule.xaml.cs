using Windows.UI.Xaml;
using Beats.Extensions;

namespace Beats.AudioControls {
    public sealed partial class SpeechModule {
        public SpeechModule() {
            InitializeComponent();
            speechNode = new SpeechNode();
        }

        private readonly SpeechNode speechNode;

        async void Button_Click(object sender, RoutedEventArgs e) {
            await MediaElement.PlayStreamAsync(await speechNode.TextToStream(Text.Text), true);
        }
    }
}
