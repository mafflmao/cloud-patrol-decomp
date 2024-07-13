using System;
using System.Text;

public class IsMagicItemAtLeastLevelModifier : BountyModifier
{
	public int MinimumItemLevel { get; set; }

	public override void Initialize(Bounty owner)
	{
		base.Initialize(owner);
		if (owner is IHasMagicItem)
		{
			return;
		}
		throw new Exception("Cannot initialize '" + GetType().Name + "' with owner that is not '" + typeof(IHasMagicItem).Name + "'");
	}

	public override bool AllowIncrement()
	{
		int num = 0;
		return num >= MinimumItemLevel;
	}

	public override void PerformDescriptionReplacement(StringBuilder stringBuilder)
	{
		base.PerformDescriptionReplacement(stringBuilder);
		stringBuilder.Replace("{level}", MinimumItemLevel.ToString());
	}
}
