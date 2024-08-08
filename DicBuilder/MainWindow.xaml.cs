using Mod = Lyt.DicBuilder.Model; 

namespace DicBuilder;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private async void OnButtonClick(object sender, RoutedEventArgs e)
    {
        GC.Collect();

        //var tei = await ParseTei(Model.Language.French, "fra-eng");
        //var tei_eng = await ParseTei(Model.Language.French, "eng-fra");
        //var idp = await this.ParseIdp(Model.Language.French, "FRENCH");
        //var udd = await this.ParseUddl(Model.Language.French, "english-french");
        //var dcc = await this.ParseDictCc(Model.Language.French, "dictcc-en-fr");
        //this.Statistics("French" , tei, tei_eng, idp, udd, dcc);

        // FRA <-> ITA 
        //var tei_fra_eng = await TeiParser.Load(Model.Language.French, Model.Language.English, "fra-eng");
        //var tei_eng_ita = await TeiParser.Load(Model.Language.English, Model.Language.Italian, "eng-ita");
        //var eng_ita = tei_eng_ita.ToTranslator();
        //var fra_ita = tei_fra_eng.Bridge(eng_ita);

        //var tei_ita_eng = await TeiParser.Load(Model.Language.Italian, Model.Language.English, "ita-eng");
        //var tei_eng_fra = await TeiParser.Load(Model.Language.English, Model.Language.French, "eng-fra");
        //var eng_fra = tei_eng_fra.ToTranslator();
        //var ita_fra = tei_ita_eng.Bridge(eng_fra);

        //fra_ita.Consolidate(ita_fra);
        //ita_fra.Consolidate(fra_ita);
        //fra_ita.CompareSingles(ita_fra);

        // ENG <-> FRA 



        // ITA <-> ENG 
        var idp = await IdpParser.Load(Mod.Language.Italian, "ITALIAN");
        var udd = await UddlParser.Load(Mod.Language.Italian, "english-italian");
        var dcc = await DictCcParser.Load(Mod.Language.Italian, "dictcc-en-it");




        //var ita_eng = tei_ita_eng.ToTranslator();
        //ita_eng.Consolidate(eng_ita);
        //eng_ita.Consolidate(ita_eng);
        //ita_eng.CompareSingles(eng_ita);
    }

    #region Commented OUT 
    //private async void OnButtonItalianClick(object sender, RoutedEventArgs e)
    //{
    //    GC.Collect();
    //    //var tei = await ParseTei(Model.Language.Italian, "ita-eng");
    //    //var tei_eng = await ParseTei(Model.Language.Italian, "eng-ita");
    //    //var idp = await this.ParseIdp(Model.Language.Italian, "ITALIAN");
    //    //var udd = await this.ParseUddl(Model.Language.Italian, "english-italian");
    //    //var dcc = await this.ParseDictCc(Model.Language.Italian, "dictcc-en-it");
    //    //this.Statistics("Italian", tei, tei_eng, idp, udd, dcc);
    //}

    //private async void OnButtonGermanClick(object sender, RoutedEventArgs e)
    //{
    //    GC.Collect();
    //    //var tei = await ParseTei(Model.Language.German, "deu-eng");
    //    //var tei_eng = await ParseTei(Model.Language.German, "eng-deu");
    //    //var idp = await this.ParseIdp(Model.Language.German, "GERMAN");
    //    //var udd = await this.ParseUddl(Model.Language.German, "english-german");
    //    //var dcc = await this.ParseDictCc(Model.Language.German, "dictcc-en-de");
    //    //this.Statistics("German", tei, tei_eng, idp, udd, dcc);
    //}

    //private async void OnButtonSpanishClick(object sender, RoutedEventArgs e)
    //{
    //    GC.Collect();
    //    //var tei = await ParseTei(Model.Language.Spanish, "spa-eng");
    //    //var tei_eng = await ParseTei(Model.Language.Spanish, "eng-spa");
    //    //var idp = await this.ParseIdp(Model.Language.Spanish, "SPANISH");
    //    //var udd = await this.ParseUddl(Model.Language.Spanish, "english-spanish");
    //    //var dcc = await this.ParseDictCc(Model.Language.Spanish, "dictcc-en-sp");
    //    //this.Statistics("Spanish", tei, tei_eng, idp, udd, dcc);
    //}
    #endregion Commented OUT 

    private void OnButtonAllClick(object sender, RoutedEventArgs e) => GC.Collect();

    private void Statistics(
        string language, Dictionary<string, Word> eng, WordTranslator idp, WordTranslator udd, WordTranslator dcc)
    {
        Debug.WriteLine(" ");
        Debug.WriteLine("Statistics --- " + language + " ---");
        Debug.WriteLine(" ");

        int count = 0;
        int countIdp = 0;
        int countUdd = 0;
        int countDcc = 0;
        int count0 = 0;
        int count1 = 0;
        int count2 = 0;
        int count3 = 0;
        foreach (string key in eng.Keys)
        {
            int countX = 0;
            if (idp.ContainsKey(key))
            {
                ++countIdp;
                ++countX;
            }
            else
            {
                // Debug.WriteLine("'" + key + "' missing in TEI");
            }

            if (udd.ContainsKey(key))
            {
                ++countUdd;
                ++countX;
            }
            else
            {
                // Debug.WriteLine("'" + key + "' missing in UDD");
            }


            if (dcc.ContainsKey(key))
            {
                ++countDcc;
                ++countX;
            }
            else
            {
                // Debug.WriteLine("'" + key + "' missing in DCC");
            }

            if (countX == 0)
            {
                // Debug.WriteLine("'" + key + "' missing in All");
                ++count0;
            }
            else if (countX == 1)
            {
                ++count1;
            }
            else if (countX == 2)
            {
                ++count2;
            }
            else if (countX == 3)
            {
                ++count3;
            }
        }

        foreach (string key in eng.Keys)
        {
            if (idp.ContainsKey(key) && udd.ContainsKey(key) && dcc.ContainsKey(key))
            {
                ++count;
                // Debug.WriteLine("'" + key + "'");
            }
        }

        Debug.WriteLine(" ");
        Debug.WriteLine(eng.Keys.Count.ToString() + " TEI words");
        Debug.WriteLine(countIdp.ToString() + " IDP words " + idp.Keys.Count.ToString());
        Debug.WriteLine(countUdd.ToString() + " UDDL words " + udd.Keys.Count.ToString());
        Debug.WriteLine(countDcc.ToString() + " DCC words " + dcc.Keys.Count.ToString());
        Debug.WriteLine(count0.ToString() + " ZERO words");
        Debug.WriteLine(count1.ToString() + " ONE words");
        Debug.WriteLine(count2.ToString() + " TWO words");
        Debug.WriteLine(count3.ToString() + " THREE words");
        Debug.WriteLine(count.ToString() + " ALL words");
        Debug.WriteLine("Complete");
        Debug.WriteLine(" ");
    }
}