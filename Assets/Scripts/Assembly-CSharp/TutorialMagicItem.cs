using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialMagicItem : MonoBehaviour
{
	private const float PollWaitTime = 0.5f;

	private const float ReminderTime = 10f;

	public SoundEventData spawnSoundEffect;

	public Transform poofEffect;

	public GameObject trollPrefab;

	public Transform magicItemSpawnPoint;

	public GameObject magicItemCollectablePrefab;

	private bool _powerupTriggered;

	private bool _powerupCollected;

	public GameObject[] trollsToSpawnIn;

	private List<Vector3> _trollPositions = new List<Vector3>();

	private int _trollsToKillBeforeContinuing;

	private void Start()
	{
		GameObject[] array = trollsToSpawnIn;
		foreach (GameObject gameObject in array)
		{
			_trollPositions.Add(gameObject.transform.localPosition);
			UnityEngine.Object.Destroy(gameObject);
		}
	}

	private void SpawnTrolls()
	{
		foreach (Vector3 trollPosition in _trollPositions)
		{
			GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(trollPrefab);
			gameObject.transform.parent = base.gameObject.transform.parent;
			gameObject.transform.localPosition = trollPosition;
			_trollsToKillBeforeContinuing++;
			UnityEngine.Object.Instantiate(poofEffect, gameObject.transform.position, Quaternion.identity);
		}
		SoundEventManager.Instance.Play2D(spawnSoundEffect);
	}

	private void OnEnable()
	{
		LevelManager.ArrivedAtNextRoom += HandleLevelManagerArrivedAtNextRoom;
		Powerup.Triggered += HandlePowerupTriggered;
		MagicItemCollectable.PowerupCollected += HandlePowerupCollected;
		Health.Killed += HandleHealthKilled;
		SwrveEventsTutorials.StartRoomTimer();
	}

	private void OnDisable()
	{
		LevelManager.ArrivedAtNextRoom -= HandleLevelManagerArrivedAtNextRoom;
		Powerup.Triggered -= HandlePowerupTriggered;
		MagicItemCollectable.PowerupCollected -= HandlePowerupCollected;
		Health.Killed -= HandleHealthKilled;
	}

	private void HandleHealthKilled(object sender, EventArgs e)
	{
		_trollsToKillBeforeContinuing--;
	}

	private void HandleLevelManagerArrivedAtNextRoom(object sender, LevelManager.NextRoomEventArgs e)
	{
		StartCoroutine(TutorialCoroutine());
	}

	private void HandlePowerupTriggered(object sender, PowerupEventArgs e)
	{
		_powerupTriggered = true;
		ShipManager.instance.PowerupTutorialHandRendererEnabled = false;
	}

	private void HandlePowerupCollected(object sender, PowerupEventArgs e)
	{
		_powerupCollected = true;
	}

	private IEnumerator TutorialCoroutine()
	{
		NotificationPanel.Instance.DisplayDismissOnRoomTransition(LocalizationManager.Instance.GetString("TUTORIAL_MAGIC_ITEM_COLLECT"));
		TutorialVoiceOverManager.Instance.PlayTouchMagicItem();
		SpawnMagicItem(LevelManager.Instance.powerupToUnlockAfterTutorialsComplete);
		while (!_powerupCollected)
		{
			yield return new WaitForSeconds(0.5f);
		}
		NotificationPanel.Instance.DisplayDismissOnRoomTransition(LocalizationManager.Instance.GetString("TUTORIAL_MAGIC_ITEM_USE"));
		TutorialVoiceOverManager.Instance.PlayActivateMagicItem();
		SwrveEventsTutorials.MagicItemCollectCompleted();
		if (!_powerupTriggered)
		{
			ShipManager.instance.PowerupTutorialHandRendererEnabled = true;
		}
		while (!_powerupTriggered)
		{
			yield return new WaitForSeconds(0.5f);
		}
		yield return new WaitForSeconds(0.5f);
		SpawnTrolls();
		while (_trollsToKillBeforeContinuing > 0)
		{
			yield return new WaitForSeconds(0.5f);
		}
		SwrveEventsTutorials.MagicItemUsedCompleted();
		StringNotificationPanelSettings notificationSettings = StringNotificationPanelSettings.BuildDismissAfterTime(LocalizationManager.Instance.GetString("TUTORIAL_MAGIC_ITEM_UNLOCK"), 4f);
		NotificationPanel.Instance.Display(notificationSettings);
		TutorialVoiceOverManager.Instance.PlayWhyMagicItems();
		yield return new WaitForSeconds(4f);
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private void SpawnMagicItem(PowerupData powerupData)
	{
		GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(magicItemCollectablePrefab, magicItemSpawnPoint.transform.position, magicItemSpawnPoint.transform.rotation);
		gameObject.transform.parent = magicItemSpawnPoint.transform;
		iTween.MoveTo(magicItemSpawnPoint.gameObject, iTween.Hash("position", magicItemSpawnPoint.transform.position + new Vector3(5f, 0f, 0f), "time", 6f, "easetype", iTween.EaseType.linear, "looptype", iTween.LoopType.loop));
		iTween.MoveTo(gameObject.gameObject, iTween.Hash("position", gameObject.transform.localPosition + new Vector3(0f, 1.25f, 0f), "islocal", true, "time", 2f, "easetype", iTween.EaseType.easeInOutSine, "looptype", iTween.LoopType.pingPong));
		MagicItemCollectable componentInChildren = gameObject.GetComponentInChildren<MagicItemCollectable>();
		componentInChildren.SetMagicItem(powerupData);
		componentInChildren.changeAfterSpawn = false;
	}
}
