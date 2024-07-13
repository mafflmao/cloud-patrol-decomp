using UnityEngine;

public class SurviveBounty : Bounty, IHasRoomRoot
{
	private bool _initialRoomCleared;

	public GameObject RoomRoot { get; private set; }

	private void OnEnable()
	{
		LevelManager.RoomClear += HandleLevelManagerRoomClear;
	}

	private void OnDisable()
	{
		LevelManager.RoomClear -= HandleLevelManagerRoomClear;
	}

	private void HandleLevelManagerRoomClear(object sender, LevelManager.RoomClearEventArgs e)
	{
		if (!_initialRoomCleared)
		{
			_initialRoomCleared = true;
		}
		else if (LevelManager.Instance.FinishedTutorials)
		{
			RoomRoot = e.RootNode;
			TryIncrementProgress();
			RoomRoot = null;
		}
	}
}
