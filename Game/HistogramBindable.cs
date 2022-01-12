namespace Parole.Game
{
    using Lyt.CoreMvvm;

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public sealed class HistogramBindable : Bindable<HistogramControl>
    {
        public HistogramBindable(HistogramControl histogramControl) : base(histogramControl)
        {

        }
    }
}
