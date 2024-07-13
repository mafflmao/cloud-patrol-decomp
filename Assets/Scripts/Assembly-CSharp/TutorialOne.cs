using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class TutorialOne : MonoBehaviour
{
	public GameObject theHand;

	public SoundEventData TutorialErrorBuzzer;

	private bool _killedTrolls;

	private bool _comboCoinSpawned;

	private bool _started;

	private void OnEnable()
	{
		LevelManager.ArrivedAtNextRoom += HandleLevelManagerArrivedAtNextRoom;
		Shooter.Shooting += HandleShooterShooting;
		ComboCoin.CoinTimeout += HandleComboCoinTimeout;
		ComboCoin.ComboCoinSpawned += HandleComboCoinComboCoinSpawned;
		TargetQueue.TargetAdded += HandleTargetQueueTargetAdded;
		SwrveEventsTutorials.StartRoomTimer();
	}

	private void OnDisable()
	{
		LevelManager.ArrivedAtNextRoom -= HandleLevelManagerArrivedAtNextRoom;
		Shooter.Shooting -= HandleShooterShooting;
		ComboCoin.CoinTimeout -= HandleComboCoinTimeout;
		ComboCoin.ComboCoinSpawned -= HandleComboCoinComboCoinSpawned;
		TargetQueue.TargetAdded -= HandleTargetQueueTargetAdded;
	}

	private void HandleTargetQueueTargetAdded(object sender, EventArgs args)
	{
		DisplayHand(false);
	}

	private void DisplayHand(bool state)
	{
		if (state)
		{
			theHand.GetComponent<Animation>().Play();
		}
		else
		{
			theHand.GetComponent<Animation>().Stop();
		}
		theHand.GetComponentInChildren<TrailRenderer>().enabled = state;
		theHand.GetComponent<Renderer>().enabled = state;
		foreach (Transform item in theHand.transform)
		{
			item.GetComponent<MeshRenderer>().enabled = state;
		}
	}

	private IEnumerator OnboardingSequence()
	{
		_started = true;
		if (!_killedTrolls)
		{
			NotificationPanel.Instance.DisplayDismissOnRoomTransition(LocalizationManager.Instance.GetString("TUTORIAL_SLIDE"));
			TutorialVoiceOverManager.Instance.PlaySlideTrolls();
			yield return new WaitForSeconds(1f);
			while (!_killedTrolls)
			{
				yield return new WaitForSeconds(0.1f);
			}
		}
		NotificationPanel.Instance.DisplayDismissOnRoomTransition(LocalizationManager.Instance.GetString("TUTORIAL_COINS"));
		TutorialVoiceOverManager.Instance.PlayTouchCoins();
		while (!_comboCoinSpawned)
		{
			yield return new WaitForSeconds(0.1f);
		}
		DisplayHand(true);
		theHand.GetComponent<Animation>().Play("HandCollectCoins");
		bool anyMoneyAround = AnyMoneyRemaining();
		while (anyMoneyAround)
		{
			yield return new WaitForSeconds(0.1f);
			anyMoneyAround = AnyMoneyRemaining();
		}
		DisplayHand(false);
		SwrveEventsTutorials.CoinCollectCompleted();
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private bool AnyMoneyRemaining()
	{
		return GameObject.FindGameObjectsWithTag("ComboCoin").Any();
	}

	private void HandleLevelManagerArrivedAtNextRoom(object sender, LevelManager.NextRoomEventArgs e)
	{
		StartCoroutine(OnboardingSequence());
	}

	private void HandleComboCoinTimeout(object sender, CancellableEventArgs e)
	{
		e.Cancel();
	}

	private void HandleComboCoinComboCoinSpawned(object sender, EventArgs e)
	{
		_comboCoinSpawned = true;
	}

	private void HandleShooterShooting(object sender, Shooter.ShootEventArgs e)
	{
		if (e.ComboSize != 3 || !_started)
		{
			e.Cancel();
			DisplayHand(true);
			TutorialVoiceOverManager.Instance.PlayNegativeFeedback();
			SwrveEventsTutorials.ComboFailed();
		}
		else
		{
			_killedTrolls = true;
			SwrveEventsTutorials.ComboCompleted();
		}
	}
}
