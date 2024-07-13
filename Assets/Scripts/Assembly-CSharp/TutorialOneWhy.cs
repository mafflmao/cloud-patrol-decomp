using UnityEngine;

public class TutorialOneWhy : MonoBehaviour
{
	private void OnEnable()
	{
		LevelManager.ArrivedAtNextRoom += HandleLevelManagerArrivedAtNextRoom;
	}

	private void OnDisable()
	{
		LevelManager.ArrivedAtNextRoom -= HandleLevelManagerArrivedAtNextRoom;
	}

	private void HandleLevelManagerArrivedAtNextRoom(object sender, LevelManager.NextRoomEventArgs e)
	{
		NotificationPanel.Instance.DisplayDismissOnRoomTransition(LocalizationManager.Instance.GetString("TUTORIAL_BIGGER_COMBOS"));
		TutorialVoiceOverManager.Instance.PlayWhySwipe();
	}
}
