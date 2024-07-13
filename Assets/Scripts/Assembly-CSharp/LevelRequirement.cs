using System;

public class LevelRequirement : UpgradeRequirement
{
	public int rankRequired;

	public override string NotMetText
	{
		get
		{
			string @string = LocalizationManager.Instance.GetString("SD_REQUIREMENT_LEVEL");
			return string.Format(@string, rankRequired);
		}
	}

	private void HandlePlayerRankChanged(object sender, EventArgs e)
	{
		OnRecheckRequirements();
	}

	public override bool CheckRequirement()
	{
		return true;
	}
}
