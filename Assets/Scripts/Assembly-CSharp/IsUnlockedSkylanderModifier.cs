using System.Linq;

public class IsUnlockedSkylanderModifier : BountyModifier
{
	public override bool AllowIncrement()
	{
		string text = StartGameSettings.Instance.activeSkylander.name;
		string text2 = UnlockedCharacterCache.Instance.OrderedCharacterUnlocks.FirstOrDefault();
		return text != text2;
	}
}
