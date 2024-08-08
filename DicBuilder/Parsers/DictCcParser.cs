namespace Lyt.DicBuilder.Parsers;

public sealed class DictCcParser : Parser 
{
    public DictCcParser(Language language) : base(language) { }

    public static async Task<WordTranslator> Load(Language language, string file)
    {
        Debug.WriteLine("DictCC - Load: " + language.ToString());

        var parser = new DictCcParser(language);
        string[] strings = await parser.Load(file);
        var dictionary = parser.Process(strings);
        return dictionary;
    }

    #region Tables 

    private static readonly string MasculineKey = "{m}";
    private static readonly string MasculineKey1 = "{m.pl}";
    private static readonly string FeminineKey = "{f}";
    private static readonly string FeminineKey1 = "{f.pl}";

    private static readonly Dictionary<string, Gender> Genders = new Dictionary<string, Gender>
    {
        { MasculineKey , Gender.Masculine },
        { MasculineKey1 , Gender.Masculine },
        { "[male]" , Gender.Masculine },
        { "[female]" , Gender.Feminine},
        { FeminineKey , Gender.Feminine },
        { FeminineKey1 , Gender.Feminine },
    };

    private static readonly string NounKey = "noun";
    private static readonly string NounKey1 = "Noun";
    private static readonly string AdjectiveKey = "adj";
    private static readonly string VerbKey = "verb";
    private static readonly string ArticleKey = "article";
    private static readonly string AdverbKey = "adv";
    private static readonly string PronounKey = "pron";
    private static readonly string UnknownKey = "[none]";
    private static readonly string UnknownKey1 = "[none][none]";
    private static readonly string UnknownKey2 = "name";
    private static readonly string PrepositionKey = "prep";
    private static readonly string ConjunctionKey = "conj";
    private static readonly string PastPKey = "past-p";
    private static readonly string PresPKey = "pres-p";
    private static readonly string PrefixKey = "prefix";
    private static readonly string SuffixKey = "suffix";
    private static readonly string ExpressionKey = "expression";

    private static readonly Dictionary<string, Grammar> Grammars = new Dictionary<string, Grammar>
    {
        { NounKey , Grammar.Noun },
        { NounKey1 , Grammar.Noun },
        { UnknownKey , Grammar.Unknown },
        { UnknownKey1 , Grammar.Unknown },
        { UnknownKey2 , Grammar.Unknown },
        { AdjectiveKey , Grammar.Adjective},
        { ArticleKey , Grammar.Article },
        { AdverbKey , Grammar.Adverb },
        { VerbKey , Grammar.Verb },
        { PronounKey , Grammar.Pronoun },
        { PrepositionKey , Grammar.Preposition},
        { ConjunctionKey , Grammar.Conjunction},
        { PastPKey , Grammar.PastParticipe},
        { PresPKey, Grammar.PresentParticipe},
        { PrefixKey , Grammar.Exclude},
        { SuffixKey, Grammar.Exclude},
        { ExpressionKey, Grammar.Exclude},
    };

    private static readonly Dictionary<string, Usage> Usages = new Dictionary<string, Usage>
    {
        { "[sl.]", Usage.Slang},
        { "[coll.]", Usage.Colloqial},
        { "[fam.]", Usage.Familiar},
        { "[inf.]", Usage.Informal},
        { "[formal]", Usage.Formal},
        { "[fig.]", Usage.Figurative},
        { "[also fig.]", Usage.Figurative},
        { "[pej.]", Usage.Pejorative},
        { "[vulg.]", Usage.Vulgar},
        { "[hum.]", Usage.Humour},
        { "[dated]", Usage.Dated},
        { "[arch.]", Usage.Archaic},
    };

    private static readonly Dictionary<string, Plurality> Pluralities = new Dictionary<string, Plurality>
    {
        { "[sg]", Plurality.Singular},
        { "[sg.]", Plurality.Singular},
        { "{sg}", Plurality.Singular},
        { "[no pl.]", Plurality.Singular},

        { "[m.pl.]", Plurality.Plural},
        { "[f.pl.]", Plurality.Plural},
        { "[pl]", Plurality.Plural},
        { "[pl.]", Plurality.Plural},
        { "{pl}", Plurality.Plural},
    };

