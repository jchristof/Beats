using System;
using Windows.UI.Xaml.Controls;

namespace Beats.AudioControls
{
    public sealed partial class KeyboardToAudioFrameOut : UserControl
    {
        public KeyboardToAudioFrameOut()
        {
            this.InitializeComponent();
        }

        private void PianoControl_OnKeyReleasedEvent(object sender, EventArgs e) {
            throw new NotImplementedException();
        }

        private void PianoControl_OnKeyPressedEvent(object sender, EventArgs e) {
            throw new NotImplementedException();
        }

    }
}
