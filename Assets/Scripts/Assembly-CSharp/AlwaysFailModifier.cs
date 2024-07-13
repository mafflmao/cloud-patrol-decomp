using System.Text;

public class AlwaysFailModifier : BountyModifier
{
	public override bool AllowIncrement()
	{
		return false;
	}

	public override void PerformDescriptionReplacement(StringBuilder stringBuilder)
	{
	}
}
