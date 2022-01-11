namespace Parole.Game
{
    using Lyt.CoreMvvm;

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Media;

    public class Theme : Singleton<Theme>
    {
        public enum Style
        {
            Default, 
        }

        public Brush Background { get; private set; }

        public Brush Text { get; private set; }
        
        public Brush BoxBorder { get; private set; }
        
        public Brush BoxUnknown { get; private set; }
        
        public Brush BoxAbsent { get; private set; }
        
        public Brush BoxPresent { get; private set; }
        
        public Brush BoxExact { get; private set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private Theme()
#pragma warning restore CS8618 
        {
            this.Set(Style.Default);
        }

        public void Set ( Style style)
        {
            switch (style)
            {
                default:
                case Style.Default:
                    this.Background = Brushes.Black;
                    this.Text = Brushes.AntiqueWhite;
                    this.BoxBorder = Brushes.Aquamarine;
                    this.BoxUnknown = Brushes.DarkGray;
                    this.BoxPresent = Brushes.DarkOrange;
                    this.BoxAbsent = Brushes.LightGray;
                    this.BoxExact = Brushes.MediumSeaGreen;
                    break;
            }
        }
    }
}
