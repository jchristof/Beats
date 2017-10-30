
using Windows.UI.Xaml;

namespace Beats {
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class StartPage {
        public StartPage() {
            InitializeComponent();
        }

        private void GoTo_Beats(object sender, RoutedEventArgs e) {
            App.RootFrame.Navigate(typeof(MainPage));
        }

        private void GoTo_AudioGrid(object sender, RoutedEventArgs e) {
                
        }

        private void GoTo_MidiWaveTable(object sender, RoutedEventArgs e) {

        }
    }
}
