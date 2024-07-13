using System.Collections.Generic;
using UnityEngine;

public class DifficultyManager : SingletonMonoBehaviour
{
	public float healingElixirDifficultyAdjustment = 0.15f;

	public float globalDifficulty = 0.5f;

	public float currentGlobalDifficulty;

	public float bossRoomDifficultyPerWave = 0.125f;

	public float bossDifficultyEasy = 1.5f;

	public float difficultyIncreasePerRoom = 0.05f;

	public int roomsBeforeIncreasingDifficulty = 10;

	public int redTrollRoomThreshold = 50;

	public int redWizardRoomThreshold = 25;

	public int bombShipTrollRoomThreshold = 60;

	public float chanceToSpawnRedTroll = 0.5f;

	public float chanceToSpawnBombShipTroll = 0.1f;

	public float projectileTime;

	public float lobberProjectileTime;

	public float corkscrewProjectileTime;

	public float orbitSpeed;

	public float animatedMovementSpeed;

	public float shooterWaitTime;

	public float shooterChargeTime;

	public float wizardDizzyTime;

	public float wizardDodgeTime;

	public float wizardHardDizzyTime;

	public float wizardHardDodgeTime;

	public float shieldUpTime;

	public float shieldDownTime;

	public float shieldChargeTime;

	public float shieldFireTime;

	public float shieldAnimationSpeed;

	private float _maxProjectileTime = 3f;

	private float _minProjectileTime = 1f;

	private float _baseProjectileTime = 5f;

	private float _lobberProjectileAdjustment = 0.5f;

	private float _corkscrewProjectileAdjustment = 1f;

	private float _maxAnimatedMovementSpeed = 2f;

	private float _minAnimatedMovementSpeed = 0.6f;

	private float _minOrbitSpeed;

	private float _maxOrbitSpeed = 1E+09f;

	private float _baseAnimatedMovementSpeed = 0.6f;

	private float _maxShooterWaitTime = 2.5f;

	private float _minShooterWaitTime = 0.75f;

	private float _baseShooterWaitTime = 4f;

	private float _maxShooterChargeTime = 2f;

	private float _minShooterChargeTime = 0.75f;

	private float _baseShooterChargeTime = 3f;

	private float _minShieldTime;

	private float _maxShieldTime = 3f;

	private float _baseShieldCycleTime = 3f;

	private float _baseShieldFireTime = 2f;

	private float _baseShieldChargeTime = 1.5f;

	private float _minWizardDizzyTime = 1.5f;

	private float _maxWizardDizzyTime = 4f;

	private float _baseWizardDizzyTime = 4f;

	private float _minWizardDodgeTime = 0.2f;

	private float _maxWizardDodgeTime = 0.5f;

	private float _baseWizardDodgeTime = 0.5f;

	private float _baseWizardHardDizzyTime = 0.75f;

	private float _baseWizardHardDodgeTime = 0.1f;

	private float _wingedBootsDifficultyScale = 0.5f;

	private float _difficultyWhenEnteringBossRoom;

	public bool displayDifficulty;

	public static DifficultyManager Instance
	{
		get
		{
			return SingletonMonoBehaviour.GetSingletonInstance<DifficultyManager>();
		}
	}

	public float OrbitSpeed
	{
		get
		{
			return orbitSpeed;
		}
	}

	public float ProjectileTime
	{
		get
		{
			return projectileTime;
		}
	}

	public float LobberProjectileTime
	{
		get
		{
			return lobberProjectileTime;
		}
	}

	public float CorkscrewProjectileTime
	{
		get
		{
			return corkscrewProjectileTime;
		}
	}

	public float AnimatedMovementSpeed
	{
		get
		{
			return animatedMovementSpeed;
		}
	}

	public float ShooterWaitTime
	{
		get
		{
			return shooterWaitTime;
		}
	}

	public float ShooterChargeTime
	{
		get
		{
			return shooterChargeTime;
		}
	}

	public float WizardDizzyTime
	{
		get
		{
			return wizardDizzyTime;
		}
	}

	public float WizardDodgeTime
	{
		get
		{
			return wizardDodgeTime;
		}
	}

	public float WizardHardDizzyTime
	{
		get
		{
			return wizardHardDizzyTime;
		}
	}

	public float WizardHardDodgeTime
	{
		get
		{
			return wizardHardDodgeTime;
		}
	}

	public float ShieldUpTime
	{
		get
		{
			return shieldUpTime;
		}
	}

