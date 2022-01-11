namespace Parole.Model
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class Table
    {
        private readonly static int rows = 6;

        private readonly Word solution;
        private readonly Word[] words;
        private readonly Placement[] placements;
        private readonly DateTime started;
        
        private int currentCol;
        private DateTime ended;

        public Table()
        {
            this.started = DateTime.Now;
            this.CurrentRow = 0;
            this.currentCol = 0;
            // TODO: load words already played 
            string solutionString = Words.Instance.RandomPick(new HashSet<string> { "hello" });
            this.solution = new Word(solutionString);
            Debug.WriteLine("Solution: " + this.solution.AsString()); 
            this.words = new Word[rows];
            this.placements = new Placement[rows];
            for (int i = 0; i < rows; ++i)
            {
                var word = new Word();
                this.words[i] = word;
                this.placements[i] = word.Evaluate(solution, out bool _);
            }
        }

        public TimeSpan GameTime => DateTime.Now - this.started;

        public Word WordAt(int row) => this.words[row];

        public Placement PlacementAt(int row) => this.placements[row];

        public int CurrentRow { get; private set; }

        public TimeSpan CompletedTime { get; private set; } 

        public bool IsGameOver  { get; private set; }

        public bool IsWon { get; private set; }

        public void OnBackspace()
        {
            var word = this.words[this.CurrentRow];
            if (this.currentCol >= 0)
            {
                if ((this.currentCol > 0) && word.IsEmpty(this.currentCol))
                {
                    word.Clear(this.currentCol - 1);
                }
                else
                {
                    word.Clear(this.currentCol);
                } 
            }

            if (this.currentCol > 0)
            {
                --this.currentCol;
            }
        }

        public void OnNewChar (char character)
        {
            this.words[this.CurrentRow].Set(this.currentCol, character);
            if (this.currentCol < Word.Length - 1)
            {
                ++this.currentCol;
            }
        }

        public void OnEnter()
        {
            this.placements[this.CurrentRow] = this.words[this.CurrentRow].Evaluate(solution, out bool isFound);
            if ( isFound )
            {
                // Win ! 
                this.IsGameOver = true;
                this.IsWon = true;
                this.ended = DateTime.Now;
                this.CompletedTime = this.ended - this.started;
            }
            else
            {
                if( this.CurrentRow == rows - 1 )
                {
                    // lost :(
                    this.IsGameOver = true;
                    this.IsWon = false;
                    this.ended = DateTime.Now;
                }
                else
                {
                    // next row 
                    ++ this.CurrentRow;
                    this.currentCol = 0;    
                }
            }
        }
    }
}
