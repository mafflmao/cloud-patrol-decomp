using System.Collections.Generic;
using UnityEngine;

public class ClearInOneShotBounty : Bounty
{
	private GameObject[] _enemiesInCurrentRoomAtStart;

	private bool _stillPossibleThisRoom;

	private void OnEnable()
	{
		LevelManager.ArrivedAtNextRoom += HandleLevelManagerArrivedAtNextRoom;
		Shooter.Shooting += HandleShooterShooting;
	}

	private void OnDisable()
	{
		LevelManager.ArrivedAtNextRoom -= HandleLevelManagerArrivedAtNextRoom;
		Shooter.Shooting -= HandleShooterShooting;
	}

	private void HandleShooterShooting(object sender, Shooter.ShootEventArgs e)
	{
		if (!_stillPossibleThisRoom)
		{
			return;
		}
		HashSet<GameObject> hashSet = new HashSet<GameObject>(e.Targets);
		GameObject[] enemiesInCurrentRoomAtStart = _enemiesInCurrentRoomAtStart;
		foreach (GameObject gameObject in enemiesInCurrentRoomAtStart)
		{
			if (gameObject == null || !hashSet.Contains(gameObject))
			{
				_stillPossibleThisRoom = false;
				return;
			}
		}
		TryIncrementProgress();
	}

	private void HandleLevelManagerArrivedAtNextRoom(object sender, LevelManager.NextRoomEventArgs e)
	{
		List<GameObject> list = new List<GameObject>();
		ScreenManager screenManager = e.ScreenManager;
		if (screenManager != null)
		{
			int childCount = screenManager.transform.childCount;
			for (int i = 0; i < childCount; i++)
			{
				Transform child = screenManager.transform.GetChild(i);
				Health component = child.GetComponent<Health>();
				if (component.isEnemy)
				{
					list.Add(child.gameObject);
				}
			}
		}
		_enemiesInCurrentRoomAtStart = list.ToArray();
		_stillPossibleThisRoom = true;
	}
}
