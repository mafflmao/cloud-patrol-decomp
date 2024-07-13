public class UsingBonusElementModifier : BountyModifier
{
	public bool RequireCorrectElement { get; set; }

	public override bool AllowIncrement()
	{
		if (RequireCorrectElement)
		{
			return StartGameSettings.Instance.IsBonusElementActive;
		}
		return !StartGameSettings.Instance.IsBonusElementActive;
	}
}
