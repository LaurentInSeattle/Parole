namespace Parole.Game;

public sealed class SelectBindable : Bindable<SelectControl>
{
    private Action onSelected; 

    public SelectBindable(SelectControl selectControl, Action onSelected ) : base(selectControl) => this.Setup(onSelected);

    public void Setup(Action onSelected)
    {
        this.onSelected = onSelected;
        this.SelectVisibility = Visibility.Visible;
        this.FiveCommand = new Command(this.OnFive);
        this.SixCommand = new Command(this.OnSix);

        var ti = Theme.Instance;
        this.BorderBrush = ti.BoxBorder;
        this.BackgroundBrush = ti.BoxUnknown;
        this.TextBrush = ti.Text;
    }

    private void OnFive(object _)
    {
        Word.Length = 5;
        this.Dismiss();
    }

    private void OnSix(object _)
    {
        Word.Length = 6;
        this.Dismiss();
    } 

    private void Dismiss ()
    { 
        this.SelectVisibility = Visibility.Hidden;
        this.onSelected?.Invoke();
    }

    #region Bound Properties 

    public Visibility SelectVisibility { get => this.Get<Visibility>(); set => this.Set(value); }

    public ICommand FiveCommand { get => this.Get<ICommand>(); set => this.Set(value); }

    public ICommand SixCommand { get => this.Get<ICommand>(); set => this.Set(value); }

    public Brush BorderBrush { get => this.Get<Brush>(); set => this.Set(value); }

    public Brush BackgroundBrush { get => this.Get<Brush>(); set => this.Set(value); }

    public Brush TextBrush { get => this.Get<Brush>(); set => this.Set(value); }

    #endregion Bound Properties 
}
