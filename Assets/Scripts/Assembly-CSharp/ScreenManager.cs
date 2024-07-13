using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenManager : MonoBehaviour
{
	private static readonly ILogger _log = LogBuilder.Instance.GetLogger(typeof(ScreenManager), LogLevel.Debug);

	public List<ObjectSpawner> spawners;

	public float clearSFXdelay = 0.25f;

	public float maxScreenTime;

	public GameObject[] enemiesToTurnOn;

	public bool isTutorial;

	public bool skipLevel;

	private float _bombCleanupWaitTime = 0.25f;

	private HashSet<Hazard> _bombList = new HashSet<Hazard>();

	private bool _hasStartedWaitingForChildrenToBeKilled;

	private void Start()
	{
		FillBombList();
		SetEnemiesEnabled(false);
	}

	private void OnEnable()
	{
		BombController.BombControllerStarted += HandleBombControllerStarted;
		GameManager.Revived += HandleGameManagerRevived;
	}

	private void OnDisable()
	{
		BombController.BombControllerStarted -= HandleBombControllerStarted;
		GameManager.Revived -= HandleGameManagerRevived;
	}

	private void HandleGameManagerRevived(object sender, EventArgs e)
	{
		StopAllCoroutines();
		if (_hasStartedWaitingForChildrenToBeKilled)
		{
			StartCoroutine(WaitForChildrenToBeKilledCoroutine());
		}
	}

	private void HandleBombControllerStarted(object sender, EventArgs e)
	{
		if ((bool)SkyIronShield.ActiveShield)
		{
		}
	}

	public void ActivateScreen()
	{
		if (maxScreenTime > 0f)
		{
			StartCoroutine(AdvanceScreenAfterTime(maxScreenTime));
		}
		SetEnemiesEnabled(true);
		foreach (ObjectSpawner spawner in spawners)
		{
			spawner.StartSpawn();
		}
		if (!RocketBooster.IsActive)
		{
			SoundEventManager.Instance.Play2D(GlobalSoundEventData.Instance.UI_Scene_Transition_End);
		}
		if (base.enabled)
		{
			StartCoroutine(WaitForChildrenToBeKilledCoroutine());
		}
		else
		{
			_log.LogError("Room Activated even though it wasn't enabled. Was the camera outside the bounding box?");
		}
	}

	private IEnumerator WaitForChildrenToBeKilledCoroutine()
	{
		_log.Log("Started waiting for children to be killed");
		_hasStartedWaitingForChildrenToBeKilled = true;
		int childCount = base.transform.childCount;
		while (childCount > 0 && !skipLevel)
		{
			yield return new WaitForEndOfFrame();
			childCount = base.transform.childCount;
		}
		StartCoroutine(CleanupBombs());
		LevelManager.Instance.RoomCleared();
		if (!isTutorial && !skipLevel)
		{
			yield return new WaitForSeconds(LevelManager.Instance.GetWaitTimeAfterRoomClear());
		}
		MoveToNextRoom();
	}

	private void SetEnemiesEnabled(bool enabled)
	{
		_log.LogDebug("SetEnemiesEnabled({0})", enabled);
		for (int i = 0; i < enemiesToTurnOn.Length; i++)
		{
			if (enemiesToTurnOn[i] != null)
			{
				enemiesToTurnOn[i].SetActive(enabled);
			}
			else
			{
				_log.LogError("Missing reference in objects to destroy in room " + LevelManager.Instance.currentScreenRoot.name);
			}
		}
	}

	public IEnumerator CleanupBombs()
	{
		yield return new WaitForSeconds(_bombCleanupWaitTime);
		foreach (Hazard hazard in _bombList)
		{
			if (hazard != null)
			{
				hazard.DropLoot();
				UnityEngine.Object.Destroy(hazard.gameObject);
			}
		}
		_bombList.Clear();
	}

	private void FillBombList()
	{
		Hazard[] componentsInChildren = base.transform.parent.GetComponentsInChildren<Hazard>();
		foreach (Hazard hazard in componentsInChildren)
		{
			if (hazard.tag == "Bomb")
			{
				_bombList.Add(hazard);
			}
		}
	}

	private void MoveToNextRoom()
	{
		_log.Log("Trying to move to next room...");
		if (GameManager.gameState != GameManager.GameState.Playing)
		{
			_log.Log("Not moving because game state is - " + GameManager.gameState);
			return;
		}
		LevelManager.Instance.GoToNextLevelAsync();
		if (!RocketBooster.IsActive)
		{
			SoundEventManager.Instance.Play2D(GlobalSoundEventData.Instance.UI_Scene_Transition_Start);
		}
	}

	private IEnumerator AdvanceScreenAfterTime(float time)
	{
		yield return new WaitForSeconds(time);
		foreach (Hazard hazard in _bombList)
		{
			hazard.DropLoot();
			UnityEngine.Object.Destroy(hazard.gameObject);
		}
		GameObject[] children = GetChildren();
		GameObject[] array = children;
		foreach (GameObject child in array)
		{
			UnityEngine.Object.Destroy(child);
		}
	}

	private GameObject[] GetChildren()
	{
		GameObject[] array = new GameObject[base.transform.childCount];
		for (int i = 0; i < base.transform.childCount; i++)
		{
			array[i] = base.transform.GetChild(i).gameObject;
		}
		return array;
	}
}
