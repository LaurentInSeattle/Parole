namespace Parole.Game
{
    using Lyt.CoreMvvm;

    using Parole.Model;

    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    public enum State
    {
        Running,
        Ended,
    }

    public sealed class GameBindable : Bindable<GameView>
    {
        private State gameState;
        private Table table;
        private LetterBindable [,] letterBindables;
        private DateTime startTime; 

        private BackgroundWorker endGameAnimationWorker;

        private readonly string[,] keyboardLayout = new string[4, 8]
        {
            { "Q" , "E" , "R", "T", "U", "I", "O", "P"},
            { "A" , "S" , "D", "F", "G", "H", "L", " "},
            { "Z" , "C" , "V", "B", "N", "M", " ", "⇦"},
            { "à" , "è" , "é", "ì", "ò", "ù", " ", "Enter"},
        };

        private Dictionary<string, KeyBindable> keyBindables; 

        public GameBindable(GameView gameView) : base(gameView)
        {
            // General init
            this.gameState = State.Ended;

            // Commands 
            this.StartCommand = new Command(this.OnStartGame);
            //this.EndGameCommand = new Command(this.OnCanEndGame, this.OnEndGame);
            //this.PauseGameCommand = new Command(this.OnCanPauseGame, this.OnPause);
            //this.ResumeGameCommand = new Command(this.OnCanResumeGame, this.OnResume);
            //this.HideEndGameInfoCommand = new Command(this.OnHideEndGameInfo);

            // Prepare for end game 
            //this.InitializeEndGameAnimationWorker();
            //this.endGameBlocksBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#20FFFFFF"));
            //this.endGameBlocksBrush.Freeze();

            // Prepare for new game 
            this.IsEndGameInfoVisible = false;
            this.FirstStartVisibility = Visibility.Visible;

            Messenger.Instance.Register<KeyMessage>(this.OnKeyPress);
            Messenger.Instance.Register<ControlMessage>(this.OnControlKeyPress);
            Words.Instance.Load();
            this.SetupTableGrid();
            this.keyBindables = new Dictionary<string, KeyBindable>(); 
            this.SetupKeyboardGrid();
        }

        private void SetupKeyboardGrid()
        {
            var grid = this.View.KeyboardGrid;
            for (int row = 0; row < 4; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    string key = this.keyboardLayout[row, col];
                    if( string.IsNullOrWhiteSpace(key))
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
            this.letterBindables = new LetterBindable[6, Word.Length];
            var grid = this.View.TableGrid;
            for (int row = 0; row < 6; row++)
            {
                for (int col = 0; col < Word.Length; col++)
                {
                    var letterBindable = Binder<LetterControl, LetterBindable>.Create();
                    this.letterBindables[row, col] = letterBindable;   
                    var control = letterBindable.View;
                    _ = grid.Children.Add(control);
                    control.SetValue(Grid.RowProperty, row);
                    control.SetValue(Grid.ColumnProperty, col);
                }
            }
        }

        private void ClearTableGrid()
        {
            for (int row = 0; row < 6; row++)
            {
                for (int col = 0; col < Word.Length; col++)
                {
                    var letterBindable = this.letterBindables[row, col] ;
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

        private void OnStartGame(object _ )
        {
            this.table = new Table();
            this.gameState = State.Running;
            this.startTime = DateTime.Now;
            this.ClearTableGrid();
            this.ClearKeyboard();
            this.StartVisibility = Visibility.Hidden;
            this.Hide();
        }

        private void OnKeyPress(KeyMessage keyMessage) 
            => Dispatch.OnUiThread(this.OnKeyPressUi, keyMessage);
        
        private void OnKeyPressUi(KeyMessage keyMessage)
        {
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
            this.MessageVisibility = Visibility.Hidden;
            // Debug.WriteLine(controlMessage.Key.ToString());
            if (!this.IsGameRunning)
            {
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
                        int row = this.table.IsGameOver ? this.table.CurrentRow : this.table.CurrentRow - 1;
                        this.RefreshRowOnEnter(row);
                        this.RefreshKeyboard(); 
                        if (this.table.IsGameOver)
                        {
                            this.StartVisibility = Visibility.Visible;
                            if ( this.table.IsWon)
                            {
                                // Message: Win
                                this.Show("Partita Finita: Hai Vinto!");
                            }
                            else
                            {
                                // Message: Lost
                                this.Show("Partita Finita: Perdi...");
                            }
                        }
                    } 
                    else
                    {
                        // Message: Not in list 
                        this.Show("Parola Sconosciuta") ; 
                    }
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

        private void Show ( string message )
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
            var word = this.table.WordAt(row);
            var placement = this.table.PlacementAt(row);
            for (int col = 0; col < Word.Length; col++)
            {
                var letterBindable = this.letterBindables[row, col];
                letterBindable.Update(word.Get(col), placement[col]);
            }
        }

        private void RefreshKeyboard()
        {
            var absent = this.table.AbsentLetters();
            foreach (char letter in absent)
            {
                string letterString = letter.ToString();    
                if(this.keyBindables.TryGetValue(letterString, out var keyBindable))
                {
                    keyBindable.Update(CharacterPlacement.Absent);
                }
            }

            var present = this.table.PresentLetters();
            foreach (char letter in present)
            {
                string letterString = letter.ToString();
                if (this.keyBindables.TryGetValue(letterString, out var keyBindable))
                {
                    keyBindable.Update(CharacterPlacement.Present);
                }
            }

            var exact = this.table.ExactLetters();
            foreach (char letter in exact)
            {
                string letterString = letter.ToString();
                if (this.keyBindables.TryGetValue(letterString, out var keyBindable))
                {
                    keyBindable.Update(CharacterPlacement.Exact);
                }
            }
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

        /// <summary> Gets or sets the StartCommand bound property.</summary>
        public ICommand StartCommand { get => this.Get<ICommand>(); set => this.Set(value); }

        public Visibility StartVisibility { get => this.Get<Visibility>(); set => this.Set(value); }

        public string Message { get => this.Get<string>(); set => this.Set(value); }

        public Visibility MessageVisibility { get => this.Get<Visibility>(); set => this.Set(value); }

        public Visibility EndGameInfoVisibility { get => this.Get<Visibility>(); set => this.Set(value); }

        public Visibility FirstStartVisibility { get => this.Get<Visibility>(); set => this.Set(value); }

    }
}
