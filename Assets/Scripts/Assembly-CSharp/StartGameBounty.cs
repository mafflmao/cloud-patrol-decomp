using System;

public class StartGameBounty : Bounty
{
	private void OnEnable()
	{
		GameManager.GameStarted += HandleGameManagerGameStarted;
	}

	private void OnDisable()
	{
		GameManager.GameStarted -= HandleGameManagerGameStarted;
	}

	private void HandleGameManagerGameStarted(object sender, EventArgs e)
	{
		TryIncrementProgress();
	}
}
