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
        public LetterBindable(LetterControl letterControl) : base ( letterControl) => this.Clear();

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
                letterGrid.AnimateRow(0, 148.0, 200);
                letterGrid.AnimateColumn(0, 148.0, 200);
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
                letterGrid.RowDefinitions[0].Height = new GridLength(148.0, GridUnitType.Pixel);
                letterGrid.ColumnDefinitions[0].Width = new GridLength(148.0, GridUnitType.Pixel);
            } 
        }

        #region Bound Properties 

        public Brush BorderBrush { get => this.Get<Brush>(); set => this.Set(value); }

        public Brush BackgroundBrush { get => this.Get<Brush>(); set => this.Set(value); }

        public Brush TextBrush { get => this.Get<Brush>(); set => this.Set(value); }

        public string Text { get => this.Get<string>(); set => this.Set(value); }

        #endregion Bound Properties 
    }
}
