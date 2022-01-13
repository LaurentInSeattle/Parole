namespace Parole.Model
{
    using Lyt.CoreMvvm;
    using Lyt.CoreMvvm.Extensions;

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
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
            try
            {
                string root = WpfExtensions.ApplicationDataFolder("LYT", "Parole"); 
                string path = Path.Combine(root, FileName);
                if (File.Exists(path))
                {
                    var serializer = new XmlSerializer(typeof(History));
                    if (serializer != null)
                    {
                        using var reader = new FileStream(path, FileMode.Open);
                        if (reader != null)
                        {
                            var history = serializer.Deserialize(reader) as History;
                            if (history != null && !history.GameEntries.IsNullOrEmpty())
                            {
                                this.GameEntries.AddRange(history.GameEntries);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        public void Save()
        {
            try
            {
                string root = WpfExtensions.ApplicationDataFolder("LYT", "Parole");
                string path = Path.Combine(root, FileName);
                var serializer = new XmlSerializer(this.GetType());
                using var writer = new FileStream(path, FileMode.Create);
                if ((writer != null) && (serializer != null))
                {
                    serializer.Serialize(writer, this);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        public void Add(GameEntry gameEntry)
        {
            this.GameEntries.Add(gameEntry);
        }

        public HashSet<string> PlayedWords()
        {
            var hashSet = new HashSet<string>(this.GameEntries.Count);
            foreach (var gameEntry in this.GameEntries)
            {
                _ = hashSet.Add(gameEntry.Word);
            }

            return hashSet;
        }

        public Statistics EvaluateStatistics()
        {
            var statistics = new Statistics();
            int wins = (from entry in this.GameEntries where entry.IsWon select entry).Count();
            int count = this.GameEntries.Count;
            statistics.Wins = wins;
            statistics.Losses = this.GameEntries.Count - wins;
            statistics.WinRate = count == 0 ? 0 : (int)((0.5f + (100 * wins)) / count);
            long durationLong = (from entry in this.GameEntries select entry.Duration.Ticks).Sum();
            TimeSpan durationTs = new(durationLong);
            statistics.Duration = durationTs;
            var streaks = this.CalculateStreaks();
            statistics.BestStreak = streaks.Item1;
            statistics.CurrentStreak = streaks.Item2;

            var list = new List<int> ();
            for (int i = 0; i < 6; i++)
            {
                int hist = 
                    (from entry in this.GameEntries where entry.IsWon && entry.Steps == i select entry).Count();
                list.Add (hist);   
            } 

            statistics.Histogram = list;

            return statistics;
        }

        private (int,int) CalculateStreaks()
        {
            int longestStreak = 0;
            int currentStreak = 0;
            foreach (var entry in this.GameEntries)
            {
                if (entry.IsWon)
                {
                    currentStreak++;
                }
                else
                {
                    if (currentStreak > longestStreak)
                    {
                        longestStreak = currentStreak;
                    } 

                    currentStreak = 0;
                }
            }
            
            if (currentStreak > longestStreak)
            {
                longestStreak = currentStreak;
            } 

            return (longestStreak,currentStreak);
        }
        public sealed class GameEntry
        {
            public GameEntry() { /* Req'ed for serialization */ }

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

            public int CurrentStreak { get; set; }

            public int BestStreak { get; set; }

            public List<int> Histogram { get; set; } = new List<int> { 0, 0, 0, 0, 0, 0};
        }
    }
}
