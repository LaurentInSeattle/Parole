namespace Parole.Game;

public class Theme : Singleton<Theme>
{
    public enum Style
    {
        Default, 
    }

    public Brush Background { get; private set; }

    public Brush Text { get; private set; }

    public Brush TextAbsent { get; private set; }

    public Brush BoxBorder { get; private set; }
    
    public Brush BoxUnknown { get; private set; }
    
    public Brush BoxAbsent { get; private set; }
    
    public Brush BoxPresent { get; private set; }
    
    public Brush BoxExact { get; private set; }

// #pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private Theme() => this.Set(Style.Default);
// #pragma warning restore CS8618 

    public void Set ( Style style)
    {
        switch (style)
        {
            default:
            case Style.Default:
                this.Background = Brushes.Black;
                this.Text = Brushes.LavenderBlush;
                this.TextAbsent = Brushes.SlateGray;
                this.BoxBorder = Brushes.Lavender;
                this.BoxUnknown = Brushes.DarkSlateBlue;
                this.BoxPresent = Brushes.DarkOrange;
                this.BoxAbsent = Brushes.DarkSlateGray;
                this.BoxExact = Brushes.MediumSeaGreen;
                break;
        }
    }
}
