namespace Parole.Game;

public sealed class HistogramRowBindable : Bindable<HistogramRowControl>
{
    public HistogramRowBindable() : base() { }

    public void Update(int guessCount, int count, int max)
    {
        this.GuessCount = guessCount.ToString("D");
        this.Count = count.ToString("D");
        this.RectangleWidth = 220.0 * count / (double)max;
    }

    public string Count { get => this.Get<string>(); set => this.Set(value); }

    public string GuessCount { get => this.Get<string>(); set => this.Set(value); }

    public double RectangleWidth { get => this.Get<double>(); set => this.Set(value); }

}
