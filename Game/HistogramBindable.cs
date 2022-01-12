namespace Parole.Game
{
    using Lyt.CoreMvvm;

    using Parole.Model;

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Controls;

    public sealed class HistogramBindable : Bindable<HistogramControl>
    {
        private readonly Dictionary<int, HistogramRowBindable> histogramRowBindables; 
        
        public HistogramBindable(HistogramControl histogramControl) : base(histogramControl)
        {
            this.histogramRowBindables = new Dictionary<int,HistogramRowBindable>();
            var grid = histogramControl.HistogramGrid;
            for (int row = 0; row < 6; ++row)
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
            for (int row = 0; row < 6; ++row)
            {
                int histogramValue = histogramValues[row];
                if ( this.histogramRowBindables.TryGetValue(row, out HistogramRowBindable? histogramRowBindable))
                {
                    if( histogramRowBindable != null )
                    {
                        histogramRowBindable.Update(row+1, histogramValue, maxValue);
                    }
                }
            }
        }
    }
}