    private static readonly Dictionary<string, Dialect> Dialects = new Dictionary<string, Dialect>
    {
        { "[Am.]", Dialect.American},
        { "[esp. Am.]", Dialect.American},
        { "[Br.]", Dialect.British},
        { "[esp. Br.]", Dialect.British},
        { "[can.]", Dialect.Canadian},
        { "[Can.]", Dialect.Canadian},
        { "[Aus.]", Dialect.Australian},
    };

    #endregion Tables 

    private async Task<string[]> Load(string name)
    {
        Debug.WriteLine(name + " started");
        string folder  = "DictCC";
        string content = FileManager.Instance.LoadTextResource(folder, name);
        string[] lines = content.Split(new string[] { "\n", "\r", "\n\r", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
        return lines;
    }

    private WordTranslator Process(string[] lines)
    {
        Debug.WriteLine("DictCC - Process: " + this.Language.ToString());

        var pairs = new WordTranslator(Language.English, this.Language, 8000);
        int lineCount = 0;
        int pairCount = 0;
        foreach (string line in lines)
        {
            ++lineCount;

            var pair = this.ProcessLine(line);
            if (pair == null)
            {
                continue;
            }

            string english = pair.Item1;
            Word word = pair.Item2;
            if ((english == null) || (word == null))
            {
                // commment line or failed
                continue;
            }

            if (!pairs.TryGetValue(english, out var entry))
            {
                entry = new List<Word>();
                pairs.Add(english, entry);
            }

            entry.Add(word);

            ++pairCount;
        }

        Debug.WriteLine(lineCount.ToString() + " lines processed");
        Debug.WriteLine(pairCount.ToString() + " words");
        Debug.WriteLine("Complete");
        Debug.WriteLine("");
        Debug.WriteLine("------");
        Debug.WriteLine("");
        return pairs;
    }

    private Tuple<string, Word> ProcessLine(string line)
    {
        try
        {
            if (line.StartsWith('#'))
            {
                // comment line 
                return null;
            }

            if ((line.IndexOf('®')!= -1 ) || (line.IndexOf('™') != -1)) 
            {
                return null; 
            }

            string[] tokens = line.Split("\t", StringSplitOptions.RemoveEmptyEntries);
            if (tokens.GetLength(0) != 3)
            {
                return null;
            }

            string english = tokens[0];
            if (english.Length < 2)
            {
                return null;
            }

            string entry = tokens[1];
            if (entry.Length < 2)
            {
                return null;
            }

            string grammarString = tokens[2];
            if (grammarString.Length < 2)
            {
                grammarString = string.Empty;
            }

            var grammarTuple = this.ProcessGrammar(grammarString);
            Grammar grammar = grammarTuple.Item2;
            if ( grammar == Grammar.Exclude)
            {
                return null;
            }

            var genderTuple = this.ProcessGender(entry);
            entry = genderTuple.Item1;
            Gender gender = genderTuple.Item2;

            var abbrevTuple = this.ProcessContext(entry);
            entry = abbrevTuple.Item1;
            string abbrev = abbrevTuple.Item2;

            var usageTuple = this.ProcessUsage(entry);
            entry = usageTuple.Item1;
            var usage = usageTuple.Item2;

            var dialectTuple = this.ProcessDialect(entry);
            entry = dialectTuple.Item1;
            var dialect = dialectTuple.Item2;

            var contextTuple = this.ProcessContext(entry);
            entry = contextTuple.Item1;
            string context = contextTuple.Item2;

            dialectTuple = this.ProcessDialect(english);
            english = dialectTuple.Item1;
            var englishDialect = dialectTuple.Item2;

            genderTuple = this.ProcessGender(english);
            english = genderTuple.Item1;
            var englishGender = genderTuple.Item2;

            usageTuple = this.ProcessUsage(english);
            english = usageTuple.Item1;
            var englishUsage = usageTuple.Item2;

            contextTuple = this.ProcessContext(english);
            english = contextTuple.Item1;
            string englishContext = contextTuple.Item2;
            if (!string.IsNullOrWhiteSpace(englishContext))
            {
                // Debug.WriteLine(englishContext);
            }

            entry = entry.Replace("  ", " ");
            context = context.Replace("  ", " ");
            english = english.Replace("  ", " ");

            entry = entry.Trim();
            context = context.Trim();
            english = english.Trim();

            if ( grammar == Grammar.Verb)
            {
                if ( english.StartsWith ( "to ") )
                {
                    // sb./sth. etc..
                    english = english.Replace("to ", "");
                    english = english.Replace("sb./sth.", "");
                    english = english.Replace("sb.", "");
                    english = english.Replace("sth.", "");
                    english = english.Replace("()", "");
                    english = english.Replace("(", "");
                    english = english.Replace(")", "");
                    english = english.Replace("/", "");
                    english = english.Replace("  ", " ");

                    if (this.Language == Language.French)
                    {
                        entry = entry.Replace("(qn./qc.)", "");
                        entry = entry.Replace("(qn.)", "");
                        entry = entry.Replace("(qc.)", "");
                        entry = entry.Replace("qn./qc.", "");
                        entry = entry.Replace("qn.", "");
                        entry = entry.Replace("qc.", "");
                        entry = entry.Replace("a qn.", "");
                    }
                    else if (this.Language == Language.Italian)
                    {
                        entry = entry.Replace("qn./qc.", "");
                        entry = entry.Replace("qn.", "");
                        entry = entry.Replace("qc.", "");
                        entry = entry.Replace("a qn.", "");
                        entry = entry.Replace("con qn.", "");
                        entry = entry.Replace("di qc.", "");
                    }
                    else if (this.Language == Language.Spanish)
                    {
                        entry = entry.Replace("(a-algn/algo)", "");
                        entry = entry.Replace("(algo)", "");
                        entry = entry.Replace("(a-algn)", "");
                        entry = entry.Replace("a-algn/algo", "");
                        entry = entry.Replace("algo", "");
                        entry = entry.Replace("a-algn", "");
                    }
                    else if (this.Language == Language.German)
                    {
                        // TODO
                    }

                    entry = entry.Replace("()", "");
                    entry = entry.Replace("(", "");
                    entry = entry.Replace(")", "");
                    entry = entry.Replace("/", "");
                    entry = entry.Replace("  ", " ");

                    //Debug.WriteLine(
                    //    this.FixedLength(english, 30) +
                    //    this.FixedLength(entry, 42) +
                    //    this.FixedLength(grammar.ToString(), 16) +
                    //    this.FixedLength(gender.ToString(), 16) +
                    //    context);
                }
            }

            string[] englishTokens = english.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if ( englishTokens.GetLength(0) > 1 )
            {
                // Debug.WriteLine(english);
                return null;
            }

            var word =
                new Word
                {
                    Id = Guid.NewGuid(),
                    Language = this.Language,
                    Dialect = dialect,
                    Text = entry,
                    Gender = gender,
                    Grammar = grammar,
                    Context = context,
                    Abbreviation = abbrev,
                    Usage = usage,
                };

            if (usage != Usage.None)
            {
                //Debug.WriteLine(
                //    this.FixedLength(english, 30) +
                //    this.FixedLength(entry, 42) +
                //    this.FixedLength(grammar.ToString(), 16) +
                //    this.FixedLength(gender.ToString(), 16) +
                //    this.FixedLength(usage.ToString(), 26) +
                //    context);
            }

            return new Tuple<string, Word>(english, word);
        }
        catch (Exception e)
        {
            Debug.WriteLine(e);
            if (Debugger.IsAttached) { Debugger.Break(); }
            return null;
        }
    }

    private (string, Gender) ProcessGender(string entry) => this.ProcessEnum<Gender>(Genders, entry);

    private (string, Grammar) ProcessGrammar(string entry) => this.ProcessEnum<Grammar>(Grammars, entry);

    private (string, Usage) ProcessUsage(string entry) => this.ProcessFlagEnum<Usage>(Usages, entry);

    private (string, Dialect) ProcessDialect(string entry) => this.ProcessEnum<Dialect>(Dialects, entry);

    private (string, string) ProcessContext(string entry) => this.Extract ( entry, '[', ']');

    private (string, string) ProcessAbbreviation(string entry) => this.Extract(entry, '<', '>');
}
