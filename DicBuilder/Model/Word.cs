namespace Lyt.DicBuilder.Model;

public enum Language
{
    Unknown,
    English,
    French,
    Italian,
    Spanish,
    German,
}

public enum Dialect
{
    Regular,
    British,
    American,
    Canadian,
    Austrian,
    Australian,
}

public enum Grammar
{
    Unknown,
    Noun,
    Verb,
    Adjective,
    Adverb,
    Article,
    Pronoun,
    Preposition,
    Conjunction,
    PastParticipe,
    PresentParticipe,
    Exclude,
    Number,
    Interjection,
}

public enum Gender
{
    Unknown,
    Masculine,
    Feminine,
    Neutral,
}

public enum Plurality
{
    Unknown,
    Singular,
    Plural,
}

public enum Theme
{
    Unknown,
    Time,
    Country,
    Language,
    Geography,
    Conversation,
    Abstract,
    People,
    Transport,
    Business,
    Material,
    Environment,
    Profession,
    Mathematics,
    House,
    Nature,
    City,
    Communication,
    Animal,
    Fruit,
    Anatomy,
    Society,
    Art,
    Sports,
    Family,
    Container,
    Clothes,
    Physics,
    Vegetable,
    Furniture,
    Drink,
    Tool,
    Science,
    Color,
    Object,
    Relations,
    Food,
    Device,
    Medicine,
    Education,
    Weather,
    Kitchen,
    Number,
    Feeling,
}

[Flags]
public enum Usage
{
    None = 0,
    Figurative = 1,
    Slang = 2,
    Colloqial = 4,
    Pejorative = 8,
    Vulgar = 16,
    Informal = 32,
    Familiar = 64,
    Formal = 128,
    Humour = 256,
    Archaic = 512,
    Dated = 1024,
}

public sealed class Word
{
    public Guid Id { get; set; }

    public string Text { get; set; }

    public List<string> Synonyms { get; set; }

    public string Abbreviation { get; set; }

    public string Context { get; set; }

    public string Pronunciation { get; set; }

    public string Sample { get; set; }

    public bool IsBasic { get; set; }

    public bool HasDoubledLetters { get; set; }

    public Language Language { get; set; }

    public Dialect Dialect { get; set; }

    public Grammar Grammar { get; set; }

    public Gender Gender { get; set; }

    public Plurality Plurality { get; set; }

    public Usage Usage { get; set; }

    public Theme Theme { get; set; }

    public override string ToString() 
        => string.Format("{0}  Gen: {1}  Gr: {2}", this.Text, this.Gender, this.Grammar);
}
