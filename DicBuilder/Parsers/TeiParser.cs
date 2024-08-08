
namespace Lyt.DicBuilder.Parsers;

public sealed class TeiParser : Parser
{
    private static XmlSerializer serializer;

    static TeiParser() => TeiParser.serializer = new XmlSerializer(typeof(Tei));

    public TeiParser(Language language) : base(language)
    {
    }

    public static async Task<Tei> Load(Language source, Language target, string file)
    {
        var parser = new TeiParser(target);
        var parsed = await parser.Load(file);
        parsed.Source = source;
        parsed.Target = target;
        return parsed;
    }

    private async Task<Tei> Load(string name)
    {
        Debug.WriteLine(name + " started");
        string folder = "FreeDict";
        string content = FileManager.Instance.LoadTextResource(folder, name);
        var stream = new StringReader(content);
        return serializer.Deserialize(stream) as Tei;
    }
}
