namespace Lyt.DicBuilder.Model;

public sealed class StringTranslator : Dictionary<string, List<string>>
{
    public StringTranslator(Language source, Language target, int capacity) : base(capacity, StringComparer.InvariantCultureIgnoreCase)
    {
        this.Source = source;
        this.Target = target;
    }

    public Language Source { get; }

    public Language Target { get; }

    public void Consolidate(StringTranslator other)
    {
        if ((this.Source != other.Target) || (this.Target != other.Source))
        {
            throw new InvalidOperationException("Cant consolidate");
        }

        Debug.WriteLine("StringTranslator - Consolidate: " + this.Source.ToString() +  " - " + this.Target.ToString() );

        var toRemove = new List<Tuple<string, string>>(this.Count);
        foreach (var kvp in this)
        {
            string src = kvp.Key;
            foreach (string value in kvp.Value)
            {
                bool noReverse = false;
                if (other.TryGetValue(value, out var list))
                {
                    if ((list == null) || (list.Count == 0))
                    {
                        noReverse = true;
                    }
                    else
                    {
                        if (!list.Contains(src, StringComparer.InvariantCultureIgnoreCase))
                        {
                            noReverse = true;
                        }
                    }
                }
                else
                {
                    noReverse = true;
                }

                if (noReverse)
                {
                    toRemove.Add(new Tuple<string, string>(src, value));
                }
            }
        }

        //foreach (var tuple in toRemove)
        //{
        //    Debug.WriteLine(tuple.Item1 + " - " + tuple.Item2);
        //}

        foreach (var tuple in toRemove)
        {
            if (this.TryGetValue(tuple.Item1, out var list))
            {
                list.Remove(tuple.Item2);
            }
        }

        this.Statistics();
    }

    [Conditional("DEBUG")]
    public void Statistics()
    {
        Debug.WriteLine("  ");
        Debug.WriteLine("  ");
        const int N = 31;
        int[] counts = new int[N];
        foreach (var kvp in this)
        {
            int listCount = kvp.Value.Count;
            if (listCount < N - 1)
            {
                ++counts[listCount];
            }
            else
            {
                ++counts[N - 1];
            }
        }

        for (int i = 1; i < N; ++i)
        {
            Debug.WriteLine(string.Format("Count {0:D2}: {1:D4}", i, counts[i]));
        }

        Debug.WriteLine("  ");
        Debug.WriteLine("  ");
        foreach (var kvp in this)
        {
            int listCount = kvp.Value.Count;
            if (listCount > 12)
            {
                Debug.WriteLine("Entry: " + kvp.Key + " has more than 12 translations" );
                //foreach (string value in kvp.Value)
                //{
                //    Debug.WriteLine("\t\t\t" + value);
                //}
            }
        }
    }

    public void CompareSingles(StringTranslator other)
    {
        if ((this.Source != other.Target) || (this.Target != other.Source))
        {
            throw new InvalidOperationException("Cant consolidate");
        }

        int matches = 0;
        foreach (var kvp in this)
        {
            string src = kvp.Key;
            var list = kvp.Value;

            if (list.Count == 0)
            {
                continue;
            }

            if ( list.Count > 1 )
            {
                continue;
            }

            string dst = list[0];
            if ( other.TryGetValue ( dst, out list ))
            {
                if (list.Count == 1)
                {
                    string match = list[0];
                    if ( match == src )
                    {
                        ++matches;
                        // Debug.WriteLine(src + "\t\t" + dst);
                    }
                }
                else
                {
                    continue;
                }
            }
            else
            {
                continue;
            }
        }

        Debug.WriteLine("Single Exact matches: " + matches);
    }
}