using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Controls {
    public sealed partial class GridControl {
        public GridControl() {
            InitializeComponent();
            var rowDef = new RowDefinition();
            rowDef.Height = new GridLength(64);

            rowDef = new RowDefinition {Height = new GridLength(64)};

            Grid.RowDefinitions.Add(rowDef);
            rowDef = new RowDefinition { Height = new GridLength(64) };

            Grid.RowDefinitions.Add(rowDef);

            var colDef = new ColumnDefinition();
            colDef.Width = new GridLength(64);
            Grid.ColumnDefinitions.Add(colDef);

            colDef.Width = new GridLength(64);
            Grid.ColumnDefinitions.Add(colDef);
        }
    }
}
