using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Beats.AudioControls
{
    public class AudioButton : Button
    {
        public event EventHandler PointerPressPreview = delegate { };

        protected override void OnPointerPressed(PointerRoutedEventArgs e)
        {
            PointerPressPreview(this, EventArgs.Empty);
            base.OnPointerPressed(e);
        }

        public event EventHandler PointerReleasedPreview = delegate { };

        protected override void OnPointerReleased(PointerRoutedEventArgs e)
        {
            PointerReleasedPreview(this, EventArgs.Empty);
            base.OnPointerReleased(e);
        }
    }
}
