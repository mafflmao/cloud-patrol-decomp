using UnityEngine;

public class IsBossRoomModifier : NeedsOwnerModifier<IHasRoomRoot>
{
	public override bool AllowIncrement()
	{
		if (WingedBoots.IsActive || RocketBooster.IsActive)
		{
			return false;
		}
		string sceneName = base.Owner.RoomRoot.name;
		Difficulty? difficultyForRoom = LevelManager.Instance.CurrentLevel.GetDifficultyForRoom(sceneName);
		Debug.Log("Room Cleared - Difficulty - " + difficultyForRoom);
		return difficultyForRoom == Difficulty.Boss;
	}
}
