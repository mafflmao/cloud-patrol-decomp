using System;

public class MagicItemUnlockedRequirement : UpgradeRequirement
{
	public PowerupData magicItem;

	public override string NotMetText
	{
		get
		{
			string @string = LocalizationManager.Instance.GetString("SD_REQUIREMENT_MAGIC_ITEM");
			return string.Format(@string, magicItem.LocalizedName);
		}
	}

	private void HandleMagicItemUnlocked(object sender, EventArgs e)
	{
		OnRecheckRequirements();
	}

	public override bool CheckRequirement()
	{
		return true;
	}
}