	public float ShieldDownTime
	{
		get
		{
			return shieldDownTime;
		}
	}

	public float ShieldFireTime
	{
		get
		{
			return shieldFireTime;
		}
	}

	public float ShieldChargeTime
	{
		get
		{
			return shieldChargeTime;
		}
	}

	public float ShieldAnimationSpeed
	{
		get
		{
			return shieldAnimationSpeed;
		}
	}

	public float InverseGlobalDifficulty
	{
		get
		{
			return 1f / currentGlobalDifficulty;
		}
	}

	public static bool IsBossDifficultyEasy
	{
		get
		{
			return Instance.currentGlobalDifficulty <= Instance.bossDifficultyEasy;
		}
	}

	public static bool ShouldSpawnRedTrolls
	{
		get
		{
			return LevelManager.Instance.RoomsCleared >= Instance.redTrollRoomThreshold && Random.value < Instance.chanceToSpawnRedTroll;
		}
	}

	public static bool ShouldSpawnRedWizard
	{
		get
		{
			return LevelManager.Instance.RoomsCleared >= Instance.redWizardRoomThreshold;
		}
	}

	public static bool ShouldSpawnBombShipTroll
	{
		get
		{
			if (BombShipTrollManager.Instance.Ship != null || WingedBoots.IsActive || RocketBooster.IsActive)
			{
				return false;
			}
			return LevelManager.Instance.RoomsCleared >= Instance.bombShipTrollRoomThreshold && Random.value < Instance.chanceToSpawnBombShipTroll;
		}
	}

	public static bool IsDifficultyGoingUp
	{
		get
		{
			int roomsCleared = LevelManager.Instance.RoomsCleared;
			if (roomsCleared > Instance.roomsBeforeIncreasingDifficulty && LevelManager.Instance.FinishedTutorials && !RocketBooster.IsActive && !WingedBoots.IsActive)
			{
				return true;
			}
			return false;
		}
	}

	private void Start()
	{
		UpdateValuesFromSwrve();
		currentGlobalDifficulty = globalDifficulty;
		UpdateValues();
	}

