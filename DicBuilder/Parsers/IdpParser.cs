namespace Lyt.DicBuilder.Parsers;

#region File Format 

//  The dictionary files are lists of pairs of English and other-language words, in the form:
//
//     English-word Other-language-word
//     English-word Other-language-word
//     ...
//
//  where the pairs are separated by a tab character, and each pair is terminated by a carriage return character.
//  The "Other-language-word" is a translation of the English word(see the Disclaimer). 
//  If an English word has more than one translation into the other language, the English word is listed multiple times, 
//  each with a different "Other-language-word". For example, in Spanish:
//
//       dog perro
//       dog perra
//
//  Any comment lines in the file will begin with a pound sign("#"). The files are not sorted into any particular order.
//  Accent Marks
//  Because these files are given in plain ASCII format, accented characters usually are not given directly.
//  Certain conventions have been given to people adding to the IDP files for accented characters, and most of the time 
//  these conventions were followed.
//
//  Spanish/Italian Accents
//
//     A letter followed by a forward slash ("/") indicates that the letter should have an acute accent over it.
//     A letter followed by a backward slash("\") indicates that the letter should have a grave accent over it. 
//     A letter followed by a tilde ("~") indicates that the letter should have a tilde above it.
//
//  German Accents
//      A letter followed by a period (".") indicates that the letter should have an umlaut over it. 
//      (This is sometimes approximated by a vowel followed by an "e" instead.)
//      A letter followed by a caret("^") indicates that the letter should have a circonflex over it.
//     Two s characters in a row usually indicates the German double-s character ("B")

#endregion

public sealed class IdpParser : Parser
{
    #region Tables 

    private static readonly Dictionary<string, string> Accents = new Dictionary<string, string>
    {
        { "a/" , "á" },
        { "e/" , "é" },
        { "e'" , "é" },
        { "i/" , "í" },
        { "o/" , "ó" },
        { "u/" , "ú" },
        { "y/" , "ý" },
        { "a\\" , "à" },
        { "e\\" , "è" },
        { "i\\" , "ì" },
        { "o\\" , "ò" },
        { "u\\" , "ù" },
        { "n~" , "ñ" },
        { "o~" , "õ" },
        { "a." , "ä" },
        { "e." , "ë" },
        { "i." , "ï" },
        { "o." , "ö" },
        { "u." , "ü" },
        { "y." , "ÿ" },
        { "a^" , "â" },
        { "e^" , "ê" },
        { "i^" , "î" },
        { "o^" , "ô" },
        { "u^" , "û" },
        { "c," , "ç" },
    };

    private static readonly string MasculineKey = "(m)";
    private static readonly string MasculineKey1 = "[m]";
    private static readonly string MasculineKey2 = "(un)";
    private static readonly string MasculineKey3 = "(le)";
    private static readonly string MasculineKey4 = "(masc)";
    private static readonly string MasculineKey5 = "(masc.)";
    private static readonly string MasculineKey6 = "[masc]";
    private static readonly string MasculineKey7 = "[masc.]";
    private static readonly string FeminineKey = "(f)";
    private static readonly string FeminineKey1 = "[f]";
    private static readonly string FeminineKey2 = "(feminine)";
    private static readonly string FeminineKey3 = "(une)";
    private static readonly string FeminineKey4 = "(la)";

    private static readonly Dictionary<string, Gender> Genders = new Dictionary<string, Gender>
    {
        { MasculineKey , Gender.Masculine },
        { MasculineKey1 , Gender.Masculine },
        { MasculineKey2 , Gender.Masculine },
        { MasculineKey3 , Gender.Masculine },
        { MasculineKey4 , Gender.Masculine },
        { MasculineKey5 , Gender.Masculine },
        { MasculineKey6 , Gender.Masculine },
        { MasculineKey7 , Gender.Masculine },
        { FeminineKey , Gender.Feminine },
        { FeminineKey1 , Gender.Feminine },
        { FeminineKey2 , Gender.Feminine },
        { FeminineKey3 , Gender.Feminine },
        { FeminineKey4 , Gender.Feminine },
    };

    private static readonly string FrenchArticleKey1 = "un ";
    private static readonly string FrenchArticleKey2 = "le ";
    private static readonly string FrenchArticleKey3 = "une ";
    private static readonly string FrenchArticleKey4 = "la ";
    private static readonly string FrenchArticleKey5 = "l'";
    private static readonly string FrenchArticleKey6 = "des ";
    private static readonly string FrenchArticleKey7 = "les ";

