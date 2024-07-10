using Parole.Model;

namespace Parole.Game;

public enum State
{
    Idle,
    Ready,
    Running,
    Ended,
}

public sealed class GameBindable : Bindable<GameView>
{
    private LetterBindable[,] letterBindables;
    private Dictionary<string, KeyBindable> keyBindables;
    private Table table;
    private bool easy;
    private State gameState;
    private bool isAnimating;
    private DateTime startTime;
    private DispatcherTimer clockTimer;

    private readonly string[,] italianKeyboardLayout = new string[4, 8]
    {
        { "Q" , "E" , "R", "T", "U", "I", "O", "P"},
        { "A" , "S" , "D", "F", "G", "H", "L", " "},
        { "Z" , "C" , "V", "B", "N", "M", " ", "⇦ Canc"},
        { "à" , "è" , "é", "ì", "ò", "ù", " ", "Invio"},
    };

    // Bulgarian 
    // А Б В Г Д Е Ж З И Й К Л М Н О П Р С Т У Ф Х Ц Ч Ш Щ Ъ Ь Ю Я  
    //private readonly string[,] bulgarianKeyboardLayout = new string[4, 8]
    //{
    //    { "Я" , "В" , "Е", "Р", "Т", "Ъ", "У", "И" },
    //    { "А" , "С" , "Д", "Ф", "Г", "Х", "Й", "К" },
    //    { "З" , "Ь" , "Ц", "Ж", "Б", "Н", "М", "⇦" },
    //    { "О" , "П" , "Ш", "Щ", "Л", "Ю", " ", "Enter"},
    //};

// #pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public GameBindable(GameView gameView) : base(gameView)
// #pragma warning restore CS8618 
    {
        this.gameState = State.Idle;
        this.StartCommand = new Command(this.OnStartGame);
        var ti = Theme.Instance;
        this.BorderBrush = ti.BoxBorder;
        this.TextBrush = ti.Text;
        this.BackgroundBrush = ti.BoxUnknown;
        this.isAnimating = false;
        this.IsEndGameInfoVisible = false;
        this.FirstStartVisibility = Visibility.Visible;
        this.Solution = string.Empty;
        this.SolutionVisibility = Visibility.Hidden;
        this.MainGridVisibility = Visibility.Hidden;
        Messenger.Instance.Register<KeyMessage>(this.OnKeyPress);
        Messenger.Instance.Register<ControlMessage>(this.OnControlKeyPress);
        this.Select = new SelectBindable(this.View.SelectControl, this.OnWordLengthSelected);
    }

    private void OnWordLengthSelected()
    {
        Table.Rows = Word.Length == 5 ? 6 : 7;
        // Words.Instance.PreLoadBulgarian();
        Words.Instance.LoadItalian();
        History.Instance.Load();
        this.letterBindables = new LetterBindable[Table.Rows, Word.Length];
        this.SetupTableGrid();
        this.keyBindables = [];
        this.Histogram = new HistogramBindable(this.View.HistogramControl);
        this.SetupKeyboardGrid();
        this.ShowStatistics();
        this.table = new Table(this.easy);
        this.MainGridVisibility = Visibility.Visible;
        this.gameState  = State.Ready;
    }

    private void SetupKeyboardGrid()
    {
        var grid = this.View.KeyboardGrid;
        for (int row = 0; row < 4; row++)
        {
            for (int col = 0; col < 8; col++)
            {             
                // TODO
                // string key = this.bulgarianKeyboardLayout[row, col];
                string key = this.italianKeyboardLayout[row, col];
                if (string.IsNullOrWhiteSpace(key))
                {
                    continue;
                }

                var keyBindable = Binder<KeyControl, KeyBindable>.Create();
                keyBindable.SetText(key.ToString());
                var control = keyBindable.View;
                _ = grid.Children.Add(control);
                control.SetValue(Grid.RowProperty, row);
                control.SetValue(Grid.ColumnProperty, col);
                this.keyBindables.Add(key, keyBindable);
            }
        }
    }

