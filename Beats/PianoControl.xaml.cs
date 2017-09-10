
using System;

namespace Beats
{
    public sealed partial class PianoControl
    {
        public PianoControl()
        {
            this.InitializeComponent();
        }

        private void PointerPressPreview(object sender, System.EventArgs e)
        {
            KeyPressedEvent.Invoke(sender, e);
        }

        private void PointerReleasedPreview(object sender, System.EventArgs e)
        {
            KeyReleasedEvent.Invoke(sender, e);
        }

        public event EventHandler KeyPressedEvent = delegate { };
        public event EventHandler KeyReleasedEvent = delegate { };
    }
}
