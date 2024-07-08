namespace Parole.Game;

public sealed class KeyBindable : Bindable<KeyControl>
{
    public KeyBindable() : base()
    {
        this.Clear();
        this.ClickCommand = new Command(this.OnClick);
    }

    public void Update(CharacterPlacement characterPlacement)
    {
        var ti = Theme.Instance;
        this.BorderBrush = ti.BoxBorder;
        this.TextBrush = characterPlacement switch
        {
            CharacterPlacement.Absent => ti.TextAbsent,
            CharacterPlacement.Present => ti.Text,
            CharacterPlacement.Exact => ti.Text,
            _ => ti.BoxUnknown,
        };
        this.BackgroundBrush = characterPlacement switch
        {
            CharacterPlacement.Absent => ti.BoxAbsent,
            CharacterPlacement.Present => ti.BoxPresent,
            CharacterPlacement.Exact => ti.BoxExact,
            _ => ti.BoxUnknown,
        };

        if (characterPlacement == CharacterPlacement.Absent)
        {
            this.IsDisabled = true; 
        }
    }
    public bool IsDisabled {  get; private set; }   

    public void Clear()
    {
        var ti = Theme.Instance;
        this.BorderBrush = ti.BoxBorder;
        this.BackgroundBrush = ti.BoxUnknown;
        this.IsDisabled = false;
    }

    public void SetText(string text)
    {
        var ti = Theme.Instance;
        this.TextBrush = ti.Text;
        this.Text = text;
    }

    private void OnClick(object _)
    {
        if ( this.Text == "Invio")
        {
            Messenger.Instance.Send<ControlMessage>(new ControlMessage(Key.Enter));
        }
        else if (this.Text.StartsWith('⇦'))
        {
            Messenger.Instance.Send<ControlMessage>(new ControlMessage(Key.Back));
        }
        else
        {
            if (!this.IsDisabled)
            {
                Messenger.Instance.Send<KeyMessage>(new KeyMessage(this.Text));
            } 
        }
    }

    #region Bound Properties 

    public Brush BorderBrush { get => this.Get<Brush>(); set => this.Set(value); }

    public Brush BackgroundBrush { get => this.Get<Brush>(); set => this.Set(value); }

    public Brush TextBrush { get => this.Get<Brush>(); set => this.Set(value); }

    public string Text { get => this.Get<string>(); set => this.Set(value); }

    /// <summary> Gets or sets the ClickCommand bound property.</summary>
    public ICommand ClickCommand { get => this.Get<ICommand>(); set => this.Set(value); }

    #endregion Bound Properties 
}