    private void SetupTableGrid()
    {
        this.TableGrid65Visibility = Word.Length == 6 ? Visibility.Hidden : Visibility.Visible;
        this.TableGrid76Visibility = Word.Length == 5 ? Visibility.Hidden : Visibility.Visible;
        var grid = Word.Length == 6 ? this.View.TableGrid76 : this.View.TableGrid65;
        for (int row = 0; row < Table.Rows; row++)
        {
            for (int col = 0; col < Word.Length; col++)
            {
                var letterBindable = Binder<LetterControl, LetterBindable>.Create();
                this.letterBindables[row, col] = letterBindable;
                var control = letterBindable.View;
                letterBindable.Setup();
                _ = grid.Children.Add(control);
                control.SetValue(Grid.RowProperty, row);
                control.SetValue(Grid.ColumnProperty, col);
            }
        }
    }

    private void ClearTableGrid()
    {
        for (int row = 0; row < Table.Rows; row++)
        {
            for (int col = 0; col < Word.Length; col++)
            {
                var letterBindable = this.letterBindables[row, col];
                letterBindable.Clear();
            }
        }
    }

    private void ClearKeyboard()
    {
        foreach (var keyBindable in this.keyBindables.Values)
        {
            keyBindable.Clear();
        }
    }

    private bool IsGameRunning => this.gameState == State.Running;

    private bool CanGameStart => this.gameState == State.Ready || this.gameState == State.Ended ;

    private void OnStartGame(object _) => this.StartGame();

