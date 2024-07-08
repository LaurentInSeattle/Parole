namespace Parole.Game;

using System.Windows.Controls;

/// <summary> Interaction logic for HistogramControl.xaml </summary>
public partial class HistogramControl : UserControl
{
    public HistogramControl() => this.InitializeComponent();

    public Grid HistogramGrid => this.histogramGrid;
}
