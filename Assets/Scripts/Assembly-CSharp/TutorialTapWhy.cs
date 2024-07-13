using UnityEngine;

public class TutorialTapWhy : MonoBehaviour
{
	public Transform[] trolls;

	public GameObject poofEffect;

	public GameObject theHand;

	public SoundEventData poofSound;

	public void OnEnable()
	{
		LevelManager.ArrivedAtNextRoom += HandleLevelManagerArrivedAtNextRoom;
	}

	private void OnDisable()
	{
		LevelManager.ArrivedAtNextRoom -= HandleLevelManagerArrivedAtNextRoom;
	}

	private void HandleLevelManagerArrivedAtNextRoom(object sender, LevelManager.NextRoomEventArgs e)
	{
		NotificationPanel.Instance.DisplayDismissOnRoomTransition(LocalizationManager.Instance.GetString("TUTORIAL_SHOOT_ALL_TROLLS"));
		TutorialVoiceOverManager.Instance.PlayWhyTouch();
	}
}
