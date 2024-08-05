
namespace Parole.Game;

public partial class WebViewWindow : Window
{
    // URL for images: "calico" in the example below
    // https://www.google.it/search?q=calico&udm=2

    public WebViewWindow()
    {
        this.InitializeComponent();
        //this.Visibility = Visibility.Hidden;
        Messenger.Instance.Register<NavigationMessage>(this.OnNavigate);
        this.Closing += this.OnClosing;
        this.Loaded += this.OnLoaded;
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        await this.Browser.EnsureCoreWebView2Async();
        this.Browser.ZoomFactor = 1.20;
    }

    private void OnClosing(object sender, CancelEventArgs e)
    {
        this.Visibility = Visibility.Hidden;
        Messenger.Instance.Unregister(this);
    }

    private void OnNavigate(NavigationMessage message)
    {
        this.Visibility = Visibility.Visible;
        var uri = new Uri(message.Url, UriKind.Absolute);
        this.Browser.Source = uri;
    }
}
