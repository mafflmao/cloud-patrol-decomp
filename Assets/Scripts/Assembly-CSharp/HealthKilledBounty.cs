using System;
using UnityEngine;

public class HealthKilledBounty : Bounty, IHasHealthScript
{
	[HideInInspector]
	public Health Health { get; private set; }

	private void OnEnable()
	{
		Health.Killed += HandleHealthKilled;
	}

	private void OnDisable()
	{
		Health.Killed -= HandleHealthKilled;
	}

	private void HandleHealthKilled(object sender, EventArgs e)
	{
		Health = (Health)sender;
		TryIncrementProgress();
		Health = null;
	}
}
