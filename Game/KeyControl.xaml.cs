namespace Parole.Game
{
    using System.Windows.Controls;
    using System.Windows.Input;

    /// <summary> Interaction logic for KeyControl.xaml </summary>
    public partial class KeyControl : UserControl
    {
        public KeyControl()
        {
            this.InitializeComponent();
            this.PreviewKeyDown += OnKeyControlPreviewKeyDown;
        }

        private void OnKeyControlPreviewKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }
    }
}
