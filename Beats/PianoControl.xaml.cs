﻿
using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Beats
{
    public sealed partial class PianoControl : UserControl
    {
        public PianoControl()
        {
            this.InitializeComponent();
        }

        private void WhiteKey0_PointerPressPreview(object sender, System.EventArgs e)
        {
            KeyPressedEvent.Invoke(sender, e);
        }

        private void WhiteKey0_PointerReleasedPreview(object sender, System.EventArgs e)
        {

        }

        public event EventHandler KeyPressedEvent = delegate { };

    }
}
