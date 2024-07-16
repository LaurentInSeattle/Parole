namespace Parole.Game;

public partial class KeyControl : UserControl
{
    public KeyControl()
    {
        this.InitializeComponent();
        this.PreviewKeyDown += this.OnKeyControlPreviewKeyDown;
    }

    private void OnKeyControlPreviewKeyDown(object sender, KeyEventArgs e) => e.Handled = true;
}
