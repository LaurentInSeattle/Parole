namespace Parole;

/// <summary> Interaction logic for MainWindow.xaml </summary>
public partial class MainWindow : Window
{
    private WebViewWindow webView;

    // Italian keys 
    private readonly Key[] keys =
    [
        Key.A, Key.B, Key.C,
        Key.D, Key.E, Key.F,
        Key.G, Key.H, Key.I,
        // No J, No K 
        Key.L, Key.M,
        Key.N, Key.O, Key.P,
        Key.Q, Key.R, Key.S,
        Key.T, Key.U, Key.V,
        // No W X Y 
        Key.Z,
    ];

    private readonly Key[] controlKeys = [Key.Enter, Key.Delete, Key.Back, Key.Space];

    public MainWindow()
    {
        this.InitializeComponent();
        this.Loaded += (s, e) =>
        {
            new GameBindable(this.gameView);
            this.webView = new WebViewWindow();
            webView.Show();
        };
        this.PreviewKeyUp += this.MainWindowPreviewKeyUp;
        this.Closing += (s, e) =>
        {
            try { this.webView.Close(); } catch { /* swallow everything */ }
        };
    }

    private void MainWindowPreviewKeyUp(object sender, KeyEventArgs e)
    {
        var key = e.Key;
        bool foundCharacter = (from knownKey in this.keys where knownKey == key select key).Any();
        bool foundControl = (from knownKey in this.controlKeys where knownKey == key select key).Any();
        if (!foundCharacter && !foundControl)
        {
            e.Handled = false;
            return;
        }

        if (foundCharacter)
        {
            string keyString = key.ToString();
            if (!string.IsNullOrEmpty(keyString))
            {
                // MUST do that first
                e.Handled = true;
                Schedule.OnUiThread(50, this.SendKeyMessage, keyString);
            }
        }
        else if (foundControl)
        {
            // MUST do that first
            e.Handled = true;
            Schedule.OnUiThread(50, this.SendControlMessage, key);
        }

        e.Handled = false;
    }

    private void SendKeyMessage(string keyString)
        => Messenger.Instance.Send(new KeyMessage(keyString));

    private void SendControlMessage(Key key)
        => Messenger.Instance.Send(new ControlMessage(key));
}
