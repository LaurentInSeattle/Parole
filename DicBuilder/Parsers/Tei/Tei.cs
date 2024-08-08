namespace Lyt.DicBuilder.Parsers;

public partial class Tei
{
    [XmlIgnore]
    public Language Source { get; set; }

    [XmlIgnore]
    public Language Target { get; set; }

    public Dictionary<string, Word> ProcessSourceLanguage()
    {
        Debug.WriteLine("Tei - ProcessSourceLanguage: " + this.Source.ToString());

        var entries = this.Text.Body.Entries;
        int count = entries.Count;
        var dict = new Dictionary<string, Word>(count, StringComparer.InvariantCultureIgnoreCase);
        foreach (var entry in entries)
        {
            var form = entry.Form;
            string text = form.Orthograph;
            int position = text.IndexOf('-', 0);
            if (position == 0)
            {
                continue;
            }

            string[] tokens = text.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (tokens.GetLength(0) > 1)
            {
                continue;
            }

            //if (entry.Senses.Count > 10)
            //{
            //    continue;
            //}

            var word = new Word()
            {
                Text = text,
                Pronunciation = form.Pronunciation,
                Language = this.Source,
            };

            var grammar = entry.GrammarGroup;
            if (grammar != null)
            {
                string gender = grammar.Gender;
                if (!string.IsNullOrWhiteSpace(gender))
                {
                    gender = gender.Trim();
                    if (gender == "m")
                    {
                        word.Gender = Gender.Masculine;
                    }
                    else if (gender == "f")
                    {
                        word.Gender = Gender.Feminine;
                    }
                    else if (gender == "masc")
                    {
                        word.Gender = Gender.Masculine;
                    }
                    else if (gender == "fem")
                    {
                        word.Gender = Gender.Feminine;
                    }
                    else if (gender == "n")
                    {
                        word.Gender = Gender.Neutral;
                    }
                    else
                    {
                        Debug.WriteLine("Unknown gender key: " + gender);
                        word.Gender = Gender.Unknown;
                    }
                }

                string grm = grammar.Grammar;
                if (!string.IsNullOrWhiteSpace(grm))
                {
                    grm = grm.Trim();
                    if (grm == "n")
                    {
                        word.Grammar = Grammar.Noun;
                    }
                    else if (grm == "vt")
                    {
                        word.Grammar = Grammar.Verb;
                    }
                    else if (grm == "vi")
                    {
                        word.Grammar = Grammar.Verb;
                    }
                    else if (grm == "v")
                    {
                        word.Grammar = Grammar.Verb;
                    }
                    else if (grm == "adj")
                    {
                        word.Grammar = Grammar.Adjective;
                    }
                    else if (grm == "adv")
                    {
                        word.Grammar = Grammar.Adverb;
                    }
                    else if (grm == "prep")
                    {
                        word.Grammar = Grammar.Preposition;
                    }
                    else if (grm == "pron")
                    {
                        word.Grammar = Grammar.Pronoun;
                    }
                    else if (grm == "conj")
                    {
                        word.Grammar = Grammar.Conjunction;
                    }
                    else if (grm == "num")
                    {
                        word.Grammar = Grammar.Number;
                    }
                    else if (grm == "int")
                    {
                        word.Grammar = Grammar.Interjection;
                    }
                    else if (grm == "art")
                    {
                        word.Grammar = Grammar.Article;
                    }
                    else
                    {
                        Debug.WriteLine("Unknown grammar key: " + grm);
                        word.Grammar = Grammar.Unknown;
                    }
                }

                string plural = grammar.Plurality;
                if (!string.IsNullOrWhiteSpace(plural))
                {
                    plural = plural.Trim();
                    Debug.WriteLine("Unknown plurality key: " + plural);
                }
            }

            text = text.Trim().ToLowerInvariant();
            bool added = dict.TryAdd(text, word);
        }

        Debug.WriteLine("Count: " + dict.Count);
        return dict;
    }

    public StringTranslator Bridge(StringTranslator dstDict)
    {
        if ( this.Target != dstDict.Source)
        {
            throw new InvalidOperationException("Cant bridge"); 
        }

        Debug.WriteLine("StringTranslator - Bridge: " + this.Source.ToString() + " - " + this.Target.ToString());

        var src = this.Text.Body.Entries;
        var bridge = new StringTranslator (this.Source , dstDict.Target, src.Count);
        foreach (var entry in src)
        {
            string key = entry.Form.Orthograph;
            List<string> values = null;
            foreach (var sense in entry.Senses)
            {
                foreach (var cit in sense.Cits)
                {
                    if (cit.Type == "trans")
                    {
                        string dstKey = cit.Quote;
                        if (dstDict.TryGetValue(dstKey, out var list))
                        {
                            if (values == null)
                            {
                                values = new List<string>(4);
                            }

                            values.AddRange(list);
                        }
                    }
                }
            }

            if (values != null)
            {
                if (bridge.TryGetValue(key, out var list))
                {
                    list.AddRange(values);
                }
                else
                {
                    bridge.Add(key, values);
                }
            }
        }

        var changed = new Dictionary<string, List<string>>(src.Count);
        foreach (var kvp in bridge)
        {
            var list = kvp.Value;
            for (int i = 0; i < list.Count; ++i)
            {
                list[i] = list[i].Trim().ToLowerInvariant();
            }

            var newList = list.Distinct().ToList();
            if (list.Count != newList.Count)
            {
                changed.Add(kvp.Key, newList);
            }
        }

        foreach (var kvp in changed)
        {
            string key = kvp.Key;
            if (bridge.TryGetValue(key, out var list))
            {
                bridge[key] = kvp.Value;
            }
        }

        bridge.Statistics();
        return bridge;
    }

    public StringTranslator ToStringTranslator() 
    {
        var entries = this.Text.Body.Entries;
        var dict = new StringTranslator(this.Source, this.Target, entries.Count);
        foreach (var entry in entries)
        {
            string key = entry.Form.Orthograph;
            List<string> values = null;
            foreach (var sense in entry.Senses)
            {
                foreach (var cit in sense.Cits)
                {
                    if (cit.Type == "trans")
                    {
                        string value = cit.Quote;
                        if (values == null)
                        {
                            values = new List<string>(4);
                        }

                        values.Add(value);
                    }
                }
            }

            if (values != null)
            {
                if (dict.TryGetValue(key, out var list))
                {
                    list.AddRange(values);
                }
                else
                {
                    dict.Add(key, values);
                }
            }
        }

        return dict;
    }
}
