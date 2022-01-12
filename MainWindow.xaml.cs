namespace Parole
{
    using Lyt.CoreMvvm;

    using Parole.Game;

    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Media;

    /// <summary> Interaction logic for MainWindow.xaml </summary>
    public partial class MainWindow : Window
    {
        private readonly Key[] keys = new Key[]
        {
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
        };

        private readonly Key[] controlKeys = new Key[]
        {
            Key.Enter, Key.Delete, Key.Back,
        };

        public MainWindow()
        {
            this.InitializeComponent();
            this.Loaded += (s, e) => new GameBindable(this.gameView);
            this.PreviewKeyUp += this.MainWindowPreviewKeyUp;
        }

        private void MainWindowPreviewKeyUp(object sender, KeyEventArgs e)
        {
            var key = e.Key;
            // Debug.WriteLine("Entered: " + key.ToString());   

            bool foundCharacter = (from knownKey in this.keys where knownKey == key select key).Any();
            bool foundControl = (from knownKey in this.controlKeys where knownKey == key select key).Any();
            if ( !foundCharacter && ! foundControl)
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

        private void SendKeyMessage ( string keyString )
        {
            // Debug.WriteLine("sending char: " + key.ToString());
            Messenger.Instance.Send<KeyMessage>(new KeyMessage(keyString));
        }

        private void SendControlMessage(Key key)
        {
            // Debug.WriteLine("sending ctrl: " + key.ToString());
            Messenger.Instance.Send<ControlMessage>(new ControlMessage(key));
        }
    }
}
