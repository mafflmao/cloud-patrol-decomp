using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelManager : SingletonMonoBehaviour
{
	[Serializable]
	public class RoomData
	{
		public RoomGroup m_RoomGroup;

		public int m_ScoreNeededForDiffUp;
	}

	[Serializable]
	public class DifficultyData
	{
		public List<RoomData> m_RoomData;
	}

	public class RoomClearEventArgs : EventArgs
	{
		public GameObject RootNode { get; private set; }

		public RoomClearEventArgs(GameObject roomRootNode)
		{
			RootNode = roomRootNode;
		}
	}

	public class NextRoomEventArgs : EventArgs
	{
		private LevelManager _manager;

		public bool IsDelayed { get; private set; }

		public ScreenRootNodeData RoomRoot { get; private set; }

		public Vector3 CameraPosition { get; private set; }

		public ScreenManager ScreenManager { get; private set; }

		public MoveDirections MoveDirection { get; private set; }

		public NextRoomEventArgs(LevelManager manager, ScreenRootNodeData roomRoot, Vector3 cameraPosition, ScreenManager screenManager, MoveDirections moveDirection)
		{
			_manager = manager;
			RoomRoot = roomRoot;
			CameraPosition = cameraPosition;
			ScreenManager = screenManager;
			MoveDirection = moveDirection;
		}

		public void DelayMove()
		{
			Debug.Log("Delaying move!");
			IsDelayed = true;
		}

		public void ResumeMove()
		{
			_manager.MoveToNextRoom(this);
		}
	}

	public enum MoveDirections
	{
		Up = 0,
		Down = 1,
		Right = 2
	}

	private class TutorialQueueEntry
	{
		public RoomGroup RoomGroup { get; private set; }

		public EnemyTypes EnemyType { get; private set; }

		public TutorialQueueEntry(RoomGroup roomGroup, EnemyTypes enemyType)
		{
			RoomGroup = roomGroup;
			EnemyType = enemyType;
		}
	}

	private class IntegerTutorialComparer : IComparer<EnemyTypes>
	{
		public int Compare(EnemyTypes x, EnemyTypes y)
		{
			int num = (int)x;
			return num.CompareTo((int)y);
		}
	}

	private const float HorizontalRoomSpacing = -15f;

	private const float VerticalRoomSpacing = 10f;

	private const int NumberOfHorizontalTilesInBackground = 3;

	private const int NumberOfVerticalTilesInBackground = 3;

	private const float BackgroundOffsetInZ = -8.5f;

	private const float SeparationBetweenBackgroundPanels = -7.5f;

	private const float InitialBackgroundOffset = -15f;

	private const float VeritcalBackgroundOffset = 10f;

	public float m_MaxRoomTime;

	public float m_FinalStageTime = 25f;

	public SpriteText m_RoomTimeText;

	public int m_CoinBaseScore = 50;

	public List<DifficultyData> m_DifficultyRoomGroup;

	public List<Texture2D> m_SkyTexture;

	public Room m_ShortVersionHideOut;

	private Texture2D m_CurSkyTexture;

	private float m_RoomTime;

	private bool m_CanGoToNextRoom = true;

	private bool m_FirstRoomPassed;

	private bool m_DifficultyIncreased;

	private int m_CurDifficultyIndex;

	private int m_CurRoomIndex;

	private int m_RoomClearedCount;

	public PowerupData powerupToUnlockAfterTutorialsComplete;

	public SteppedCounter roomClearDelay = new SteppedCounter();

	private static readonly IntegerTutorialComparer TutorialSortOrder = new IntegerTutorialComparer();

	public Level[] levels;

	private int _currentLevelNumber = -1;

	public TutorialData tutorialData;

	private MoveDirections lastMovedDirection;

	private int roomsMovedUp;

	private int roomsMovedRight;

	private int currentRoomNumber;

	private GameObject mainCameraParent;

	private int _currentRoundNumber = -1;

	private GameObject oldBackground;

	private GameObject newBackground;

	private int _loadedBackgroundCount;

	[NonSerialized]
	public GameObject currentScreenRoot;

	private GameObject oldScreenRoot;

	public string levelOverride;

	private Dictionary<EnemyTypes, bool> _tutorialPlayed = new Dictionary<EnemyTypes, bool>();

	private Queue<string> _startGameSettingsLevelQueue = new Queue<string>();

	private Queue<string> _gameplayTutorials = new Queue<string>();

	[NonSerialized]
	private string _sceneToLoadAfterTutorialsComplete;

	private string loadingSceneName;

	private float _loadingStartTime;

	private bool activateFirstRoomOnLoad;

	private NextRoomEventArgs _movingArgs;

	public static LevelManager Instance
	{
		get
		{
			return SingletonMonoBehaviour.GetSingletonInstance<LevelManager>();
		}
	}

	public float RoomTime
	{
		get
		{
			return m_RoomTime;
		}
		set
		{
			m_RoomTime = Mathf.Max(value, 0f);
		}
	}

	public bool FirstRoomPassed
	{
		get
		{
			return m_FirstRoomPassed;
		}
	}

	public Texture2D CurSkyTexture
	{
		get
		{
			return m_SkyTexture[m_CurDifficultyIndex];
		}
	}

	public bool DiffIncreased
	{
		get
		{
			return m_DifficultyIncreased;
		}
		set
		{
			m_DifficultyIncreased = value;
		}
	}

	public int CurDifficultyIndex
	{
		get
		{
			return m_CurDifficultyIndex;
		}
	}

	public int DifficultyCount
	{
		get
		{
			return m_DifficultyRoomGroup.Count;
		}
	}

	public Level CurrentLevel
	{
		get
		{
			return levels[_currentLevelNumber];
		}
	}

	public bool IsTransitioning { get; private set; }

	public bool FinishedTutorials
	{
		get
		{
			return true;
		}
	}

	public ScreenManager CurrentScreenManager { get; private set; }

	private int RoomColumnRelativeToBackground
	{
		get
		{
			return roomsMovedRight % 3;
		}
	}

	public int RoomsCleared { get; private set; }

	public static event EventHandler<RoomClearEventArgs> RoomClear;

	public static event EventHandler<NextRoomEventArgs> MovingToNextRoom;

	public static event EventHandler<NextRoomEventArgs> ArrivedAtNextRoom;

	public static event EventHandler BackgroundLoadComplete;

	public static event EventHandler DifficultyUp;

	public static event EventHandler<EventArgs> LevelChanged;

	private void Start()
	{
		MusicManager.Instance.StopMusic();
		m_CanGoToNextRoom = true;
		m_FirstRoomPassed = false;
		RoomTime = m_MaxRoomTime;
		m_CurDifficultyIndex = 0;
		m_CurRoomIndex = 0;
		Level[] array = levels;
		foreach (Level level in array)
		{
			level.InitializeRuntime();
			foreach (RoomGroup room in level.rooms)
			{
				room.Reset();
			}
		}
		foreach (EnemyTypes item2 in Enum.GetValues(typeof(EnemyTypes)).Cast<EnemyTypes>())
		{
			_tutorialPlayed[item2] = true;
		}
		mainCameraParent = GameObject.Find("!MainCamera");
		if (mainCameraParent == null)
		{
			Debug.LogError("There's no !MainCamera in the scene. This is a problem");
		}
		if (StartGameSettings.Instance.sceneOverrides != null && StartGameSettings.Instance.sceneOverrides.Any())
		{
			string[] sceneOverrides = StartGameSettings.Instance.sceneOverrides;
			foreach (string item in sceneOverrides)
			{
				_startGameSettingsLevelQueue.Enqueue(item);
			}
		}
		AdvanceToNextLevel();
		if (!FinishedTutorials)
		{
			foreach (string scene in tutorialData.gameplayTutorials.Scenes)
			{
				_gameplayTutorials.Enqueue(scene);
			}
		}
		RoomsCleared = -1;
		AdvanceToNextRound();
		LoadNextBackgroundAsync();
		GoToNextLevelAsync();
	}

	private void OnEnable()
	{
		TransitionController.GameOverTransitionComplete += HandleTransitionControllerGameOverTransitionComplete;
		MoverWithSpeed.MoveComplete += HandleCameraMoverMoveComplete;
	}

	private void OnDisable()
	{
		TransitionController.GameOverTransitionComplete -= HandleTransitionControllerGameOverTransitionComplete;
		MoverWithSpeed.MoveComplete -= HandleCameraMoverMoveComplete;
	}

	private void Update()
	{
		if (GameManager.gameState == GameManager.GameState.Playing)
		{
			RoomTime -= Time.deltaTime;
			if (RoomTime <= 0f)
			{
				GoToNextLevelAsync();
			}
		}
	}

	private void HandleTransitionControllerGameOverTransitionComplete(object sender, EventArgs e)
	{
	}

	private void AdvanceToNextLevel()
	{
		_currentLevelNumber++;
		if (CurrentLevel.firstRoom != null)
		{
			_gameplayTutorials.Enqueue(CurrentLevel.firstRoom.sceneName);
		}
		OnLevelChanged();
	}

	public void GoToNextLevelAsync()
	{
		if (!m_FirstRoomPassed || m_CanGoToNextRoom)
		{
			m_CanGoToNextRoom = false;
			StartCoroutine(GoToNextLevel());
		}
	}

	public void RoomCleared()
	{
		if (currentScreenRoot != null)
		{
			OnRoomClear(currentScreenRoot);
		}
	}

	private void OnRoomClear(GameObject roomRootNode)
	{
		RoomsCleared++;
		GameObject gameObject = Instance.currentScreenRoot;
		if (gameObject != null && CurrentLevel.GetDifficultyForRoom(gameObject.name) == Difficulty.Boss)
		{
			AchievementManager.Instance.IncrementStep(Achievements.BossSmall);
			AchievementManager.Instance.IncrementStep(Achievements.BossLarge);
		}
		if (LevelManager.RoomClear != null)
		{
			LevelManager.RoomClear(this, new RoomClearEventArgs(roomRootNode));
		}
	}

	public void LoadNextBackgroundAsync()
	{
		StartCoroutine(LoadNextBackground());
	}

	public void ActivateFirstRoom()
	{
		if (currentScreenRoot == null)
		{
			Debug.Log("Activate first room as soon as it loads...");
			activateFirstRoomOnLoad = true;
			return;
		}
		Debug.Log("Activating first room");
		Activator component = currentScreenRoot.GetComponent<Activator>();
		component.SetChildrenActive(true);
		if (!GameManager.gameStarted)
		{
			Debug.Log("Starting game");
			GameManager.Instance.StartGame();
		}
		ScreenManager componentInChildren = currentScreenRoot.GetComponentInChildren<ScreenManager>();
		componentInChildren.ActivateScreen();
	}

	public void Add(ScreenRootNodeData screenRootNodeData)
	{
		float num = Time.realtimeSinceStartup - _loadingStartTime;
		Debug.Log("Finished loading - " + loadingSceneName + " (" + screenRootNodeData.name + ") in " + num);
		if (currentScreenRoot == null)
		{
			currentScreenRoot = screenRootNodeData.gameObject;
			Camera componentInChildren = screenRootNodeData.GetComponentInChildren<Camera>();
			UnityEngine.Object.Destroy(componentInChildren.gameObject);
			DestroyPlaceholderObjects(screenRootNodeData.gameObject);
			if (activateFirstRoomOnLoad)
			{
				Debug.Log("Someone already told us to activate the first room...");
				ActivateFirstRoom();
			}
			return;
		}
		if (oldScreenRoot != null)
		{
		}
		oldScreenRoot = currentScreenRoot;
		currentScreenRoot = screenRootNodeData.gameObject;
		currentScreenRoot.name = loadingSceneName;
		MoveDirections moveDirections;
		if (currentRoomNumber == 0 || WingedBoots.IsActive || RocketBooster.IsActive)
		{
			moveDirections = MoveDirections.Right;
		}
		else
		{
			bool flag = lastMovedDirection != MoveDirections.Down && roomsMovedUp != 2;
			bool flag2 = lastMovedDirection != 0 && roomsMovedUp != 0;
			MoveDirections[] array = ((flag && flag2) ? new MoveDirections[3]
			{
				MoveDirections.Up,
				MoveDirections.Down,
				MoveDirections.Right
			} : ((!flag && flag2) ? new MoveDirections[2]
			{
				MoveDirections.Down,
				MoveDirections.Right
			} : ((!flag || flag2) ? new MoveDirections[1] { MoveDirections.Right } : new MoveDirections[2]
			{
				MoveDirections.Up,
				MoveDirections.Right
			})));
			moveDirections = array[UnityEngine.Random.Range(0, array.Length)];
		}
		bool flag3 = RoomColumnRelativeToBackground == 1;
		if (moveDirections == MoveDirections.Right && flag3)
		{
			LoadNextBackgroundAsync();
		}
		switch (moveDirections)
		{
		case MoveDirections.Right:
			roomsMovedRight++;
			break;
		case MoveDirections.Up:
			roomsMovedUp++;
			break;
		case MoveDirections.Down:
			roomsMovedUp--;
			break;
		}
		currentRoomNumber++;
		int num2 = roomsMovedRight / 3;
		float x = -15f * (float)roomsMovedRight + (float)(num2 * 2) * -7.5f;
		float y = 10f * (float)roomsMovedUp;
		Vector3 position = new Vector3(x, y, 0f);
		screenRootNodeData.gameObject.transform.position = position;
		ScreenManager componentInChildren2 = screenRootNodeData.gameObject.GetComponentInChildren<ScreenManager>();
		Camera componentInChildren3 = screenRootNodeData.gameObject.GetComponentInChildren<Camera>();
		Vector3 position2 = componentInChildren3.transform.position;
		UnityEngine.Object.Destroy(componentInChildren3.gameObject);
		DestroyPlaceholderObjects(screenRootNodeData.gameObject);
		lastMovedDirection = moveDirections;
		NextRoomEventArgs nextRoomEventArgs = new NextRoomEventArgs(this, screenRootNodeData, position2, componentInChildren2, moveDirections);
		OnMovingToNextRoom(nextRoomEventArgs);
		if (!nextRoomEventArgs.IsDelayed)
		{
			MoveToNextRoom(nextRoomEventArgs);
		}
		else
		{
			Debug.Log("Not moving on, args are delayed....");
		}
	}

	private void OnMovingToNextRoom(NextRoomEventArgs args)
	{
		IsTransitioning = true;
		if (LevelManager.MovingToNextRoom != null)
		{
			LevelManager.MovingToNextRoom(this, args);
		}
	}

	public void MoveToNextRoom(NextRoomEventArgs args)
	{
		if (WingedBoots.IsActive)
		{
			LinearMover linearMover = mainCameraParent.AddComponent<LinearMover>();
			linearMover.endPosition = args.CameraPosition;
		}
		else if (RocketBooster.IsActive)
		{
			RocketMover rocketMover = mainCameraParent.GetComponent<RocketMover>();
			if (mainCameraParent.GetComponent<RocketMover>() == null)
			{
				rocketMover = mainCameraParent.AddComponent<RocketMover>();
			}
			if (RocketBooster.RoomsRemaining == 1)
			{
				rocketMover.stopAtArrival = true;
			}
			rocketMover.RoomFinishedLoading(args);
		}
		else
		{
			MoverWithSpeed moverWithSpeed = mainCameraParent.AddComponent<MoverWithSpeed>();
			moverWithSpeed.destinationPoint = args.CameraPosition;
			moverWithSpeed.easeInDistance = 4f;
			moverWithSpeed.easeOutDistance = 5f;
		}
		roomClearDelay.Step();
		_movingArgs = args;
	}

	public float GetWaitTimeAfterRoomClear()
	{
		return roomClearDelay.value;
	}

	private void HandleCameraMoverMoveComplete(object sender, EventArgs e)
	{
		if (_movingArgs == null)
		{
			Debug.LogError("Someone's raising MoverWithSpeed.MoveComplete when levelManager doesn't have moving args...");
		}
		OnArrivedAtNextRoom(_movingArgs);
		_movingArgs = null;
	}

	private void OnArrivedAtNextRoom(NextRoomEventArgs args)
	{
		IsTransitioning = false;
		if (_movingArgs != null)
		{
			if (_movingArgs.ScreenManager != null)
			{
				_movingArgs.ScreenManager.ActivateScreen();
			}
			else
			{
				Debug.LogError("Tried to activate a Null ScreenManager");
			}
		}
		CurrentScreenManager = args.ScreenManager;
		if (LevelManager.ArrivedAtNextRoom != null)
		{
			LevelManager.ArrivedAtNextRoom(this, args);
		}
		m_CanGoToNextRoom = true;
		m_FirstRoomPassed = true;
		if (loadingSceneName.Contains("bonus") || loadingSceneName.Contains("boss"))
		{
			RoomTime = float.PositiveInfinity;
		}
		else
		{
			RoomTime = m_MaxRoomTime;
		}
	}

	private void DestroyPlaceholderObjects(GameObject rootGameObject)
	{
		PlaceholderObject[] array = rootGameObject.GetComponentsInChildren<PlaceholderObject>().ToArray();
		PlaceholderObject[] array2 = array;
		foreach (PlaceholderObject placeholderObject in array2)
		{
			if (placeholderObject != null)
			{
				UnityEngine.Object.Destroy(placeholderObject.gameObject);
			}
		}
	}

	private void OnBackgroundLoadComplete()
	{
		if (LevelManager.BackgroundLoadComplete != null)
		{
			LevelManager.BackgroundLoadComplete(this, new EventArgs());
		}
	}

	public void HideBackground()
	{
		if (newBackground != null)
		{
			newBackground.SetActive(false);
		}
		if (oldBackground != null)
		{
			oldBackground.SetActive(false);
		}
	}

	public void ShowBackground()
	{
		if (newBackground != null)
		{
			newBackground.SetActive(true);
		}
		if (oldBackground != null)
		{
			oldBackground.SetActive(true);
		}
	}

	public void AddBackground(BackgroundRootNodeData backgroundData)
	{
		Debug.Log("Finished loading background ");
		if (oldBackground != null)
		{
			UnityEngine.Object.Destroy(oldBackground);
		}
		oldBackground = newBackground;
		newBackground = backgroundData.gameObject;
		float x = -15f + (float)_loadedBackgroundCount * -60f;
		_loadedBackgroundCount++;
		Vector3 position = new Vector3(x, 10f, -8.5f);
		backgroundData.gameObject.transform.position = position;
		Camera componentInChildren = backgroundData.gameObject.GetComponentInChildren<Camera>();
		if (componentInChildren != null)
		{
			UnityEngine.Object.Destroy(componentInChildren.gameObject);
		}
		DestroyPlaceholderObjects(backgroundData.gameObject);
		OnBackgroundLoadComplete();
	}

	private IEnumerator GoToNextLevel()
	{
		if (RoomTime > 0f)
		{
			HealthBar.Instance.TriggerEvent(HealthBar.HealthBarEvent.ScreenCleared);
		}
		_loadingStartTime = Time.realtimeSinceStartup;
		string sceneName = GetNextRoomToLoad();
		if (sceneName != string.Empty)
		{
			Debug.Log("Starting load of scene - " + sceneName);
			AsyncOperation asyncResult = Application.LoadLevelAdditiveAsync(sceneName);
			loadingSceneName = sceneName;
			yield return asyncResult;
		}
		yield return null;
	}

	private IEnumerator LoadNextBackground()
	{
		if (!CurrentLevel.backgroundScenes.Any())
		{
			Debug.LogError("Unable to find any backgrounds to load... Are there any in this level?");
			yield break;
		}
		int randomBackgroundIndex = UnityEngine.Random.Range(0, CurrentLevel.backgroundScenes.Count);
		string background = CurrentLevel.backgroundScenes[randomBackgroundIndex];
		Debug.Log("Loading Background: " + background);
		yield return Application.LoadLevelAdditiveAsync(background);
	}

	private string GetNextRoomToLoad()
	{
		DiffIncreased = false;
		if (m_CurRoomIndex >= m_DifficultyRoomGroup[m_CurDifficultyIndex].m_RoomData.Count)
		{
			HealthBar.Instance.Die();
			return string.Empty;
		}
		Room room = m_DifficultyRoomGroup[m_CurDifficultyIndex].m_RoomData[m_CurRoomIndex].m_RoomGroup.DrawNextRandomRoom();
		m_CurRoomIndex++;
		return room.sceneName;
	}

	private string GetBossRoom()
	{
		DiffIncreased = false;
		if (m_CurRoomIndex >= m_DifficultyRoomGroup[m_CurDifficultyIndex].m_RoomData.Count)
		{
			HealthBar.Instance.Die();
			return string.Empty;
		}
		Room room = m_DifficultyRoomGroup[m_CurDifficultyIndex].m_RoomData[m_CurRoomIndex].m_RoomGroup.DrawNextRandomRoom();
		m_CurRoomIndex++;
		return room.sceneName;
	}

	private void OnDifficultyUp()
	{
		if (LevelManager.DifficultyUp != null)
		{
			LevelManager.DifficultyUp(this, null);
		}
	}

	private bool AlreadyPlayedTutorialsFor(Room room)
	{
		return !GetUnplayedTutorialsForRoom(room).Any();
	}

	private IEnumerable<EnemyTypes> GetUnplayedTutorialsForRoom(Room room)
	{
		HashSet<EnemyTypes> hashSet = new HashSet<EnemyTypes>();
		if (room.requiredTutorials != null)
		{
			foreach (EnemyTypes requiredTutorial in room.requiredTutorials)
			{
				bool value;
				if (!_tutorialPlayed.TryGetValue(requiredTutorial, out value))
				{
					string message = string.Format("Enemy type '{0}' in room '{1}' is not in the list of tutorial types levelManager knows about. BAD metadata.", requiredTutorial.ToString(), room.sceneName);
					Debug.LogError(message);
				}
				else if (!value)
				{
					hashSet.Add(requiredTutorial);
				}
			}
		}
		EnemyTypes[] array = hashSet.ToArray();
		Array.Sort(array, TutorialSortOrder);
		return array;
	}

	private void AdvanceToNextRound()
	{
		_currentRoundNumber++;
		if (_currentRoundNumber >= CurrentLevel.roundData.Count && _currentLevelNumber < levels.Length - 1)
		{
			AdvanceToNextLevel();
			_currentRoundNumber = 0;
		}
	}

	private void OnLevelChanged()
	{
		if (LevelManager.LevelChanged != null)
		{
			LevelManager.LevelChanged(this, new EventArgs());
		}
	}
}
