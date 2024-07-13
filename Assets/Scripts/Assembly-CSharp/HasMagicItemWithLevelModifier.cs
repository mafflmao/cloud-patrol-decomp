using System.Text;

public class HasMagicItemWithLevelModifier : BountyModifier
{
	public int MinimumLevel { get; set; }

	public override bool AllowIncrement()
	{
		return false;
	}

	public override void PerformDescriptionReplacement(StringBuilder stringBuilder)
	{
		base.PerformDescriptionReplacement(stringBuilder);
		stringBuilder.Replace("{level}", MinimumLevel.ToString());
	}
}
