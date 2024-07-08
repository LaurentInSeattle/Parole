namespace Parole.Game;

public sealed class HistogramBindable : Bindable<HistogramControl>
{
    private readonly Dictionary<int, HistogramRowBindable> histogramRowBindables; 
    
    public HistogramBindable(HistogramControl histogramControl) : base(histogramControl)
    {
        this.histogramRowBindables = [];
        var grid = histogramControl.HistogramGrid;
        for (int row = 0; row < Table.Rows; ++row)
        {
            var histogramRowBindable = Binder<HistogramRowControl, HistogramRowBindable>.Create();
            var control = histogramRowBindable.View;
            _ = grid.Children.Add(control);
            control.SetValue(Grid.RowProperty, row);
            control.SetValue(Grid.ColumnProperty, 0);
            this.histogramRowBindables.Add(row, histogramRowBindable);
        } 
    }

    public void Update(History.Statistics statistics)
    {
        var histogramValues = statistics.Histogram; 
        int maxValue = (from value in histogramValues select value).Max();
        for (int row = 0; row < Table.Rows; ++row)
        {
            int histogramValue = histogramValues[row];
            if ( this.histogramRowBindables.TryGetValue(row, out HistogramRowBindable histogramRowBindable))
            {
                histogramRowBindable?.Update(row+1, histogramValue, maxValue);
            }
        }
    }
}
