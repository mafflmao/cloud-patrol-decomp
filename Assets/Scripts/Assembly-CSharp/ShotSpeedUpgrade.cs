using System;
using UnityEngine;

public class ShotSpeedUpgrade : CharacterUpgrade
{
	public float rateOfFire = 0.08f;

	public float bulletTravelTime = 0.1f;

	public float windUpTime;

	public Material altTargetLineMaterial;

	private void OnEnable()
	{
		Shooter.Shooting += HandleShooterShooting;
		TargetQueue.TargetAdded += HandleTargetQueueTargetAdded;
	}

	private void OnDisable()
	{
		Shooter.Shooting -= HandleShooterShooting;
		TargetQueue.TargetAdded -= HandleTargetQueueTargetAdded;
	}

	private void HandleTargetQueueTargetAdded(object sender, EventArgs e)
	{
		if (altTargetLineMaterial != null)
		{
			TargetQueue targetQueue = sender as TargetQueue;
			if (targetQueue.targetLine.GetComponent<Renderer>().material != altTargetLineMaterial)
			{
				targetQueue.targetLine.SetMaterial(altTargetLineMaterial);
			}
		}
	}

	private void HandleShooterShooting(object sender, Shooter.ShootEventArgs e)
	{
		Shooter shooter = sender as Shooter;
		if (shooter != null)
		{
			shooter.rateOfFire = rateOfFire;
			shooter.bulletTravelTime = bulletTravelTime;
			shooter.windUpTime = windUpTime;
		}
	}
}