    private static readonly Dictionary<string, Gender> FrenchArticlesGenders = new Dictionary<string, Gender>
    {
        { FrenchArticleKey1 , Gender.Masculine },
        { FrenchArticleKey2 , Gender.Masculine },
        { FrenchArticleKey3 , Gender.Feminine },
        { FrenchArticleKey4 , Gender.Feminine },
        { FrenchArticleKey5 , Gender.Unknown },
        { FrenchArticleKey6 , Gender.Unknown },
        { FrenchArticleKey7 , Gender.Unknown },
    };

    private static readonly Dictionary<string, Gender> GermanArticlesGenders = new Dictionary<string, Gender>
    {
        { "der " , Gender.Masculine },
        { "die ", Gender.Feminine},
        { "das " , Gender.Neutral},
        { "eine " , Gender.Feminine },
        { "ein " , Gender.Unknown },
    };

    private static readonly Dictionary<string, Gender> ItalianArticlesGenders = new Dictionary<string, Gender>
    {
        { "un " , Gender.Masculine },
        { "una " , Gender.Feminine },
        { "il " , Gender.Masculine },
        { "lo " , Gender.Masculine },
        { "la " , Gender.Feminine },
        { "le " , Gender.Feminine },
        { "i " , Gender.Masculine },
        { "gli ", Gender.Masculine },
    };

    private static readonly Dictionary<string, Gender> SpanishArticlesGenders = new Dictionary<string, Gender>
    {
        { "un " , Gender.Masculine },
        { "una " , Gender.Feminine },
        { "unos " , Gender.Masculine },
        { "unas " , Gender.Feminine },
        { "el " , Gender.Masculine },
        { "los " , Gender.Masculine },
        { "la " , Gender.Feminine },
        { "las " , Gender.Feminine },
    };

    private static readonly string UnknownKey = "[]";
    private static readonly string ArticleKey = "[Article]";
    private static readonly string AdverbKey = "[Adverb]";
    private static readonly string VerbKey = "[Verb]";
    private static readonly string VerbKey2 = "(verb)";
    private static readonly string NounKey = "[Noun]";
    private static readonly string NounKey2 = "(noun)";
    private static readonly string AdjectiveKey = "[Adjective]";
    private static readonly string PronounKey = "[Pronoun]";
    private static readonly string PrepositionKey = "[Preposition]";
    private static readonly string ConjunctionKey = "[Conjunction]";

    private static readonly Dictionary<string, Grammar> Grammars = new Dictionary<string, Grammar>
    {
        { UnknownKey , Grammar.Unknown },
        { ArticleKey , Grammar.Article },
        { AdverbKey , Grammar.Adverb },
        { VerbKey , Grammar.Verb },
        { VerbKey2 , Grammar.Verb },
        { NounKey , Grammar.Noun },
        { NounKey2 , Grammar.Noun },
        { PronounKey , Grammar.Pronoun },
        { AdjectiveKey , Grammar.Adjective},
        { PrepositionKey , Grammar.Preposition},
        { ConjunctionKey , Grammar.Conjunction},
    };

    #endregion Tables 

    public IdpParser(Language language) : base(language) { }

    public static async Task<WordTranslator> Load (Language target, string file)
    {
        var parser = new IdpParser(target);
        string[] lines = await parser.Load(file);
        return parser.Process(lines);
    }

