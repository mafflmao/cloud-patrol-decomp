using UnityEngine;

public class DebugRoomDisplay : MonoBehaviour
{
	public SpriteText roomNameText;

	private string lastRoomName = "?";

	private void Start()
	{
		Object.Destroy(base.gameObject);
	}

	private void OnEnable()
	{
		LevelManager.ArrivedAtNextRoom += HandleLevelManagerArrivedAtNextRoom;
		LevelManager.MovingToNextRoom += HandleLevelManagerMovingToNextRoom;
	}

	private void OnDisable()
	{
		LevelManager.ArrivedAtNextRoom -= HandleLevelManagerArrivedAtNextRoom;
		LevelManager.MovingToNextRoom -= HandleLevelManagerMovingToNextRoom;
	}

	private void HandleLevelManagerArrivedAtNextRoom(object sender, LevelManager.NextRoomEventArgs e)
	{
		roomNameText.Text = e.RoomRoot.name;
		lastRoomName = e.RoomRoot.name;
	}

	private void HandleLevelManagerMovingToNextRoom(object sender, LevelManager.NextRoomEventArgs e)
	{
		roomNameText.Text = lastRoomName + "->" + e.RoomRoot.name;
	}
}
