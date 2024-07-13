public class CharacterSubType
{
	public enum CharacterProduct
	{
		Skylanders2011 = 0,
		Skylanders2012 = 1,
		Skylanders2013 = 2,
		Skylanders2014 = 3,
		Skylanders2015 = 4
	}

	public enum CharacterFeature
	{
		WowPow = 8,
		FullAlternateDeco = 4,
		LightCore = 2,
		TBD = 1,
		None = 0
	}

	public CharacterProduct Product { get; private set; }

	public CharacterFeature Feature { get; private set; }

	public CharacterSubType(uint subTypeRaw)
	{
		byte product = (byte)(((subTypeRaw >> 8) & 0xF0) >> 4);
		byte feature = (byte)((subTypeRaw >> 8) & 0xFu);
		Product = (CharacterProduct)product;
		Feature = (CharacterFeature)feature;
	}
}
