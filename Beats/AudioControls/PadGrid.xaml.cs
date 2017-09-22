
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Beats.AudioControls {
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
