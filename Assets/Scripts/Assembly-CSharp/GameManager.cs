using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : SingletonMonoBehaviour
{
	public enum GameState
	{
		OutOfGame = 0,
		Playing = 1,
		Dying = 2,
		Dead = 3
	}

	public class GameStateChangedEventArgs : EventArgs
	{
		public GameState NewState { get; private set; }

		public GameState OldState { get; private set; }

		public GameStateChangedEventArgs(GameState newState, GameState oldState)
		{
			NewState = newState;
			OldState = oldState;
		}
	}

	public const int GUN_SLOT_MAX = 6;

	private static readonly ILogger _log = LogBuilder.Instance.GetLogger(typeof(GameManager), LogLevel.Debug);

	public static int killCount = 0;

	public static SessionStatistics sessionStats;

	public static bool autoDropFuel = false;

	public static float fuelDropRate = 1f;

	public static int gunSlotCount = 6;

	public static int GUN_SLOT_MIN = 6;

	public static int timeCount = 0;

	public static int timePrevious = 0;

	public static int timeRecord = 0;

	public static int spawnCount = 0;

	public static float startingHealth = 1f;

	public static float currentHealth = 1f;

	public static bool gameStarted = false;

	public static bool invincible = false;

	private static int _currentScore;

	public static int highScore = 0;

	public static int roomsCount = 0;

	public static int roomsPassed = 0;

	public static int tutorialRooms = 3;

	public static int moneyCollectedInVoyage;

	public static int moneyAwardedForBonus;

	public static int skylandersUnlockedForBonus;

	public static int gemsCollectedInVoyage;

	public static int projsHitInVoyage;

	public static int redBombsHitInVoyage;

	public static int bombsHitInVoyage;

	public static int bombsPtsLostInVoyage;

	public static float globalDifficultyDecreaseFactor = 0.8f;

	public static float BonusFactorPerSkylander = 0.25f;

	public static int coinsToPtsFactor = 10;

	private static GameState _gameState = GameState.OutOfGame;

	public static bool debugMode = true;

	public Transform endGameScreen;

	public PowerupList powerupsToPreload;

	public SoundEventData m_GameOverSound;

	public int m_MaxContinue;

	private int m_ContinueCount;

	private Stack<PauseReason> _pauseReasons = new Stack<PauseReason>();

	private bool _isPaused;

	private float _timeScaleBeforePause = -1f;

	private static bool _didComboInCurrentRoom = false;

	private static int _roomCountNoCombo = 0;

	public VersionData versionData;

	public static FullScreenFX goFlash;

	public static GameManager Instance
	{
		get
		{
			return SingletonMonoBehaviour.GetSingletonInstance<GameManager>();
		}
	}

	public static int currentScore
	{
		get
		{
			return _currentScore;
		}
		set
		{
			if (value != _currentScore)
			{
				int delta = value - _currentScore;
				_currentScore = value;
				OnScoreChanged(delta);
			}
		}
	}

	public static GameState gameState
	{
		get
		{
			return _gameState;
		}
		set
		{
			if (_gameState != value)
			{
				GameState previousGameState = _gameState;
				_gameState = value;
				OnGameStateChanged(previousGameState);
			}
		}
	}

	public int ContinueCount
	{
		get
		{
			return m_ContinueCount;
		}
	}

	public bool IsGameOver
	{
		get
		{
			return gameState == GameState.Dead;
		}
	}

	public bool IsPaused
	{
		get
		{
			return _isPaused;
		}
	}

	public PauseReason LastPauseReason
	{
		get
		{
			return _pauseReasons.Peek();
		}
	}

	public static event EventHandler<IntegerChangeEventArgs> MoneyCollected;

	public static event EventHandler<EventArgs> GameStarted;

	public static event EventHandler<GameStateChangedEventArgs> GameStateChanged;

	public static event EventHandler<EventArgs> GameOver;

	public static event EventHandler<EventArgs> Revived;

	public static event EventHandler<EventArgs> PlayerTookDamage;

	public static event EventHandler<EventArgs> InvincibilityInvoked;

	public static event EventHandler<PauseChangeEventArgs> PauseChanged;

	public static event EventHandler<PauseStackChangeEventArgs> PauseStackChanged;

	public static event EventHandler<EventArgs> GameManagerLoaded;

	public static void EnemyKilled()
	{
		killCount++;
	}

	public static void EnemySpawned()
	{
		spawnCount++;
	}

	public static void RoomCleared()
	{
		roomsPassed++;
		roomsCount = Math.Max(roomsPassed - tutorialRooms, 0);
		if (!_didComboInCurrentRoom)
		{
			_roomCountNoCombo++;
		}
		if (_roomCountNoCombo >= 20)
		{
			AchievementManager.Instance.SetStep(Achievements.ComboNone, 1);
		}
		_didComboInCurrentRoom = false;
	}

	public static void GotMoney(int delta)
	{
		moneyCollectedInVoyage += delta;
		ScoreKeeper.Instance.AddScore(delta * coinsToPtsFactor);
		OnMoneyCollected(delta, moneyCollectedInVoyage);
	}

	private static void OnMoneyCollected(int delta, int finalAmount)
	{
		if (GameManager.MoneyCollected != null)
		{
			GameManager.MoneyCollected(null, new IntegerChangeEventArgs(delta, finalAmount));
		}
	}

	protected override void AwakeOnce()
	{
		m_ContinueCount = 0;
		base.AwakeOnce();
		Resources.UnloadUnusedAssets();
		CharacterAudioResources audioResources = StartGameSettings.Instance.activeSkylander.AudioResources;
		if (audioResources != null)
		{
		}
		_gameState = GameState.OutOfGame;
	}

	public void Start()
	{
		gameState = GameState.Playing;
	}

	private void OnEnable()
	{
		Shooter.ComboCompleted += OnComboCompleted;
	}

	private void OnDisable()
	{
		Shooter.ComboCompleted -= OnComboCompleted;
	}

	public void StartGame()
	{
		Bedrock.AnalyticsLogEvent("GameTime", true);
		gameState = GameState.Playing;
		gameStarted = true;
		OnGameStarted();
	}

	protected void OnGameStarted()
	{
		ScreenTimeoutUtility.Instance.AllowTimeout = false;
		if (GameManager.GameStarted != null)
		{
			GameManager.GameStarted(this, new EventArgs());
		}
	}

	public static void ResetData()
	{
		sessionStats = new SessionStatistics();
		killCount = 0;
		timeCount = 0;
		spawnCount = 0;
		moneyCollectedInVoyage = 0;
		moneyAwardedForBonus = 0;
		skylandersUnlockedForBonus = 0;
		gemsCollectedInVoyage = 0;
		roomsCount = 0;
		roomsPassed = 0;
		gunSlotCount = GUN_SLOT_MIN;
		currentHealth = startingHealth;
		gameStarted = false;
		_didComboInCurrentRoom = false;
		_roomCountNoCombo = 0;
		currentScore = 0;
		HealingElixir.Reset();
	}

	protected static void OnGameStateChanged(GameState previousGameState)
	{
		if (GameManager.GameStateChanged != null)
		{
			GameManager.GameStateChanged(null, new GameStateChangedEventArgs(gameState, previousGameState));
		}
	}

	public void Continue()
	{
		m_ContinueCount++;
		gameState = GameState.Playing;
		currentHealth = 1f;
		sessionStats.deathAI = "None";
		ShipManager.instance.ResetTargetting();
		LevelManager.Instance.RoomTime = LevelManager.Instance.m_MaxRoomTime;
		HealthBar.Instance.Continue();
		OnRevived();
	}

	public static void ResetGame(bool retry)
	{
		ResetData();
		if (retry)
		{
			Debug.Log("Loading game...");
			Application.LoadLevel("GameLoad");
		}
		else
		{
			Debug.Log("Loading loadout...");
			Application.LoadLevel("Loadout");
		}
	}

	public static void KillAllProjectiles()
	{
		GameObject[] array = GameObject.FindGameObjectsWithTag("Projectile");
		GameObject[] array2 = array;
		foreach (GameObject obj in array2)
		{
			UnityEngine.Object.Destroy(obj);
		}
	}

	public static void HitAllEnemies()
	{
		List<Health> list = new List<Health>();
		list.AddRange(UnityEngine.Object.FindObjectsOfType(typeof(Health)).Cast<Health>());
		DamageInfo damageInfo = new DamageInfo();
		damageInfo.damageAmount = 100;
		foreach (Health item in list)
		{
			if (item != null && item.isEnemy)
			{
				item.TakeHit(damageInfo);
			}
		}
		list.Clear();
	}

	public static void KillAllEnemies()
	{
		List<Health> list = new List<Health>();
		list.AddRange(UnityEngine.Object.FindObjectsOfType(typeof(Health)).Cast<Health>());
		foreach (Health item in list)
		{
			if (item != null && item.isEnemy)
			{
				item.noKill = false;
				item.Kill();
			}
		}
		list.Clear();
	}

	public static void ExplodeBombs()
	{
		GameObject[] array = GameObject.FindGameObjectsWithTag("Bomb");
		GameObject[] array2 = array;
		foreach (GameObject gameObject in array2)
		{
			gameObject.SendMessage("Cleanup", SendMessageOptions.DontRequireReceiver);
		}
	}

	public static void ExplodeProjectiles()
	{
		GameObject[] array = GameObject.FindGameObjectsWithTag("Projectile");
		GameObject[] array2 = array;
		foreach (GameObject gameObject in array2)
		{
			gameObject.SendMessage("Cleanup", SendMessageOptions.DontRequireReceiver);
		}
	}

	public void ShowGameOverScreen()
	{
		KillAllProjectiles();
		gameState = GameState.Dying;
		VictoryDance();
		ShipManager.instance.DisableTargetting();
		SoundEventManager.Instance.Play2D(m_GameOverSound);
		TransitionController.Instance.StartCloudTransition();
		InvokeHelper.InvokeSafe(ResultsController.Instance.ShowResultScreen, 0.5f, ResultsController.Instance);
	}

	public void FinishGame(bool doEndgameFanfare)
	{
		if (StartGameSettings.Instance.IsBonusElementActive)
		{
			float num = BonusFactorPerSkylander * (float)moneyCollectedInVoyage;
			moneyAwardedForBonus = Convert.ToInt32(Math.Round((float)(skylandersUnlockedForBonus = 0) * num));
		}
		int num2 = moneyCollectedInVoyage + moneyAwardedForBonus;
		SwrveEventsRewards.AwardCoins(moneyCollectedInVoyage, "CollectedInFlight");
		SwrveEventsRewards.AwardCoins(moneyAwardedForBonus, "ElementOfTheDayBonus");
		AchievementManager.Instance.IncrementStepBy(Achievements.CoinsEarn, num2);
		if (num2 > 3000)
		{
			AchievementManager.Instance.SetStep(Achievements.CoinsMedium, 1);
		}
		if (num2 > 7500)
		{
			AchievementManager.Instance.SetStep(Achievements.CoinsLarge, 1);
		}
		Bedrock.AnalyticsEndTimedEvent("GameTime");
		SwrveEventsGameplay.GameEnd();
		AchievementManager.Instance.SyncAchievements();
		if (!ScoreKeeper.newHighScore)
		{
		}
	}

	protected void OnGameOver()
	{
		if (GameManager.GameOver != null)
		{
			GameManager.GameOver(this, new EventArgs());
		}
		foreach (PowerupData powerup in powerupsToPreload.powerups)
		{
			powerup.ReleasePowerupPrefab();
		}
	}

	public void ForceGameOverEvent()
	{
		OnGameOver();
	}

	private void OnComboCompleted(object sender, Shooter.ComboCompletedEventArgs args)
	{
		if (args.Number > 1)
		{
			_didComboInCurrentRoom = true;
		}
		if (args.Number == 6)
		{
			AchievementManager.Instance.IncrementStep(Achievements.ComboMaxSmall);
			AchievementManager.Instance.IncrementStep(Achievements.ComboMaxLarge);
		}
	}

	public void Update()
	{
		if (Input.GetKeyDown(KeyCode.KeypadPlus))
		{
			_log.LogDebug("Speeding up time by 0.1 seconds.");
			Time.timeScale = Mathf.Clamp(Time.timeScale + 0.1f, 0f, 5f);
		}
		if (Input.GetKeyDown(KeyCode.KeypadMinus))
		{
			_log.LogDebug("Slowing down time by 0.1 seconds.");
			Time.timeScale = Mathf.Clamp01(Time.timeScale - 0.1f);
		}
		if (Input.GetKeyDown(KeyCode.KeypadMultiply))
		{
			_log.LogDebug("Restoring normal time.");
			Time.timeScale = 1f;
		}
		if (Input.GetKeyDown(KeyCode.KeypadDivide))
		{
			_log.LogDebug("Setting minimum time scale.");
			Time.timeScale = 0.1f;
		}
		if (gameState == GameState.Dead)
		{
			Application.LoadLevel("ElementSelect");
			_gameState = GameState.OutOfGame;
		}
	}

	public static void HurtPlayer(float amount)
	{
		if (invincible)
		{
			Debug.Log("Tried to kill player but he was invincible");
			OnInvincibilityInvoked();
		}
		else if (HealingElixirScreen.IsActive)
		{
			Debug.LogWarning("Player shouldn't be hurt while the healing elixir screen is up.  Are projectiles still firing?");
		}
		else
		{
			CameraShake();
			SoundEventManager.Instance.Play2D(GlobalSoundEventData.Instance.PlayerTakeHit_SFX);
		}
	}

	public static void VictoryDance()
	{
		GameObject[] array = GameObject.FindGameObjectsWithTag("Enemy");
		GameObject[] array2 = array;
		foreach (GameObject gameObject in array2)
		{
			gameObject.SendMessage("VictoryDance", SendMessageOptions.DontRequireReceiver);
		}
	}

	private void OnRevived()
	{
		if (GameManager.Revived != null)
		{
			GameManager.Revived(null, new EventArgs());
		}
	}

	protected static void OnPlayerTookDamage()
	{
		if (GameManager.PlayerTookDamage != null)
		{
			GameManager.PlayerTookDamage(null, new EventArgs());
		}
	}

	protected static void OnInvincibilityInvoked()
	{
		if (GameManager.InvincibilityInvoked != null)
		{
			GameManager.InvincibilityInvoked(null, new EventArgs());
		}
	}

	public static void CameraShake()
	{
		CameraShake(Camera.main);
	}

	public static void CameraShake(Camera cam)
	{
		cam.GetComponent<Animation>().Play("cameraShake");
	}

	public static void CameraShakeLoopStart()
	{
		Camera.main.GetComponent<Animation>().Play("cameraShakeLoop");
	}

	public static void CameraShakeLoopStop()
	{
		Camera.main.GetComponent<Animation>().Stop();
	}

	private void OnPauseChanged(PauseReason reason)
	{
		if (GameManager.PauseChanged != null)
		{
			GameManager.PauseChanged(this, new PauseChangeEventArgs(reason));
		}
	}

	private void OnPauseStackChanged(PauseReason reason, bool wasPush)
	{
		if (GameManager.PauseStackChanged != null)
		{
			PauseStackChangeEventArgs e = new PauseStackChangeEventArgs(reason, wasPush);
			GameManager.PauseStackChanged(this, e);
		}
	}

	public bool IsPauseReasonInStack(PauseReason reason)
	{
		return _pauseReasons.Contains(reason);
	}

	public void PushPause(PauseReason reason)
	{
		_log.LogDebug("PushPause({0})", reason);
		bool flag = _pauseReasons.Any();
		_pauseReasons.Push(reason);
		OnPauseStackChanged(reason, true);
		if (!flag)
		{
			SetPaused(true, reason);
		}
	}

	public void PopPause(PauseReason reason)
	{
		_log.LogDebug("PopPause({0})", reason);
		if (!_pauseReasons.Any())
		{
			throw new InvalidOperationException("Cannot pop pause state - the pause stack is empty.");
		}
		PauseReason pauseReason = _pauseReasons.Peek();
		if (pauseReason != reason)
		{
			throw new InvalidOperationException(string.Concat("Cannot pop pause state '", reason, "', it does not match the last pause reason (", pauseReason, ")"));
		}
		_pauseReasons.Pop();
		OnPauseStackChanged(reason, false);
		if (!_pauseReasons.Any())
		{
			SetPaused(false, pauseReason);
		}
	}

	private void SetPaused(bool paused, PauseReason reason)
	{
	}

	public void OnGameManagerLoaded()
	{
		if (GameManager.GameManagerLoaded != null)
		{
			GameManager.GameManagerLoaded(this, new EventArgs());
		}
	}

	private static void OnScoreChanged(int delta)
	{
	}
}
