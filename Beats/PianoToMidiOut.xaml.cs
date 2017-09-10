using System;
using Windows.UI.Xaml.Controls;

namespace Beats
{
    public sealed partial class PianoToMidiOut : UserControl
    {
        public PianoToMidiOut()
        {
            this.InitializeComponent();
        }

        private void PianoControl_OnKeyPressedEvent(object sender, EventArgs e) {
            MidiSelector.SendOut(0, 60, 127);
        }

    }
}
