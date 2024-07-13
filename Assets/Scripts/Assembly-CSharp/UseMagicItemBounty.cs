public class UseMagicItemBounty : Bounty, IHasMagicItem
{
	public PowerupData PowerupData { get; private set; }

	private void OnEnable()
	{
		Powerup.Triggered += HandlePowerupHolderPowerupTriggered;
	}

	private void OnDisable()
	{
		Powerup.Triggered -= HandlePowerupHolderPowerupTriggered;
	}

	private void HandlePowerupHolderPowerupTriggered(object sender, PowerupEventArgs e)
	{
		PowerupData = e.PowerupData;
		TryIncrementProgress();
		PowerupData = null;
	}
}
