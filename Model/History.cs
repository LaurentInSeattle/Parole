namespace Parole.Model
{
    using Lyt.CoreMvvm;

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml.Serialization;

    public sealed class History : Singleton<History>
    {
        private static readonly string FileName = "parole_history.xml";

        public History()
        {
            this.GameEntries = new List<GameEntry>();
        }

        public List<GameEntry> GameEntries { get; set; }

        public void Load()
        {
            if (File.Exists(FileName))
            {
                var serializer = new XmlSerializer(typeof(List<GameEntry>));
                using var reader = new FileStream(FileName, FileMode.Open);
                var tempList = (List<GameEntry>)serializer.Deserialize(reader);
            }
        }

        public void Save()
        {
            var serializer = new XmlSerializer(this.GetType());
            using var writer = new FileStream(FileName, FileMode.Create);

            serializer.Serialize(writer, this);
        }


        public void Add(GameEntry gameEntry)
        {
            this.GameEntries.Add(gameEntry);
        }

        public Statistics EvaluateStatistics ()
        {
            // TODO
            var statistics = new Statistics();
            return statistics; 
        }

        public sealed class GameEntry
        {
            public GameEntry() { }

            public GameEntry(DateTime started, TimeSpan duration, string word, int steps, bool isWon)
            {
                this.Started = started;
                this.Duration = duration;
                this.Word = word;
                this.Steps = steps;
                this.IsWon = isWon;
            }

            public DateTime Started { get; set; }

            public TimeSpan Duration { get; set; }

            public string Word { get; set; } = string.Empty;

            public int Steps { get; set; }

            public bool IsWon { get; set; }
        }

        public sealed class Statistics
        {
            public TimeSpan Duration { get; set; }

            public int Wins { get; set; }

            public int Losses { get; set; }

            public int WinRate { get; set; }

            public int Streak { get; set; }

            public int BestStreak { get; set; }

            public List<int> Histogram { get; set; } = new List<int> { 0, 0, 0, 0, 0 };
        }
    }
}
