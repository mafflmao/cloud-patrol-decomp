using System;
using UnityEngine;

public class SplashDamageKilledBounty : Bounty, IHasHealthScript
{
	[HideInInspector]
	public Health Health { get; private set; }

	private void OnEnable()
	{
		SplashDamage.Killed += HandleSplashDamageKilled;
	}

	private void OnDisable()
	{
		SplashDamage.Killed -= HandleSplashDamageKilled;
	}

	private void HandleSplashDamageKilled(object sender, EventArgs e)
	{
		Health = (Health)sender;
		TryIncrementProgress();
		Health = null;
	}
}
