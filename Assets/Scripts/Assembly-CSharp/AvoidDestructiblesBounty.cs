using System;

public class AvoidDestructiblesBounty : Bounty
{
	private bool _hitDestructibleInCurrentRoom;

	private bool _initialRoomCleared;

	private void OnEnable()
	{
		LevelManager.ArrivedAtNextRoom += HandleLevelManagerArrivedAtNextRoom;
		LevelManager.RoomClear += HandleLevelManagerRoomClear;
		Health.TookHit += HandleHealthTookHit;
	}

	private void OnDisable()
	{
		LevelManager.ArrivedAtNextRoom -= HandleLevelManagerArrivedAtNextRoom;
		LevelManager.RoomClear -= HandleLevelManagerRoomClear;
		Health.TookHit -= HandleHealthTookHit;
	}

	private void HandleLevelManagerRoomClear(object sender, LevelManager.RoomClearEventArgs e)
	{
		if (!_initialRoomCleared)
		{
			_initialRoomCleared = true;
		}
		else if (!_hitDestructibleInCurrentRoom && LevelManager.Instance.FinishedTutorials)
		{
			TryIncrementProgress();
		}
	}

	private void HandleLevelManagerArrivedAtNextRoom(object sender, LevelManager.NextRoomEventArgs e)
	{
		_hitDestructibleInCurrentRoom = false;
	}

	private void HandleHealthTookHit(object sender, EventArgs e)
	{
		Health healthComponent = (Health)sender;
		if (IsDestructibleModifier.IsHealthAttachedToDestructible(healthComponent))
		{
			_hitDestructibleInCurrentRoom = true;
			ResetProgress();
		}
	}
}
