namespace Parole.Model
{
    using Lyt.CoreMvvm;

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;

    public sealed class Words : Singleton<Words>
    {
        private static readonly string wordsFile0 = "parole5.txt";
        private static readonly string wordsFile1 = "parole51.txt";
        private static readonly string wordsFile2 = "parole52.txt";
        private static readonly string resourcesFolder = "Resources";

        private readonly HashSet<string> words;

        private Words()
        {
            this.words = new HashSet<string>(1024, StringComparer.InvariantCultureIgnoreCase);
        }

        public void Load()
        {
            string content = wordsFile0.LoadTextResource(resourcesFolder);
            string[] tokens = content.Split(new char[] { ' ', '\t', '\r', '\n', }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string token in tokens)
            {
                if (!string.IsNullOrWhiteSpace(token) && (token.Length == Word.Length))
                {
                    _ = words.Add(token);
                    // Debug.WriteLine(token);
                }
            }

            Debug.WriteLine("Word count: " + words.Count);

            content = wordsFile1.LoadTextResource(resourcesFolder);
            tokens = content.Split(new char[] { ' ', '\t', '\r', '\n', }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string token in tokens)
            {
                if (!string.IsNullOrWhiteSpace(token) && (token.Length == Word.Length))
                {
                    _ = words.Add(token);
                    // Debug.WriteLine(token);
                }
            }

            Debug.WriteLine("Word count: " + words.Count);

            content = wordsFile2.LoadTextResource(resourcesFolder);
            tokens = content.Split(new char[] { ' ', '\t', '\r', '\n', }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string token in tokens)
            {
                if (!string.IsNullOrWhiteSpace(token) && (token.Length == Word.Length))
                {
                    _ = words.Add(token);
                    // Debug.WriteLine(token);
                }
            }

            Debug.WriteLine("Word count: " + words.Count);
        }

        public bool IsPresent(Word word) => words.Contains(word.AsString());

        public string RandomPick ( HashSet<string> exclude )
        {
            long now = DateTime.Now.Ticks;
            int seed = (int)( now>> 4) ;
            var random = new Random(seed);
            bool found = false;

            int retries = 20 * exclude.Count;
            while ( !found)
            {
                int choice = random.Next(this.words.Count);
                int index = 0;
                foreach (string word in this.words)
                {
                    if( index == choice )
                    {
                        if( !exclude.Contains(word))
                        {
                            return word;    
                        }
                    }

                    ++index;
                }

                -- retries;
                if (retries == 0)
                {
                    break;
                } 
            }

            return string.Empty;
        }
    }
}
