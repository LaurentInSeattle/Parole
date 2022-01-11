namespace Parole
{
    using Lyt.CoreMvvm;

    using Parole.Game;

    using System;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Media;

    /// <summary> Interaction logic for MainWindow.xaml </summary>
    public partial class MainWindow : Window
    {
        private int skipKeys; 

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
            CompositionTarget.Rendering += new EventHandler(this.CompositionTargetRendering);
        }


        private void CompositionTargetRendering(object? sender, EventArgs e)
        {
            -- this.skipKeys;
            if ( this.skipKeys >= 0)
            {
                return;
            }

            foreach (var key in keys)
            {
                if ((Keyboard.GetKeyStates(key) & KeyStates.Down) > 0)
                {
                    this.skipKeys = 17;
                    // Debug.WriteLine(key.ToString()); 
                    string keyString = key.ToString();
                    if (!string.IsNullOrEmpty(keyString))
                    {
                        Messenger.Instance.Send<KeyMessage>(new KeyMessage(keyString));
                    } 
                }
            }

            foreach (var key in controlKeys)
            {
                if ((Keyboard.GetKeyStates(key) & KeyStates.Down) > 0)
                {
                    this.skipKeys = 17;
                    // Debug.WriteLine(key.ToString()); 
                    Messenger.Instance.Send<ControlMessage>(new ControlMessage(key));
                }
            }
        }
    }
}
