using System;
using System.Collections.Generic;
using UnityEngine;

public class MagicItemManager : SingletonMonoBehaviour
{
	public SteppedCounter magicItemSpawnLimitFactor;

	public PowerupList powerups;

	public GameObject magicItemCollectable;

	public float baseChanceToSpawn = 0.2f;

	public float maxChanceToSpawn = 0.3f;

	public float magicItemChangeTime = 1f;

	public bool debugSpawn;

	public static bool IsMagicItemActive;

	private PowerupData _activeItem;

	private GameObject _currentMagicItem;

	private List<PowerupData> _unlockedPowerups = new List<PowerupData>();

	private List<PowerupData> _lockedPowerups = new List<PowerupData>();

	public static MagicItemManager Instance
	{
		get
		{
			return SingletonMonoBehaviour.GetSingletonInstance<MagicItemManager>();
		}
	}

	public IEnumerable<PowerupData> UnlockedMagicItems
	{
		get
		{
			return _unlockedPowerups;
		}
	}

	public IEnumerable<PowerupData> LockedMagicItems
	{
		get
		{
			return _lockedPowerups;
		}
	}

	public int NumberOfUnlockedMagicItems
	{
		get
		{
			return _unlockedPowerups.Count;
		}
	}

	public int NumberOfLockedMagicItems
	{
		get
		{
			return _lockedPowerups.Count;
		}
	}

	private void OnEnable()
	{
		LevelManager.ArrivedAtNextRoom += HandleLevelManagerArrivedAtNextRoom;
		UpdateUnlockedMagicItemList();
		baseChanceToSpawn = Bedrock.GetRemoteVariableAsFloat("MagicItemSpawnChance", baseChanceToSpawn);
		maxChanceToSpawn = Bedrock.GetRemoteVariableAsFloat("MagicItemMaxSpawnChance", maxChanceToSpawn);
		magicItemSpawnLimitFactor.incrementAmount = Bedrock.GetRemoteVariableAsFloat("MagicItemLimitIncrementAmount", magicItemSpawnLimitFactor.incrementAmount);
		magicItemSpawnLimitFactor.stepsPerIncrement = Bedrock.GetRemoteVariableAsInt("MagicItemLimitStepsPerIncrement", magicItemSpawnLimitFactor.stepsPerIncrement);
		GameManager.GameOver += HandleGameManagerGameOver;
		Powerup.Triggered += HandlePowerupTriggered;
	}

	private void OnDisable()
	{
		LevelManager.ArrivedAtNextRoom -= HandleLevelManagerArrivedAtNextRoom;
		GameManager.GameOver -= HandleGameManagerGameOver;
		Powerup.Triggered -= HandlePowerupTriggered;
		if (_currentMagicItem != null)
		{
			UnityEngine.Object.Destroy(_currentMagicItem);
		}
	}

	private void HandlePowerupTriggered(object sender, PowerupEventArgs e)
	{
		magicItemSpawnLimitFactor.Step();
	}

	private void HandleGameManagerGameOver(object sender, EventArgs e)
	{
		base.enabled = false;
	}

	public void UpdateUnlockedMagicItemList()
	{
		_unlockedPowerups.Clear();
		_lockedPowerups.Clear();
		foreach (PowerupData powerup in powerups.powerups)
		{
			if (powerup.isCollectable)
			{
				_unlockedPowerups.Add(powerup);
			}
		}
	}

	private void HandleLevelManagerArrivedAtNextRoom(object sender, LevelManager.NextRoomEventArgs e)
	{
		if (ShouldSpawnMagicItem())
		{
			SpawnMagicItem();
		}
	}

	private bool ShouldSpawnMagicItem()
	{
		return false;
	}

	public void SpawnMagicItem()
	{
		PowerupData powerupData = ChooseMagicItem();
		if (powerupData == null)
		{
			Debug.LogWarning("We don't have any magic items unlocked. The magic item manager will not spawn anything until we do.");
		}
		else
		{
			SpawnMagicItem(powerupData);
		}
	}

	public GameObject SpawnMagicItem(PowerupData powerupToSpawn)
	{
		_activeItem = powerupToSpawn;
		GameObject[] array = GameObject.FindGameObjectsWithTag("MagicItemSpawnPoint");
		if (array.Length > 0)
		{
			Transform transform = array[UnityEngine.Random.Range(0, array.Length)].transform;
			if (_currentMagicItem != null)
			{
				UnityEngine.Object.Destroy(_currentMagicItem);
			}
			_currentMagicItem = UnityEngine.Object.Instantiate(magicItemCollectable, transform.position, transform.rotation) as GameObject;
			_currentMagicItem.transform.parent = LevelManager.Instance.currentScreenRoot.transform;
			_currentMagicItem.GetComponent<Mover>().direction = Vector3.Normalize(transform.localScale);
			MagicItemCollectable componentInChildren = _currentMagicItem.GetComponentInChildren<MagicItemCollectable>();
			componentInChildren.SetMagicItem(_activeItem);
		}
		return _currentMagicItem;
	}

	public bool IsPowerupUnlockedFromString(string storageName)
	{
		foreach (PowerupData powerup in powerups.powerups)
		{
			if (powerup.storageKey == storageName)
			{
				return true;
			}
		}
		return false;
	}

	public int GetPowerupLevelFromString(string storageName)
	{
		foreach (PowerupData powerup in powerups.powerups)
		{
			if (powerup.storageKey == storageName)
			{
				return powerup.GetLevel();
			}
		}
		return 0;
	}

	public PowerupData GetMagicItemFromStorageKey(string storageName)
	{
		foreach (PowerupData powerup in powerups.powerups)
		{
			if (powerup.storageKey == storageName)
			{
				return powerup;
			}
		}
		return null;
	}

	public PowerupData ChooseMagicItem()
	{
		return ChooseMagicItem(false);
	}

	public PowerupData ChooseMagicItem(bool lockedMagicItemsOnly)
	{
		if (!string.IsNullOrEmpty(DebugSettingsUI.magicItemToSpawn))
		{
			foreach (PowerupData powerup in powerups.powerups)
			{
				if (powerup.name == DebugSettingsUI.magicItemToSpawn)
				{
					return powerup;
				}
			}
			Debug.LogError("QA override set, but couldn't find powerup '" + DebugSettingsUI.magicItemToSpawn + "'.");
		}
		if (lockedMagicItemsOnly)
		{
			return ChooseRandomWithAffinity(_lockedPowerups);
		}
		return ChooseRandomWithAffinity(_unlockedPowerups);
	}

	private PowerupData ChooseRandomWithAffinity(IEnumerable<PowerupData> powerups)
	{
		MagicItemAffinityUpgrade passiveUpgradeOrDefault = CharacterUpgradeManager.Instance.GetPassiveUpgradeOrDefault<MagicItemAffinityUpgrade>();
		PowerupData powerupData = null;
		if (passiveUpgradeOrDefault != null)
		{
			powerupData = passiveUpgradeOrDefault.powerup;
			if (UnityEngine.Random.value <= passiveUpgradeOrDefault.percentAffinity)
			{
				return powerupData;
			}
		}
		List<PowerupData> list = new List<PowerupData>();
		foreach (PowerupData powerup in powerups)
		{
			list.Add(powerup);
		}
		if (powerupData != null)
		{
			list.Remove(powerupData);
			if (list.Count == 0)
			{
				return powerupData;
			}
		}
		return list.RandomOrDefault();
	}
}