	public void UpdateValuesFromSwrve()
	{
		Dictionary<string, string> resourceDictionary = new Dictionary<string, string>();
		if (Bedrock.GetRemoteUserResources("DifficultyManager", out resourceDictionary))
		{
			globalDifficulty = Bedrock.GetFromResourceDictionaryAsFloat(resourceDictionary, "StartingDifficulty", globalDifficulty);
			healingElixirDifficultyAdjustment = Bedrock.GetFromResourceDictionaryAsFloat(resourceDictionary, "HealingElixirDifficultyAdjustment", healingElixirDifficultyAdjustment);
			bossRoomDifficultyPerWave = Bedrock.GetFromResourceDictionaryAsFloat(resourceDictionary, "BossRoomDifficultyPerWave", bossRoomDifficultyPerWave);
			bossDifficultyEasy = Bedrock.GetFromResourceDictionaryAsFloat(resourceDictionary, "BossDifficultyEasy", bossDifficultyEasy);
			difficultyIncreasePerRoom = Bedrock.GetFromResourceDictionaryAsFloat(resourceDictionary, "DifficultyIncreasePerRoom", difficultyIncreasePerRoom);
			chanceToSpawnRedTroll = Bedrock.GetFromResourceDictionaryAsFloat(resourceDictionary, "ChanceToSpawnRedTroll", chanceToSpawnRedTroll);
			chanceToSpawnBombShipTroll = Bedrock.GetFromResourceDictionaryAsFloat(resourceDictionary, "ChanceToSpawnBombShipTroll", chanceToSpawnBombShipTroll);
			_maxProjectileTime = Bedrock.GetFromResourceDictionaryAsFloat(resourceDictionary, "MaxProjectileTime", _maxProjectileTime);
			_minProjectileTime = Bedrock.GetFromResourceDictionaryAsFloat(resourceDictionary, "MinProjectileTime", _minProjectileTime);
			_baseProjectileTime = Bedrock.GetFromResourceDictionaryAsFloat(resourceDictionary, "BaseProjectileTime", _baseProjectileTime);
			_lobberProjectileAdjustment = Bedrock.GetFromResourceDictionaryAsFloat(resourceDictionary, "LobberProjectileAdjustment", _lobberProjectileAdjustment);
			_corkscrewProjectileAdjustment = Bedrock.GetFromResourceDictionaryAsFloat(resourceDictionary, "CorkscrewProjectileAdjustment", _corkscrewProjectileAdjustment);
			_maxAnimatedMovementSpeed = Bedrock.GetFromResourceDictionaryAsFloat(resourceDictionary, "MaxAnimatedMovementSpeed", _maxAnimatedMovementSpeed);
			_minAnimatedMovementSpeed = Bedrock.GetFromResourceDictionaryAsFloat(resourceDictionary, "MinAnimatedMovementSpeed", _minAnimatedMovementSpeed);
			_minOrbitSpeed = Bedrock.GetFromResourceDictionaryAsFloat(resourceDictionary, "MinOrbitSpeed", _minOrbitSpeed);
			_maxOrbitSpeed = Bedrock.GetFromResourceDictionaryAsFloat(resourceDictionary, "MaxOrbitSpeed", _maxOrbitSpeed);
			_baseAnimatedMovementSpeed = Bedrock.GetFromResourceDictionaryAsFloat(resourceDictionary, "BaseAnimatedMovementSpeed", _baseAnimatedMovementSpeed);
			_maxShooterWaitTime = Bedrock.GetFromResourceDictionaryAsFloat(resourceDictionary, "MaxShooterWaitTime", _maxShooterWaitTime);
			_minShooterWaitTime = Bedrock.GetFromResourceDictionaryAsFloat(resourceDictionary, "MinShooterWaitTime", _minShooterWaitTime);
			_baseShooterWaitTime = Bedrock.GetFromResourceDictionaryAsFloat(resourceDictionary, "BaseShooterWaitTime", _baseShooterWaitTime);
			_maxShooterChargeTime = Bedrock.GetFromResourceDictionaryAsFloat(resourceDictionary, "MaxShooterChargeTime", _maxShooterChargeTime);
			_minShooterChargeTime = Bedrock.GetFromResourceDictionaryAsFloat(resourceDictionary, "MinShooterChargeTime", _minShooterChargeTime);
			_baseShooterChargeTime = Bedrock.GetFromResourceDictionaryAsFloat(resourceDictionary, "BaseShooterChargeTime", _baseShooterChargeTime);
			_minShieldTime = Bedrock.GetFromResourceDictionaryAsFloat(resourceDictionary, "MinShieldTime", _minShieldTime);
			_maxShieldTime = Bedrock.GetFromResourceDictionaryAsFloat(resourceDictionary, "MaxShieldTime", _maxShieldTime);
			_baseShieldCycleTime = Bedrock.GetFromResourceDictionaryAsFloat(resourceDictionary, "BaseShieldCycleTime", _baseShieldCycleTime);
			_baseShieldFireTime = Bedrock.GetFromResourceDictionaryAsFloat(resourceDictionary, "BaseShieldFireTime", _baseShieldFireTime);
			_baseShieldChargeTime = Bedrock.GetFromResourceDictionaryAsFloat(resourceDictionary, "BaseShieldChargeTime", _baseShieldChargeTime);
			_minWizardDizzyTime = Bedrock.GetFromResourceDictionaryAsFloat(resourceDictionary, "MinWizardDizzyTime", _minWizardDizzyTime);
			_maxWizardDizzyTime = Bedrock.GetFromResourceDictionaryAsFloat(resourceDictionary, "MaxWizardDizzyTime", _maxWizardDizzyTime);
			_baseWizardDizzyTime = Bedrock.GetFromResourceDictionaryAsFloat(resourceDictionary, "BaseWizardDizzyTime", _baseWizardDizzyTime);
			_minWizardDodgeTime = Bedrock.GetFromResourceDictionaryAsFloat(resourceDictionary, "MinWizardDodgeTime", _minWizardDodgeTime);
			_maxWizardDodgeTime = Bedrock.GetFromResourceDictionaryAsFloat(resourceDictionary, "MaxWizardDodgeTime", _maxWizardDodgeTime);
			_baseWizardDodgeTime = Bedrock.GetFromResourceDictionaryAsFloat(resourceDictionary, "BaseWizardDodgeTime", _baseWizardDodgeTime);
			_baseWizardHardDizzyTime = Bedrock.GetFromResourceDictionaryAsFloat(resourceDictionary, "BaseWizardHardDizzyTime", _baseWizardHardDizzyTime);
			_baseWizardHardDodgeTime = Bedrock.GetFromResourceDictionaryAsFloat(resourceDictionary, "BaseWizardHardDoodgeTime", _baseWizardHardDodgeTime);
			roomsBeforeIncreasingDifficulty = Bedrock.GetFromResourceDictionaryAsInt(resourceDictionary, "RoomsBeforeIncreasingDifficulty", roomsBeforeIncreasingDifficulty);
			redTrollRoomThreshold = Bedrock.GetFromResourceDictionaryAsInt(resourceDictionary, "RedTrollRoomThreshold", redTrollRoomThreshold);
			redWizardRoomThreshold = Bedrock.GetFromResourceDictionaryAsInt(resourceDictionary, "RedWizardRoomThreshold", redWizardRoomThreshold);
			bombShipTrollRoomThreshold = Bedrock.GetFromResourceDictionaryAsInt(resourceDictionary, "BombShipTrollRoomThreshold", bombShipTrollRoomThreshold);
		}
	}

