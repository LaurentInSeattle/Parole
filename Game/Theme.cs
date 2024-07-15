namespace Parole.Game;

public class Theme : Singleton<Theme>
{
    public enum Style
    {
        Default,
        Translucent,
    }

    public Brush Background { get; private set; }

    public Brush UiText { get; private set; }

    public Brush Text { get; private set; }

    public Brush TextAbsent { get; private set; }

    public Brush BoxBorder { get; private set; }

    public Brush BoxUnknown { get; private set; }

    public Brush BoxAbsent { get; private set; }

    public Brush BoxPresent { get; private set; }

    public Brush BoxExact { get; private set; }

    // #pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public Theme() => this.Set(Style.Translucent);
    // #pragma warning restore CS8618 

    public void Set(Style style)
    {
        switch (style)
        {
            default:
            case Style.Default:
                this.Background = Brushes.Black;
                this.UiText = Brushes.DarkOrange;
                this.Text = Brushes.LavenderBlush;
                this.TextAbsent = Brushes.SlateGray;
                this.BoxBorder = Brushes.Lavender;
                this.BoxUnknown = Brushes.DarkSlateBlue;
                this.BoxPresent = Brushes.DarkOrange;
                this.BoxAbsent = Brushes.DarkSlateGray;
                this.BoxExact = Brushes.MediumSeaGreen;
                break;

            case Style.Translucent:
                this.Background = Brushes.Black;
                this.UiText = Brushes.LightCoral;
                this.Text = Brushes.LavenderBlush;
                this.TextAbsent = Brushes.SlateGray;
                this.BoxBorder = Brushes.PowderBlue;
                this.BoxUnknown = new SolidColorBrush(Color.FromArgb(0x22, 0x48, 0x3D, 0x9B));// Brushes.DarkSlateBlue;
                this.BoxPresent = new SolidColorBrush(Color.FromArgb(0xD0, 0xFF, 0xB0, 0x10));
                this.BoxAbsent = new SolidColorBrush(Color.FromArgb(0xC0, 0x20, 0x30, 0x50));// Brushes.DarkSlateGray;
                this.BoxExact = new SolidColorBrush(Color.FromArgb(0xC0, 0x2F, 0xA0, 0x5F));
                break;
        }
    }
}
