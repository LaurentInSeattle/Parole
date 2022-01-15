namespace Parole.Game
{
    using Lyt.CoreMvvm;

    using Parole.Model;

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Media;

    public sealed class LetterBindable : Bindable<LetterControl>
    {
        private const double smallSize = 132;
        private const double largeSize = 160;
        private const double smallGridSize = 120;
        private const double largeGridSize = 148;

        public LetterBindable() : base() => this.Clear();

        public void Update(char letter, CharacterPlacement characterPlacement, bool animate = false)
        {
            var ti = Theme.Instance;
            this.BorderBrush = ti.BoxBorder;
            this.TextBrush = ti.Text;
            this.Text = letter.ToString();
            this.BackgroundBrush = characterPlacement switch
            {
                CharacterPlacement.Absent => ti.BoxAbsent,
                CharacterPlacement.Present => ti.BoxPresent,
                CharacterPlacement.Exact => ti.BoxExact,
                _ => ti.BoxUnknown,
            };

            if (animate)
            {
                var letterGrid = this.View.LetterGrid;
                letterGrid.RowDefinitions[0].Height = new GridLength(0.0, GridUnitType.Pixel);
                letterGrid.ColumnDefinitions[0].Width = new GridLength(0.0, GridUnitType.Pixel);
                double size = Word.Length == 6 ? smallGridSize : largeGridSize;
                letterGrid.AnimateRow(0, size, 200);
                letterGrid.AnimateColumn(0, size, 200);
            } 
        }

        public void Clear()
        {
            var ti = Theme.Instance;
            this.BorderBrush = ti.BoxBorder;
            this.BackgroundBrush = ti.BoxUnknown;
            this.TextBrush = Brushes.Transparent;
            this.Text = string.Empty;
            if (this.View != null)
            {
                var letterGrid = this.View.LetterGrid;
                double size = Word.Length == 6 ? smallGridSize : largeGridSize;
                letterGrid.RowDefinitions[0].Height = new GridLength(size, GridUnitType.Pixel);
                letterGrid.ColumnDefinitions[0].Width = new GridLength(size, GridUnitType.Pixel);
            } 
        }

        public void Setup()
        {
            bool isSmall = Word.Length == 6;
            var control = this.View;
            control.Height = isSmall ? smallSize : largeSize;
            control.Width = isSmall ? smallSize : largeSize;
            var grid = this.View.LetterGrid;
            double size = isSmall ? smallGridSize : largeGridSize;
            grid.Height = size;
            grid.Width = size;
            grid.RowDefinitions[0].Height = new GridLength(size, GridUnitType.Pixel);
            grid.ColumnDefinitions[0].Width = new GridLength(size, GridUnitType.Pixel);
        }

        #region Bound Properties 

        public Brush BorderBrush { get => this.Get<Brush>(); set => this.Set(value); }

        public Brush BackgroundBrush { get => this.Get<Brush>(); set => this.Set(value); }

        public Brush TextBrush { get => this.Get<Brush>(); set => this.Set(value); }

        public string Text { get => this.Get<string>(); set => this.Set(value); }

        #endregion Bound Properties 
    }
}
