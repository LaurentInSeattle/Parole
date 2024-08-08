namespace Lyt.DicBuilder.Model;

public sealed class WordTranslator : Dictionary<string, List<Word>>
{
    public WordTranslator(Language source, Language target, int capacity)
        : base(capacity, StringComparer.InvariantCultureIgnoreCase)
    {
        this.Source = source;
        this.Target = target;
    }

    public Language Source { get; }

    public Language Target { get; }

    public static WordTranslator Merge(
        WordTranslator one, WordTranslator two,
        Dictionary<string, Word> pronunciation, Dictionary<string, Word> grammar)
    {
        var merged = new WordTranslator(one.Source, one.Target, one.Count + two.Count);

        return merged;
    }

    public Dictionary<string, Word> ProcessTargetLanguage()
    {
        var target = new Dictionary<string, Word>(this.Count * 2);

        return target;
    }
}
