﻿namespace Parole.Game;

public sealed class KeyMessage(string key)
{
    public char Key { get; private set; } = key[0];
}

public sealed class ControlMessage(Key key)
{
    public Key Key { get; private set; } = key;
}
