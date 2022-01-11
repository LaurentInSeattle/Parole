namespace Parole.Game
{
    using System.Windows.Controls;

    /// <summary> Interaction logic for GameView.xaml </summary>
    public partial class GameView : UserControl
    {
        public GameView() => this.InitializeComponent();

        public Grid TableGrid => this.tableGrid;

        public Grid KeyboardGrid => this.keyboardGrid;
    }
}
