using System.Collections;
using UnityEngine;

public class TutorialTap : MonoBehaviour
{
	public Transform[] trolls;

	public GameObject poofEffect;

	public GameObject theHand;

	public SoundEventData poofSound;

	private int nextTroll;

	private void Start()
	{
		HideTrolls();
	}

	public void OnEnable()
	{
		LevelManager.ArrivedAtNextRoom += HandleLevelManagerArrivedAtNextRoom;
		Shooter.ComboCompleted += HandleComboCompleted;
		LevelManager.RoomClear += HandleLevelManagerRoomClear;
		SwrveEventsTutorials.StartRoomTimer();
	}

	private void OnDisable()
	{
		LevelManager.ArrivedAtNextRoom -= HandleLevelManagerArrivedAtNextRoom;
		Shooter.ComboCompleted -= HandleComboCompleted;
		LevelManager.RoomClear -= HandleLevelManagerRoomClear;
	}

	private void HandleLevelManagerRoomClear(object sender, LevelManager.RoomClearEventArgs e)
	{
		iTween.MoveBy(theHand, new Vector3(0f, -6f, 0f), 0.5f);
		SwrveEventsTutorials.TapShootCompleted();
	}

	public void HideTrolls()
	{
		Transform[] array = trolls;
		foreach (Transform transform in array)
		{
			transform.GetComponent<Troll>().enabled = false;
			transform.position += new Vector3(0f, 2.5f, 0f);
		}
	}

	private void HandleComboCompleted(object sender, Shooter.ComboCompletedEventArgs args)
	{
		iTween.Stop(theHand);
		StartCoroutine(SpawnInTroll());
	}

	public IEnumerator SpawnInTroll()
	{
		if (nextTroll == 0)
		{
			yield return new WaitForSeconds(1f);
		}
		yield return new WaitForSeconds(0.5f);
		if (nextTroll < trolls.Length)
		{
			trolls[nextTroll].GetComponent<Troll>().enabled = true;
			trolls[nextTroll].position -= new Vector3(0f, 2.5f, 0f);
			SoundEventManager.Instance.Play(poofSound, theHand);
			Object.Instantiate(poofEffect, trolls[nextTroll].GetComponent<Collider>().bounds.center, Quaternion.Euler(0f, 0f, 0f));
			yield return new WaitForSeconds(0.4f);
			iTween.MoveTo(theHand, trolls[nextTroll].GetComponent<Collider>().bounds.center, 0.5f);
		}
		else
		{
			iTween.MoveBy(theHand, new Vector3(0f, -6f, 0f), 0.5f);
		}
		nextTroll++;
	}

	private void HandleLevelManagerArrivedAtNextRoom(object sender, LevelManager.NextRoomEventArgs e)
	{
		NotificationPanel.Instance.DisplayDismissOnRoomTransition(LocalizationManager.Instance.GetString("TUTORIAL_TAP_TROLLS"));
		TutorialVoiceOverManager.Instance.PlayTapTrolls();
		StartCoroutine(SpawnInTroll());
	}
}