    private void StartGame()
    {   
        if ( ! this.CanGameStart )
        {
            return; 
        }

        this.easy = !this.easy;
        // this.table = new Table(this.easy);
        this.table = new Table(easy:true);
        this.gameState = State.Running;
        this.startTime = DateTime.Now;
        this.ClearTableGrid();
        this.ClearKeyboard();
        this.StartVisibility = Visibility.Hidden;
        this.Solution = string.Empty;
        this.SolutionVisibility = Visibility.Hidden;
        this.Hide();
        this.clockTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(1300),
            IsEnabled = true,
        };
        this.clockTimer.Tick += this.OnClockTimerTick;
        this.clockTimer.Start();
    }

    private void OnClockTimerTick(object sender, EventArgs e)
    {
        this.ClockVisibility = Visibility.Visible;
        TimeSpan elapsed = DateTime.Now - this.startTime;
        this.Clock = elapsed.ToString(@"d\.hh\:mm\:ss");
    }

    private void StopClockTimer()
    {
        if (this.clockTimer != null)
        {
            this.clockTimer.Stop();
            this.clockTimer.Tick -= this.OnClockTimerTick;
            this.clockTimer = null;
        }

        this.ClockVisibility = Visibility.Hidden;
        this.Clock = string.Empty;
    }

    private void OnKeyPress(KeyMessage keyMessage)
        => Dispatch.OnUiThread(this.OnKeyPressUi, keyMessage);

    private void OnKeyPressUi(KeyMessage keyMessage)
    {
        if (this.isAnimating)
        {
            return;
        }

        // Debug.WriteLine(keyMessage.Key);
        this.MessageVisibility = Visibility.Hidden;
        if (!this.IsGameRunning)
        {
            return;
        }

        // send to game keyMessage.Key
        this.table.OnNewChar(keyMessage.Key);
        this.RefreshRow(this.table.CurrentRow);
    }

    private void OnControlKeyPress(ControlMessage controlMessage)
        => Dispatch.OnUiThread(this.OnControlKeyPressUi, controlMessage);

    private void OnControlKeyPressUi(ControlMessage controlMessage)
    {
        if (this.isAnimating)
        {
            return;
        }

        this.MessageVisibility = Visibility.Hidden;
        // Debug.WriteLine(controlMessage.Key.ToString());
        if (this.CanGameStart)
        {
            this.StartGame();
            return;
        }

        var key = controlMessage.Key;
        if (key == Key.Enter)
        {
            var word = this.table.WordAt(this.table.CurrentRow);
            if (word.IsComplete)
            {
                // Evaluate
                if (this.table.OnEnter())
                {
                    string url = string.Format("https://it.wiktionary.org/wiki/{0}", word.AsString().ToLower());
                    Messenger.Instance.Send(new NavigationMessage(url)); 
                    int row = this.table.IsGameOver ? this.table.CurrentRow : this.table.CurrentRow - 1;
                    this.RefreshRowOnEnter(row);
                    this.RefreshKeyboard();
                    if (this.table.IsGameOver)
                    {
                        this.OnGameOver();
                    }
                }
                else
                {
                    // Message: Not in list 
                    this.Show("Parola Sconosciuta");
                }
            }
        }
        else if (key == Key.Space)
        {
            if (this.CanGameStart)
            {
                this.StartGame();
            }
        }
        else
        {
            // Backspace or delete
            this.table.OnBackspace();
            this.RefreshRow(this.table.CurrentRow);
        }
    }

    private void Hide()
    {
        this.Message = string.Empty;
        this.MessageVisibility = Visibility.Hidden;
    }

    private void Show(string message)
    {
        this.Message = message;
        this.MessageVisibility = Visibility.Visible;
    }

    private void RefreshRow(int row)
    {
        var word = this.table.WordAt(row);
        for (int col = 0; col < Word.Length; col++)
        {
            var letterBindable = this.letterBindables[row, col];
            letterBindable.Update(word.Get(col), CharacterPlacement.Unknown);
        }
    }

    private void RefreshRowOnEnter(int row)
    {
        this.isAnimating = true;

        var word = this.table.WordAt(row);
        var placement = this.table.PlacementAt(row);
        int delay = 50;
        for (int col = 0; col < Word.Length; col++)
        {
            var letterBindable = this.letterBindables[row, col];
            CharacterPlacement characterPlacement = placement[col];
            char character = word.Get(col);
            delay += col * 150;
            var tuple = new Tuple<LetterBindable, char, CharacterPlacement>(letterBindable, character, characterPlacement);
            Schedule.OnUiThread(delay, this.UpdateLetter, tuple);
        }

        Schedule.OnUiThread(1000, () => this.isAnimating = false);
    }

    private void UpdateLetter(Tuple<LetterBindable, char, CharacterPlacement> tuple)    
        => tuple.Item1.Update(tuple.Item2, tuple.Item3, animate:true);

    private void RefreshKeyboard()
    {
        void Refresh( HashSet<char> hash, CharacterPlacement placement )
        {
            foreach (char letter in hash)
            {
                string letterString = letter.ToString();
                if (this.keyBindables.TryGetValue(letterString, out var keyBindable))
                {
                    keyBindable.Update(placement);
                }
            }
        }

        Refresh(this.table.AbsentLetters(), CharacterPlacement.Absent);
        Refresh(this.table.PresentLetters(), CharacterPlacement.Present);
        Refresh(this.table.ExactLetters(), CharacterPlacement.Exact);
    }

    private void ShowMessage ()
    {
        if (this.table.IsWon)
        {
            // Message: Win
            this.Show("Partita Finita\n Hai Vinto!");
        }
        else
        {
            // Message: Lost
            this.Show("Partita Finita\n Perdi...");
            this.Solution = this.table.Solution;
            this.SolutionVisibility = Visibility.Visible;
            string url = string.Format("https://it.wiktionary.org/wiki/{0}", this.Solution.ToLower());
            Messenger.Instance.Send(new NavigationMessage(url));
        }
    }

    private void OnGameOver()
    {
        this.gameState = State.Ended;
        Schedule.OnUiThread(5000, () => this.StartVisibility = Visibility.Visible);
        this.StopClockTimer();
        Schedule.OnUiThread(2500, () => this.ShowMessage());

        var gameEntry =
            new History.GameEntry
            {
                Duration = DateTime.Now - this.startTime,
                IsWon = this.table.IsWon,
                Started = this.startTime,
                Steps = this.table.CurrentRow,
                Word = this.table.Solution,
            };
        History.Instance.Add(gameEntry);
        History.Instance.Save();
        this.ShowStatistics();
    }

    private void ShowStatistics()
    {
        var statistics = History.Instance.EvaluateStatistics();

        this.Plays =
            string.Format(
                "Giocato {0} Partite per {1} Minuti.",
                statistics.Wins + statistics.Losses,
                (int)(0.5 + statistics.Duration.TotalMinutes));
        this.Wins = string.Format("Vince : {0} ", statistics.Wins);
        this.Losses = string.Format("Perdite : {0} ", statistics.Losses);
        this.WinRate = string.Format("Percentuale di Vincita : {0} % ", statistics.WinRate);
        this.BestStreak = string.Format("Serie più Lunga : {0}", statistics.BestStreak);
        this.CurrentStreak = string.Format("Serie in Corso : {0}", statistics.CurrentStreak);
        this.Histogram.Update(statistics);
    }

    public bool IsEndGameInfoVisible
    {
        get => this.Get<bool>();
        set
        {
            this.Set(value);
            this.EndGameInfoVisibility = value ? Visibility.Visible : Visibility.Collapsed;
        }
    }

    #region Bound Properties 

    public Visibility MainGridVisibility { get => this.Get<Visibility>(); set => this.Set(value); }

    public Visibility TableGrid65Visibility { get => this.Get<Visibility>(); set => this.Set(value); }

    public Visibility TableGrid76Visibility { get => this.Get<Visibility>(); set => this.Set(value); }

    /// <summary> Gets or sets the StartCommand bound property.</summary>
    public ICommand StartCommand { get => this.Get<ICommand>(); set => this.Set(value); }

    public Visibility StartVisibility { get => this.Get<Visibility>(); set => this.Set(value); }

    public string Message { get => this.Get<string>(); set => this.Set(value); }

    public Visibility MessageVisibility { get => this.Get<Visibility>(); set => this.Set(value); }

    public string Clock { get => this.Get<string>(); set => this.Set(value); }

    public Visibility ClockVisibility { get => this.Get<Visibility>(); set => this.Set(value); }

    public string Solution { get => this.Get<string>(); set => this.Set(value); }

    public Visibility SolutionVisibility { get => this.Get<Visibility>(); set => this.Set(value); }

    public Visibility EndGameInfoVisibility { get => this.Get<Visibility>(); set => this.Set(value); }

    public Visibility FirstStartVisibility { get => this.Get<Visibility>(); set => this.Set(value); }

    public string Plays { get => this.Get<string>(); set => this.Set(value); }

    public string Wins { get => this.Get<string>(); set => this.Set(value); }

    public string Losses { get => this.Get<string>(); set => this.Set(value); }

    public string WinRate { get => this.Get<string>(); set => this.Set(value); }

    public string BestStreak { get => this.Get<string>(); set => this.Set(value); }

    public string CurrentStreak { get => this.Get<string>(); set => this.Set(value); }

    public HistogramBindable Histogram { get => this.Get<HistogramBindable>(); set => this.Set(value); }

    public SelectBindable Select { get => this.Get<SelectBindable>(); set => this.Set(value); }

    public Brush BorderBrush { get => this.Get<Brush>(); set => this.Set(value); }

    public Brush BackgroundBrush { get => this.Get<Brush>(); set => this.Set(value); }

    public Brush TextBrush { get => this.Get<Brush>(); set => this.Set(value); }

    #endregion Bound Properties 
}
