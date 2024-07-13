using System.Linq;

public class HasUnlockedMagicItemModifier : BountyModifier
{
	public override bool AllowIncrement()
	{
		return MagicItemManager.Instance.UnlockedMagicItems.Count() > 1;
	}
}