	public void UpdateDifficulty()
	{
		if (IsDifficultyGoingUp)
		{
			if (WingedBoots.IsActive)
			{
				currentGlobalDifficulty += difficultyIncreasePerRoom * _wingedBootsDifficultyScale;
			}
			else
			{
				currentGlobalDifficulty += difficultyIncreasePerRoom;
			}
		}
		UpdateValues();
	}

	public void DifficultyUp()
	{
		currentGlobalDifficulty += difficultyIncreasePerRoom;
		UpdateValues();
	}

	private void UpdateValues()
	{
		float num = 1f / currentGlobalDifficulty;
		orbitSpeed = Mathf.Clamp(60f * (currentGlobalDifficulty + _baseAnimatedMovementSpeed), _minOrbitSpeed, _maxOrbitSpeed);
		animatedMovementSpeed = Mathf.Clamp(currentGlobalDifficulty * _baseAnimatedMovementSpeed, _minAnimatedMovementSpeed, _maxAnimatedMovementSpeed);
		projectileTime = Mathf.Clamp(_baseProjectileTime / (currentGlobalDifficulty + 0.5f), _minProjectileTime, _maxProjectileTime);
		lobberProjectileTime = projectileTime + _lobberProjectileAdjustment;
		corkscrewProjectileTime = projectileTime + _corkscrewProjectileAdjustment;
		shooterWaitTime = Mathf.Clamp(_baseShooterWaitTime * num, _minShooterWaitTime, _maxShooterWaitTime);
		shooterChargeTime = Mathf.Clamp(_baseShooterChargeTime * num, _minShooterChargeTime, _maxShooterChargeTime);
		wizardDizzyTime = Mathf.Clamp(_baseWizardDizzyTime * num, _minWizardDizzyTime, _maxWizardDizzyTime);
		wizardDodgeTime = Mathf.Clamp(_baseWizardDodgeTime * num, _minWizardDodgeTime, _maxWizardDodgeTime);
		wizardHardDizzyTime = _baseWizardHardDizzyTime;
		wizardHardDodgeTime = _baseWizardHardDodgeTime;
		shieldUpTime = Mathf.Clamp(currentGlobalDifficulty, _minShieldTime, _maxShieldTime);
		shieldDownTime = _baseShieldCycleTime - shieldUpTime;
		shieldFireTime = shooterWaitTime;
		shieldChargeTime = shooterChargeTime;
		shieldAnimationSpeed = Mathf.Clamp(currentGlobalDifficulty, 1f, currentGlobalDifficulty);
	}

	public void StartBossRoom()
	{
		_difficultyWhenEnteringBossRoom = currentGlobalDifficulty;
		UpdateValues();
	}

	public void IncrementBossDifficulty()
	{
		currentGlobalDifficulty += bossRoomDifficultyPerWave;
		UpdateValues();
	}

	public void EndBossRoom()
	{
		currentGlobalDifficulty = _difficultyWhenEnteringBossRoom;
		UpdateValues();
	}

	public void HealingElixirDifficultyDecrease()
	{
		currentGlobalDifficulty -= healingElixirDifficultyAdjustment;
		UpdateValues();
	}

	private void OnGUI()
	{
		if (displayDifficulty)
		{
			GUI.backgroundColor = Color.black;
			GUI.color = Color.white;
			GUI.Box(new Rect((float)Screen.width - 100f, (float)Screen.height - 30f, 100f, 30f), "Difficulty: " + currentGlobalDifficulty.ToString("F2"));
		}
	}
}