    private async Task<string[]> Load(string name)
    {
        Debug.WriteLine("IDP Load: " + name + " started");
        string folder = "Idp";
        string content = FileManager.Instance.LoadTextResource(folder, name);
        string[] lines = content.Split(new string[] { "\n", "\r", "\n\r", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
        return lines;
    }

    private WordTranslator Process(string[] lines)
    {
        Debug.WriteLine("IDP - Process: " + this.Language.ToString());

        var pairs = new WordTranslator(Language.English, this.Language, 8000);
        int lineCount = 0;
        int pairCount = 0;
        foreach (string line in lines)
        {
            var list = this.ProcessLine(line);
            if (list == null)
            {
                continue;
            }

            foreach (var pair in list)
            {
                string english = pair.Item1;
                Word word = pair.Item2;
                if ((english == null) || (word == null))
                {
                    // commment line or failed
                    continue;
                }

                if ( !pairs.TryGetValue(english, out var entry))
                {
                    entry = new List<Word>();
                    pairs.Add(english, entry);
                }

                entry.Add(word);
                ++pairCount;
            }

            ++lineCount;
        }

        Debug.WriteLine(lineCount.ToString() + " lines processed");
        Debug.WriteLine(pairCount.ToString() + " words");
        Debug.WriteLine("Complete");
        return pairs;
    }

    private List<Tuple<string, Word>> ProcessLine(string line)
    {
        try
        {
            if (line.StartsWith('#'))
            {
                // comment line 
                return null;
            }

            string[] tokens = line.Split("\t", StringSplitOptions.RemoveEmptyEntries);
            if (tokens.GetLength(0) != 2)
            {
                return null;
            }

            string english = tokens[0];
            if (english.Length < 2)
            {
                return null;
            }

            string entriesAll = tokens[1];
            if (entriesAll.Length < 2)
            {
                return null;
            }

            entriesAll = this.ProcessAccents(entriesAll);

            var grammarTuple = this.ProcessGrammar(entriesAll);
            entriesAll = grammarTuple.Item1;
            Grammar grammar = grammarTuple.Item2;

            string[] entries = entriesAll.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
            var list = new List<Tuple<string, Word>>(entries.GetLength(0));
            foreach (string entryIterationVariable in entries)
            {
                string entry = entryIterationVariable;

                var genderTuple = this.ProcessGender(entry);
                entry = genderTuple.Item1;
                Gender gender = genderTuple.Item2;

                var genderTupleEx = this.ProcessArticles(entry);
                entry = genderTupleEx.Item1;
                if (gender == Gender.Unknown)
                {
                    gender = genderTupleEx.Item2;
                }

                var contextTuple = this.ProcessContext(entry);
                entry = contextTuple.Item1;
                string context = contextTuple.Item2;

                entry = entry.Replace("  ", " ");
                context = context.Replace("  ", " ");
                english = english.Replace("  ", " ");

                entry = entry.Trim();
                context = context.Trim();
                english = english.Trim();

                var word =
                    new Word
                    {
                        Id = Guid.NewGuid(),
                        Text = entry,
                        Gender = gender,
                        Grammar = grammar,
                        Context = context,
                    };

                //Debug.WriteLine(
                //    this.FixedLength(english, 26) +
                //    this.FixedLength(entry, 42) +
                //    this.FixedLength(grammar.ToString(), 16) +
                //    this.FixedLength(gender.ToString(), 16) +
                //    context);

                list.Add(new Tuple<string, Word>(english, word));
            }

            return list;
        }
        catch (Exception e)
        {
            Debug.WriteLine(e);
            if (Debugger.IsAttached) { Debugger.Break(); }
            return null;
        }
    }

    private string ProcessAccents(string entry)
    {
        //  Spanish/Italian Accents
        //
        //     A letter followed by a forward slash ("/") indicates that the letter should have an acute accent over it.
        //     A letter followed by a backward slash("\") indicates that the letter should have a grave accent over it. 
        //     A letter followed by a tilde ("~") indicates that the letter should have a tilde above it.
        //
        //  German Accents
        //      A letter followed by a period (".") indicates that the letter should have an umlaut over it. 
        //      (This is sometimes approximated by a vowel followed by an "e" instead.)
        //      A letter followed by a caret("^") indicates that the letter should have a circonflex over it.
        //     Two s characters in a row usually indicates the German double-s character ("B")

        foreach (var pair in Accents)
        {
            string key = pair.Key;
            string value = pair.Value;
            entry = entry.Replace(key, value);
        }

        return entry;
    }

    private (string, Grammar) ProcessGrammar(string entry) => this.ProcessEnum<Grammar>(Grammars, entry);

    private (string, Gender) ProcessGender(string entry) => this.ProcessEnum<Gender>(Genders, entry);

    private (string, Gender) ProcessArticles(string entry)
    {
        switch (this.Language)
        {
            case Language.French:
                return ProcessArticles(entry, FrenchArticlesGenders);
            case Language.Italian:
                return ProcessArticles(entry, ItalianArticlesGenders);
            case Language.Spanish:
                return ProcessArticles(entry, SpanishArticlesGenders);
            case Language.German:
                return ProcessArticles(entry, GermanArticlesGenders);
        }

        return (entry, Gender.Unknown);
    }

    private (string, Gender) ProcessArticles(string entry, Dictionary<string, Gender> dictionary)
    {
        Gender gender = Gender.Unknown;
        foreach (var pair in dictionary)
        {
            string key = pair.Key;
            Gender value = pair.Value;
            if (entry.StartsWith(key))
            {
                entry = entry.Replace(key, string.Empty);
                gender = value;
                break;
            }
        }

        entry = entry.Trim();
        return (entry, gender);
    }

    private (string, string) ProcessContext(string entry) => this.Extract(entry, '(', ')');
}
