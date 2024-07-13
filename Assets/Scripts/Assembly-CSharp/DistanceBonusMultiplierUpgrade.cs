using System;

public class DistanceBonusMultiplierUpgrade : CharacterUpgrade
{
	public int multiplierBoost;

	public int roomToApply;

	public bool AlreadyAppliedUpdate { get; set; }

	private void OnEnable()
	{
		GameManager.GameOver += ResetDistanceBonusMultiplierUpgrade;
	}

	private void OnDisable()
	{
		GameManager.GameOver -= ResetDistanceBonusMultiplierUpgrade;
	}

	private void ResetDistanceBonusMultiplierUpgrade(object sender, EventArgs e)
	{
		AlreadyAppliedUpdate = false;
	}
}
