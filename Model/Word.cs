﻿namespace Parole.Model;

public class Word
{
    private readonly static char space = ' ';

    private readonly char[] characters;

    public Word()
    {
        this.characters = new char[Length];
        for (int i = 0; i < Length; i++)
        {
            this.characters[i] = space;
        }
    }

    public static int Length { get; set; } = 5;

    public Word(string word)
    {
        if (string.IsNullOrWhiteSpace(word) || word.Length != Length)
        {
            throw new ArgumentException("null, empty or incorrect length", nameof(word));
        }

        this.characters = new char[Length];
        for (int i = 0; i < Length; i++)
        {
            this.characters[i] = word[i];
        }
    }

    public bool IsComplete => !this.characters.Contains(space);

    public bool IsEvaluated { get; set; }

    public bool IsEmpty(int column) => this.characters[column] == space;

    public void Clear(int column) => this.characters[column] = space;

    public void Set(int column, char character) => this.characters[column] = char.ToUpper(character);

    public char Get(int column) => this.characters[column];

    public string AsString ()
    {
        string result = string.Empty;
        for (int i = 0; i < Length; i++)
        {
            result = string.Concat(result, this.characters[i]);
        }

        return result;
    }

    public Placement Evaluate(Word solution, out bool isFound)
    {
        isFound = false;
        var placement = new Placement();

        if (!this.IsComplete)
        {
            return placement;
        }

        string solutionAsString = solution.AsString();
        for (int i = 0; i < Length; i++)
        {
            if (!solutionAsString.Contains(this.characters[i], StringComparison.InvariantCultureIgnoreCase))
            {
                placement[i] = CharacterPlacement.Absent;
            }
        }

        var list = new List<char>(Word.Length);
        for (int i = 0; i < Length; i++)
        {
            char testChar = char.ToLower(solution.characters[i]); 
            if (char.ToLower(this.characters[i]) == testChar)
            {
                placement[i] = CharacterPlacement.Exact;
            }
            else
            {
                list.Add(testChar);
            }
        }

        bool found = list.Count == 0 ;
        if (!found)
        {
            for (int i = 0; i < Length; i++)
            {
                if (placement[i] == CharacterPlacement.Exact)
                {
                    continue;
                }

                char testChar = char.ToLower(this.characters[i]);
                if (list.Contains(testChar))
                {
                    placement[i] = CharacterPlacement.Present;
                    list.Remove(testChar);
                }
            }
        }

        for (int i = 0; i < Length; i++)
        {
            if (placement[i] == CharacterPlacement.Unknown)
            {
                placement[i] = CharacterPlacement.Absent;
            } 
        }

        this.IsEvaluated = true;
        isFound = found;
        return placement;
    }
}
