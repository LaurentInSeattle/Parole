namespace Parole.Game
{
    using System.Windows.Controls;

    /// <summary> Interaction logic for LetterControl.xaml </summary>
    public partial class LetterControl : UserControl
    {
        public LetterControl() => this.InitializeComponent();

        public Grid LetterGrid => this.letterGrid;
    }
}
