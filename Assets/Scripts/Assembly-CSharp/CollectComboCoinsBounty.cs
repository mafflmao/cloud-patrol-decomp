using System;

public class CollectComboCoinsBounty : Bounty, IHasComboCoin
{
	public ComboCoin ComboCoin { get; private set; }

	private void OnEnable()
	{
		ComboCoin.Collected += HandleComboCoinCollected;
	}

	private void OnDisable()
	{
		ComboCoin.Collected -= HandleComboCoinCollected;
	}

	private void HandleComboCoinCollected(object sender, EventArgs e)
	{
		ComboCoin = (ComboCoin)sender;
		TryIncrementProgress();
		ComboCoin = null;
	}
}
