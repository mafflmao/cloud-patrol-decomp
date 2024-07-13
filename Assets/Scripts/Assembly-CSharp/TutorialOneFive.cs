using System;
using System.Collections;
using UnityEngine;

public class TutorialOneFive : MonoBehaviour
{
	public SoundEventData TutorialErrorBuzzer;

	public GameObject theHand;

	private bool _killedTrolls;

	private bool _remindUser;

	private bool _comboCoinSpwaned;

	private void OnEnable()
	{
		LevelManager.ArrivedAtNextRoom += HandleLevelManagerArrivedAtNextRoom;
		Shooter.Shooting += HandleShooterShooting;
		ComboCoin.ComboCoinSpawned += HandleComboCoinComboCoinSpawned;
		ComboCoin.CoinTimeout += HandleComboCoinTimeout;
		TargetQueue.TargetAdded += HandleTargetQueueTargetAdded;
		SwrveEventsTutorials.StartRoomTimer();
	}

	private void OnDisable()
	{
		LevelManager.ArrivedAtNextRoom -= HandleLevelManagerArrivedAtNextRoom;
		Shooter.Shooting -= HandleShooterShooting;
		ComboCoin.ComboCoinSpawned -= HandleComboCoinComboCoinSpawned;
		ComboCoin.CoinTimeout -= HandleComboCoinTimeout;
		TargetQueue.TargetAdded -= HandleTargetQueueTargetAdded;
	}

	private void HandleTargetQueueTargetAdded(object sender, EventArgs args)
	{
		DisplayHand(false);
	}

	private void DisplayHand(bool state)
	{
		theHand.GetComponentInChildren<TrailRenderer>().time = ((!state) ? 0f : 2f);
		theHand.GetComponent<Renderer>().enabled = state;
		foreach (Transform item in theHand.transform)
		{
			item.GetComponent<MeshRenderer>().enabled = state;
		}
	}

	private IEnumerator OnboardingSequence()
	{
		_remindUser = true;
		while (!_killedTrolls)
		{
			if (_remindUser)
			{
				NotificationPanel.Instance.DisplayDismissOnRoomTransition(LocalizationManager.Instance.GetString("TUTORIAL_TARGET_ALL_6"));
				TutorialVoiceOverManager.Instance.PlayTargetMaxCombo();
				_remindUser = false;
			}
			yield return new WaitForSeconds(0.1f);
		}
		while (!_comboCoinSpwaned)
		{
			yield return new WaitForSeconds(0.1f);
		}
		NotificationPanel.Instance.DisplayDismissOnRoomTransition(LocalizationManager.Instance.GetString("TUTORIAL_GREAT_COMBO"));
		TutorialVoiceOverManager.Instance.PlayTouchCoinsMaxCombo();
		bool anyMaxComboCoins = AnyMaxComboCoins();
		while (anyMaxComboCoins)
		{
			yield return new WaitForSeconds(0.1f);
			anyMaxComboCoins = AnyMaxComboCoins();
		}
		SwrveEventsTutorials.ComboCoinCollectCompleted();
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private bool AnyMaxComboCoins()
	{
		GameObject[] array = GameObject.FindGameObjectsWithTag("ComboCoin");
		GameObject[] array2 = array;
		foreach (GameObject gameObject in array2)
		{
			ComboCoin component = gameObject.GetComponent<ComboCoin>();
			if ((bool)component && component.number == 6)
			{
				return true;
			}
		}
		return false;
	}

	private void HandleComboCoinTimeout(object sender, CancellableEventArgs e)
	{
		e.Cancel();
	}

	private void HandleComboCoinComboCoinSpawned(object sender, EventArgs e)
	{
		_comboCoinSpwaned = true;
	}

	private void HandleLevelManagerArrivedAtNextRoom(object sender, LevelManager.NextRoomEventArgs e)
	{
		StartCoroutine(OnboardingSequence());
	}

	private void HandleShooterShooting(object sender, Shooter.ShootEventArgs e)
	{
		if (e.ComboSize == 6)
		{
			_killedTrolls = true;
			SwrveEventsTutorials.MaxComboCompleted();
			return;
		}
		NotificationPanel.Instance.DisplayDismissOnRoomTransition(LocalizationManager.Instance.GetString("TUTORIAL_TARGET_ALL_6"));
		e.Cancel();
		DisplayHand(true);
		TutorialVoiceOverManager.Instance.PlayNegativeFeedback();
		SwrveEventsTutorials.MaxComboFailed();
	}
}
