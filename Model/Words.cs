namespace Parole.Model
{
    using Lyt.CoreMvvm;

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;

    public sealed class Words : Singleton<Words>
    {
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

        public void Load()
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
