namespace Lyt.DicBuilder.Parsers;

[XmlRoot(ElementName = "gramGrp", Namespace = "http://www.tei-c.org/ns/1.0")]
public class GrammarGroup
{
    [XmlElement(ElementName = "pos", Namespace = "http://www.tei-c.org/ns/1.0")]
    public string Grammar { get; set; }

    [XmlElement(ElementName = "gen", Namespace = "http://www.tei-c.org/ns/1.0")]
    public string Gender { get; set; }

    [XmlElement(ElementName = "n", Namespace = "http://www.tei-c.org/ns/1.0")]
    public string Plurality { get; set; }
}

[XmlRoot(ElementName = "form", Namespace = "http://www.tei-c.org/ns/1.0")]
public class Form
{
    [XmlElement(ElementName = "orth", Namespace = "http://www.tei-c.org/ns/1.0")]
    public string Orthograph { get; set; }

    [XmlElement(ElementName = "pron", Namespace = "http://www.tei-c.org/ns/1.0")]
    public string Pronunciation { get; set; }
}

[XmlRoot(ElementName = "entry", Namespace = "http://www.tei-c.org/ns/1.0")]
public class Entry
{
    [XmlElement(ElementName = "form", Namespace = "http://www.tei-c.org/ns/1.0")]
    public Form Form { get; set; }

    [XmlElement(ElementName = "gramGrp", Namespace = "http://www.tei-c.org/ns/1.0")]
    public GrammarGroup GrammarGroup { get; set; }

    [XmlElement(ElementName = "sense", Namespace = "http://www.tei-c.org/ns/1.0")]
    public List<Sense> Senses { get; set; }
}

[XmlRoot(ElementName = "cit", Namespace = "http://www.tei-c.org/ns/1.0")]
public class Cit
{
    [XmlElement(ElementName = "quote", Namespace = "http://www.tei-c.org/ns/1.0")]
    public string Quote { get; set; }

    [XmlAttribute(AttributeName = "type")]
    public string Type { get; set; }
}

[XmlRoot(ElementName = "sense", Namespace = "http://www.tei-c.org/ns/1.0")]
public class Sense
{
    [XmlAttribute(AttributeName = "n")]
    public int Rank { get; set; }

    [XmlElement(ElementName = "cit", Namespace = "http://www.tei-c.org/ns/1.0")]
    public List<Cit> Cits { get; set; }
}

[XmlRoot(ElementName = "body", Namespace = "http://www.tei-c.org/ns/1.0")]
public class Body
{
    [XmlElement(ElementName = "entry", Namespace = "http://www.tei-c.org/ns/1.0")]
    public List<Entry> Entries { get; set; }
}

[XmlRoot(ElementName = "text", Namespace = "http://www.tei-c.org/ns/1.0")]
public class Text
{
    [XmlElement(ElementName = "body", Namespace = "http://www.tei-c.org/ns/1.0")]
    public Body Body { get; set; }
}

[XmlRoot(ElementName = "TEI", Namespace = "http://www.tei-c.org/ns/1.0")]
public partial class Tei
{
    [XmlElement(ElementName = "text", Namespace = "http://www.tei-c.org/ns/1.0")]
    public Text Text { get; set; }

    [XmlAttribute(AttributeName = "xmlns")]
    public string Xmlns { get; set; }
}
