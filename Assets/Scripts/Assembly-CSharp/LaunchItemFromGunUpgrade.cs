using UnityEngine;

public class LaunchItemFromGunUpgrade : CharacterUpgrade
{
	public HomingMissile homingMissilePrefab;

	private void OnEnable()
	{
		Shooter.Shooting += HandleShooterShooting;
	}

	private void OnDisable()
	{
		Shooter.Shooting += HandleShooterShooting;
	}

	private void HandleShooterShooting(object sender, Shooter.ShootEventArgs e)
	{
		Shooter shooter = sender as Shooter;
		if (shooter.targetQueue.Count == 1)
		{
			ShootItemAt(shooter.targetQueue.GetNextTarget().transform);
			e.Cancel();
			Debug.Log("Shooting thing");
		}
	}

	private void ShootItemAt(Transform targetTransform)
	{
		GameObject gameObject = Object.Instantiate(homingMissilePrefab.gameObject, ShipManager.instance.muzzleFlash.transform.position, Quaternion.identity) as GameObject;
		HomingMissile component = gameObject.GetComponent<HomingMissile>();
		component.Fire(targetTransform);
	}
}
