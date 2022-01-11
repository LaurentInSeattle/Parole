namespace Parole.Model
{
    public enum CharacterPlacement
    {
        Unknown,
        Absent,
        Present,
        Exact,
    }

    public class Placement
    {
        private readonly CharacterPlacement[] characterPlacements;

        public Placement()
        {
            this.characterPlacements = new CharacterPlacement[Word.Length];
            for (int i = 0; i < Word.Length; i++)
            {
                this.characterPlacements[i] = CharacterPlacement.Unknown;
            }
        }

        public CharacterPlacement this[int i]
        {
            get { return this.characterPlacements[i]; }
            set { this.characterPlacements[i] = value; }
        }
    }
}
