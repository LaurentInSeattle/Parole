namespace Parole.Game
{
    using System.Windows.Input;

    public sealed class KeyMessage
    {
        public KeyMessage(string key) => this.Key = key[0];

        public char Key { get; private set; }
    }

    public sealed class ControlMessage
    {
        public ControlMessage(Key key ) => this.Key = key;

        public Key Key { get; private set; }
    }
}
