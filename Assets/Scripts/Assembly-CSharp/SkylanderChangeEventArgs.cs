using System;

public class SkylanderChangeEventArgs : EventArgs
{
	public CharacterData OldCharacter { get; private set; }

	public SkylanderChangeEventArgs(CharacterData oldCharacter)
	{
		OldCharacter = oldCharacter;
	}
}
