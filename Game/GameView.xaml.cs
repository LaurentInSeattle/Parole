namespace Parole.Game
{
    using System.Windows.Controls;

    /// <summary> Interaction logic for GameView.xaml </summary>
    public partial class GameView : UserControl
    {
        public GameView() => this.InitializeComponent();

        public Grid TableGrid65 => this.tableGrid65;

        public Grid TableGrid76 => this.tableGrid76;

        public Grid KeyboardGrid => this.keyboardGrid;

        public HistogramControl HistogramControl => this.histogramControl;

        public SelectControl SelectControl => this.selectControl;
    }
}
