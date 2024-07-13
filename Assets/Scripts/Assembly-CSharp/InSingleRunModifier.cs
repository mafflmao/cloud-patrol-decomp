using System.Text;

public class InSingleRunModifier : BountyModifier
{
	public override bool AllowIncrement()
	{
		return true;
	}

	public override void PerformDescriptionReplacement(StringBuilder stringBuilder)
	{
	}
}
