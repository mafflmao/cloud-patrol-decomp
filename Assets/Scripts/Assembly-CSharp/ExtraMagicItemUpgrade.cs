public class ExtraMagicItemUpgrade : CharacterUpgrade
{
	public PowerupData powerup;

	private void OnEnable()
	{
		Powerup.Finished += HandlePowerupFinished;
	}

	private void OnDisable()
	{
		Powerup.Finished -= HandlePowerupFinished;
	}

	private void HandlePowerupFinished(object sender, PowerupEventArgs e)
	{
		Powerup powerup = (Powerup)sender;
		if (e.PowerupData == this.powerup && !powerup.IsBonus)
		{
			PowerupHolder availablePowerupHolder = ShipManager.instance.GetAvailablePowerupHolder();
			availablePowerupHolder.QueuePowerup(this.powerup, true);
		}
	}
}
