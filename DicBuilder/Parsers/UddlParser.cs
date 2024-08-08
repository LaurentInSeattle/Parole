namespace Lyt.DicBuilder.Parsers;

public sealed class UddlParser : Parser
{
    public UddlParser(Language language) : base(language) => tags = new HashSet<string>();

    public static async Task<WordTranslator> Load(Language language, string file)
    {
        var parser = new UddlParser(language);
        string[] strings = await parser.Load(file);
        var dictionary = parser.Process(strings);
        return dictionary;
    }

    private async Task<string[]> Load(string name)
    {
        Debug.WriteLine(name + " started");
        string folder = "Uddl";
        string content = FileManager.Instance.LoadTextResource(folder, name);
        string[] lines = content.Split(new string[] { "\n", "\r", "\n\r", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
        return lines;
    }

    #region Tables 

    private const string Basic = "basic";

    private static readonly Dictionary<string, Grammar> Grammars = new Dictionary<string, Grammar>
    {
        { "adjective" , Grammar.Adjective},
        { "article" , Grammar.Article },
        { "adverb" , Grammar.Adverb },
        { "verb" , Grammar.Verb },
        { "pronoun" , Grammar.Pronoun },
        { "preposition" , Grammar.Preposition},
        { "conjunction" , Grammar.Conjunction},
    };

    private static readonly Dictionary<string, Theme> Themes = new Dictionary<string, Theme>
    {
        { "time"            , Theme.Time           } ,
        { "country"            , Theme.Country        } ,
        { "language"            , Theme.Language       } ,
        { "geography"            , Theme.Geography      } ,
        { "conversation"            , Theme.Conversation   } ,
        { "abstract"            , Theme.Abstract       } ,
        { "people"            , Theme.People         } ,
        { "transport"            , Theme.Transport      } ,
        { "business"            , Theme.Business       } ,
        { "material"            , Theme.Material       } ,
        { "environment"            , Theme.Environment    } ,
        { "profession"            , Theme.Profession     } ,
        { "mathematics"            , Theme.Mathematics    } ,
        { "house"            , Theme.House          } ,
        { "nature"            , Theme.Nature         } ,
        { "city"            , Theme.City           } ,
        { "communication"            , Theme.Communication  } ,
        { "animal"            , Theme.Animal         } ,
        { "fruit"            , Theme.Fruit          } ,
        { "anatomy"            , Theme.Anatomy        } ,
        { "society"            , Theme.Society        } ,
        { "art"            , Theme.Art            } ,
        { "sports"            , Theme.Sports         } ,
        { "family"            , Theme.Family         } ,
        { "container"            , Theme.Container      } ,
        { "clothes"            , Theme.Clothes        } ,
        { "physics"            , Theme.Physics        } ,
        { "vegetable"            , Theme.Vegetable      } ,
        { "furniture"            , Theme.Furniture      } ,
        { "drink"            , Theme.Drink          } ,
        { "tool"            , Theme.Tool           } ,
        { "science"            , Theme.Science        } ,
        { "color"            , Theme.Color          } ,
        { "object"            , Theme.Object         } ,
        { "relations"            , Theme.Relations      } ,
        { "food"            , Theme.Food           } ,
        { "device"            , Theme.Device         } ,
        { "medicine"            , Theme.Medicine       } ,
        { "education"            , Theme.Education      } ,
        { "weather"            , Theme.Weather        } ,
        { "kitchen"            , Theme.Kitchen        } ,
        { "number"            , Theme.Number         } ,
        { "feeling"            , Theme.Feeling        } ,
    };

    #endregion Tables 

    private HashSet<string> tags;

    private WordTranslator Process(string[] lines)
    {
        Debug.WriteLine("UDDL - Process: " + this.Language.ToString());

        var translator = new WordTranslator(Language.English, this.Language, 8000);
        this.tags = new HashSet<string>();
        int lineCount = 0;
        int pairCount = 0;
        int basicCount = 0;
        int themedCount = 0;
        foreach (string line in lines)
        {
            ++lineCount;

            var pairs = this.ProcessLine(line);
            if (pairs == null)
            {
                continue;
            }

            bool wordCounted = false;
            foreach (var pair in pairs)
            {
                string english = pair.Item1;
                Word word = pair.Item2;
                if ((english == null) || (word == null))
                {
                    // commment line or failed
                    continue;
                }

                if (!translator.TryGetValue(english, out var entry))
                {
                    entry = new List<Word>();
                    translator.Add(english, entry);
                }

                entry.Add(word);

                if (!wordCounted)
                {
                    wordCounted = true;
                    if (word.IsBasic)
                    {
                        ++basicCount;
                    }

                    if (word.Theme != Theme.Unknown)
                    {
                        ++themedCount;
                    }
                }

                ++pairCount;
            }
        }

        Debug.WriteLine(lineCount.ToString() + " lines processed");
        Debug.WriteLine(translator.Keys.Count.ToString() + " words");
        Debug.WriteLine(basicCount.ToString() + " BASIC words");
        Debug.WriteLine(themedCount.ToString() + " THEMED words");
        Debug.WriteLine("Complete");

        Debug.WriteLine("");
        Debug.WriteLine("");

        return translator;
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
            if (tokens.GetLength(0) != 3)
            {
                return null;
            }

            string englishAll = tokens[0];
            if (englishAll.Length < 2)
            {
                return null;
            }

            string[] englishTokens = englishAll.Split(";", StringSplitOptions.RemoveEmptyEntries);
            string english = englishTokens[0].Trim();
            int count = englishTokens.GetLength(0);
            List<string> englishSynonyms = null;
            if (count > 1)
            {
                englishSynonyms = new List<string>(count - 1);
                for (int i = 1; i < count; ++i)
                {
                    englishSynonyms.Add(englishTokens[i].Trim());
                }
            }

            string entryAll = tokens[1];
            string[] entryTokens = entryAll.Split(";", StringSplitOptions.RemoveEmptyEntries);
            string entry = entryTokens[0].Trim();
            if (entry.Length < 2)
            {
                return null;
            }

            count = entryTokens.GetLength(0);
            List<string> entrySynonyms = null;
            if (count > 1)
            {
                entrySynonyms = new List<string>(count - 1);
                for (int i = 1; i < count; ++i)
                {
                    entrySynonyms.Add(entryTokens[i].Trim());
                }
            }

            string grammarString = tokens[2];
            if (grammarString.Length < 2)
            {
                grammarString = string.Empty;
            }

            // this.tags.Add(grammarString);

            bool isBasic = false;
            Grammar grammar = Grammar.Unknown;
            Theme theme = Theme.Unknown;
            string[] keys = line.Split("-", StringSplitOptions.RemoveEmptyEntries);
            foreach (string key in keys)
            {
                if (key == Basic)
                {
                    isBasic = true;
                }
                else
                {
                    var grammarTuple = this.ProcessGrammar(key);
                    grammar = grammarTuple.Item2;

                    var themeTuple = this.ProcessTheme(key);
                    theme = themeTuple.Item2;
                }
            }

            entry = entry.Replace("  ", " ");
            english = english.Replace("  ", " ");

            entry = entry.Trim();
            english = english.Trim();

            if (grammar == Grammar.Unknown)
            {
                if ((theme != Theme.Conversation) &&
                 (theme != Theme.Country) &&
                 (theme != Theme.Language))
                {
                    string[] words = entry.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                    if (words.GetLength(0) == 1)
                    {
                        grammar = Grammar.Noun;
                    }
                }
            }

            var word =
                new Word
                {
                    Id = Guid.NewGuid(),
                    Language = this.Language,
                    Text = entry,
                    Synonyms = entrySynonyms,
                    IsBasic = isBasic,
                    Grammar = grammar,
                    Theme = theme,
                };

            if (grammar == Grammar.Noun)
            {
                //Debug.WriteLine(
                //    this.FixedLength(english, 26) +
                //    this.FixedLength(entry, 42) +
                //    this.FixedLength(grammar.ToString(), 16) +
                //    this.FixedLength(theme.ToString(), 16) +
                //    ( isBasic ? "Basic " : "      " )+
                //    (entrySynonyms?.Count > 0 ? " Syn: " + entrySynonyms?.Count.ToString() : "" ) );
            }

            int synCount = englishSynonyms?.Count ?? 0;
            var list = new List<Tuple<string, Word>>(1 + synCount)
            {
                new Tuple<string, Word>(english, word)
            };
            if ( englishSynonyms != null )
            {
                foreach (string syn in englishSynonyms)
                {
                    list.Add(new Tuple<string, Word>(syn, word));
                }
            }

            return list ;
        }
        catch (Exception e)
        {
            Debug.WriteLine(e);
            if (Debugger.IsAttached) { Debugger.Break(); }
            return null;
        }
    }

    private (string, Theme) ProcessTheme(string entry) => this.ProcessEnum<Theme>(Themes, entry);

    private (string, Grammar) ProcessGrammar(string entry) => this.ProcessEnum<Grammar>(Grammars, entry);
}
