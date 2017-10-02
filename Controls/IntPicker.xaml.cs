
using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Controls {
    public sealed partial class IntPicker : UserControl {

        public event EventHandler<int> ValueChanged = delegate { };
        public IntPicker() {
            InitializeComponent();
        }
        private void Reduce_Tapped(object sender, TappedRoutedEventArgs e) {
            var currentValue = int.Parse(content.Text);

            if (currentValue == 0) {
                return;
            }
            content.Text = (currentValue - 1).ToString();
            ValueChanged.Invoke(this, currentValue);
        }

        private void Increase_Tapped(object sender, TappedRoutedEventArgs e) {
            var currentValue = int.Parse(content.Text);
            content.Text = (currentValue + 1).ToString();
            ValueChanged.Invoke(this, currentValue);
        }
    }
}
