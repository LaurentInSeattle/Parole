
namespace Lyt.DicBuilder.Parsers;

public class Parser
{
    public Parser(Language language) => this.Language = language;

    public Language Language { get; private set; }

    protected string FixedLength(string word, int n)
    {
        int missing = n - word.Length;
        if (missing <= 0)
        {
            return word;
        }

        return string.Concat(word, new string(' ', missing));
    }

    protected (string, string) Extract(string entry, char startWith, char endWith)
    {
        int begin = entry.IndexOf(startWith);
        int end = entry.IndexOf(endWith);
        if ((begin == -1) || (end == -1) || (end <= begin))
        {
            return (entry, string.Empty);
        }

        bool hasContext = end - begin > 2;
        string context = string.Empty;
        string toTrim = entry.Substring(begin, 1 + end - begin);
        if (hasContext)
        {
            context = entry.Substring(begin + 1, end - begin - 1);
        }

        entry = entry.Replace(toTrim, string.Empty);
        entry = entry.Trim();

        return (entry, context);
    }

    protected (string, TEnum) ProcessEnum<TEnum>(Dictionary<string, TEnum> dictionary, string entry)
    {
        if (string.IsNullOrWhiteSpace(entry))
        {
            return (string.Empty, default(TEnum));
        }

        var enumValue = default(TEnum);
        foreach (var pair in dictionary)
        {
            string key = pair.Key;
            TEnum value = pair.Value;
            if (entry.IndexOf(key) != -1)
            {
                entry = entry.Replace(key, string.Empty);
                enumValue = value;
                break;
            }
        }

        entry = entry.Replace("  " , " ");
        entry = entry.Trim();
        return (entry, enumValue);
    }

    protected (string, TEnum) ProcessFlagEnum<TEnum>(Dictionary<string, TEnum> dictionary, string entry)
        where TEnum : Enum
    {
        if (string.IsNullOrWhiteSpace(entry))
        {
            return (string.Empty, default(TEnum));
        }

        int flagCount = 0; 
        var enumValue = default(TEnum);
        int enumValueInt = (int)(object)enumValue;
        foreach (var pair in dictionary)
        {
            string key = pair.Key;
            TEnum value = pair.Value;
            if (entry.IndexOf(key) != -1)
            {
                // Debug.WriteLine(entry + "   --->   " + key);
                entry = entry.Replace(key, string.Empty);
                int valueInt = (int)(object)value;
                enumValueInt |= valueInt ;

                ++flagCount;
            }
        }

        enumValue = (TEnum) Enum.ToObject(typeof(TEnum), enumValueInt);

        entry = entry.Replace("  ", " ");
        entry = entry.Trim();
        return (entry, enumValue);
    }
}
