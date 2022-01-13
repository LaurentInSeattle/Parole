namespace Parole.Game
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Navigation;
    using System.Windows.Shapes;

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
