namespace Parole.Model
{
    using Lyt.CoreMvvm;

    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Text;

    public sealed class Words : Singleton<Words>
    {
        private static readonly string bulgarianWordsFile = "bg_en-utf8.dat";
        private static readonly string bulgarianWordsFileFormat = "bgwords{0}.txt";

        private static readonly int wordsFileCount5 = 6;
        private static readonly int wordsFileCount6 = 2;
        private static readonly string wordsFileFormat = "parole{1}{0}.txt";
        private static readonly string resourcesFolder = "Resources";
        private static readonly string  commonWordsFileFormat = "comuni{0}.txt";

        private readonly HashSet<string> words;
        private readonly HashSet<string> commonWords;

        private Words()
        {
            this.words = new HashSet<string>(4096, StringComparer.InvariantCultureIgnoreCase);
            this.commonWords = new HashSet<string>(256, StringComparer.InvariantCultureIgnoreCase);
        }

        public void PreLoadBulgarian()
        {
            HashSet<char> letters = new HashSet<char>();
            HashSet<string> words5 = new HashSet<string>();
            HashSet<string> words6 = new HashSet<string>();
            string content = bulgarianWordsFile.LoadTextResource(resourcesFolder);
            string[] tokens = content.Split(
                new char[] { ' ', '\t', '\r', '\n', }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string token in tokens)
            {
                if (string.IsNullOrWhiteSpace(token))
                {
                    continue;
                }

                int position = token.IndexOf("^;");
                if ( position == -1)
                {
                    continue ;
                }

                string word = token.Substring(position+2);
                if ( word.Contains('-'))
                {
                    continue;
                }

                if ( word.Length == 5)
                {
                    _ = words5.Add(word);

                }
                else if (word.Length == 6)
                {
                    _ = words6.Add(word);
                }

                foreach(var letter in word)
                {
                    _= letters.Add(letter);
                }
            }

            var list = letters.ToList<char>();
            list.Sort();
            foreach (char letter in list)
            {
                Debug.Write(letter);
                Debug.Write(" ");
            }

            Debug.WriteLine(" " );
            Debug.WriteLine("Letters " + letters.Count);
            Debug.WriteLine("5: " + words5.Count);
            Debug.WriteLine("6: " + words6.Count);

            try
            {
                var sb5 = new StringBuilder();
                foreach (var word in words5)
                {
                    _ = sb5.AppendLine(word);
                }

                File.WriteAllText("bgwords5.txt", sb5.ToString());

                var sb6 = new StringBuilder();
                foreach (var word in words6)
                {
                    _ = sb6.AppendLine(word);
                }

                File.WriteAllText("bgwords6.txt", sb6.ToString());
            } 
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString()); 
            }
        }

        public void LoadBulgarian()
        {
            string wordsFile = string.Format(bulgarianWordsFileFormat, Word.Length);
            string content = wordsFile.LoadTextResource(resourcesFolder);
            string[] tokens = content.Split(
                new char[] { ' ', '\t', '\r', '\n', }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string token in tokens)
            {
                if (!string.IsNullOrWhiteSpace(token) && (token.Length == Word.Length))
                {
                    _ = this.words.Add(token);
                    _ = this.commonWords.Add(token);
                }
            }

            Debug.WriteLine("Word count: " + words.Count);
        }

        public void LoadItalian()
        {
            int fileCount = Word.Length == 5 ? wordsFileCount5 : wordsFileCount6;
            for (int i = 0; i < fileCount; i++)
            {
                string wordsFile = string.Format(wordsFileFormat, i, Word.Length);
                string content = wordsFile.LoadTextResource(resourcesFolder);
                string [] tokens = content.Split(
                    new char[] { ' ', '\t', '\r', '\n', }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string token in tokens)
                {
                    if (!string.IsNullOrWhiteSpace(token) && (token.Length == Word.Length))
                    {
                        if (!Words.HasNonItalianOrSpecialCharacters(token))
                        {
                            _ = this.words.Add(token);
                            // Debug.WriteLine(token);
                        }
                    }
                }

                Debug.WriteLine("Word count: " + words.Count);
            }

            string commonWordsFile = string.Format(commonWordsFileFormat, Word.Length);
            string commonContent = commonWordsFile.LoadTextResource(resourcesFolder);
            string[] commonTokens = commonContent.Split(
                new char[] { ' ', '\t', '\r', '\n', }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string token in commonTokens)
            {
                if (!string.IsNullOrWhiteSpace(token) && (token.Length == Word.Length))
                {
                    if ((!Words.HasNonItalianOrSpecialCharacters(token)) && this.words.Contains(token))
                    {
                        _ = this.commonWords.Add(token);
                        // Debug.WriteLine(token);
                    }
                }
            }

            Debug.WriteLine("Common Word count: " + this.commonWords.Count);
        }

        public bool IsPresent(Word word) => words.Contains(word.AsString());

        public string RandomPick(HashSet<string> exclude)
        {
            string common = Words.RandomPick(this.commonWords, exclude);
            bool foundInCommon = !string.IsNullOrWhiteSpace(common);
            Debug.WriteLine(foundInCommon ? "Found common word": "Found UNCOMMON word");
            return !foundInCommon ? Words.RandomPick(this.words, exclude) : common;
        }

        private static string RandomPick(HashSet<string> hash, HashSet<string> exclude)
        {
            long now = DateTime.Now.Ticks;
            int seed = (int)(now >> 4);
            var random = new Random(seed);
            bool found = false;
            int retries = 20 * exclude.Count;
            while (!found)
            {
                int choice = random.Next(hash.Count);
                int index = 0;
                foreach (string word in hash)
                {
                    if (index == choice)
                    {
                        if (!exclude.Contains(word))
                        {
                            return word;
                        }
                    }

                    ++index;
                }

                --retries;
                if (retries <= 0)
                {
                    break;
                }
            }

            return string.Empty;
        }

        private static bool HasNonItalianOrSpecialCharacters(string word)
        {
            foreach (char c in new char[] { 'j', 'k', 'w', 'x', 'y', '.', '\'', ',', ' ' })
            {
                if (word.IndexOf(c, StringComparison.InvariantCultureIgnoreCase) != -1)
                {
                    Debug.WriteLine("Excluded: " + word);
                    return true;
                }
            }

            return false;
        }
    }
}
