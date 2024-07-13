using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

public class DebugSettingsUI : SingletonMonoBehaviour
{
	public const string BuildIdResourceName = "BuildId";

	private const int ScenesPerPage = 5;

	private const int IndentAmount = 15;

	public static bool forceMagicItemSpawn;

	public static bool preventChangeAfterSpawn;

	public static string magicItemToSpawn;

	public static bool forcePresentSpawn;

	public static PresentBoxRewardTypes? forcePresentType;

	public static bool forceSaleIcons;

	public static bool forceOneEnemyInBossRooms;

	public static int debugFPS;

	public static bool forceShowChallengeMedalAwards;

	public static bool forceShowSaleDialog;

	public static bool forceFailReadingPurchaseQueue;

	public static bool forceFailProductRetrieval;

	public static bool forceFailValidationTask;

	public static bool forceInvalidProductInResult;

	private static bool mForcePlaceholderChallengeData;

	private static bool? _showChallengeDebugUiElements;

	public Level[] levels;

	public TutorialData tutorialData;

	public GUISkin skin;

	public PowerupList powerups;

	public CharacterDataList characters;

	public int BuildNumber;

	public string BuildId;

	private bool _expandBountyOptions;

	private bool _expandSceneOptions;

	private bool _expandSaveGameOptions;

	private bool _expandEconomyOptions;

	private bool _expandGameplayOptions;

	private bool _expandSwrveOptions;

	private bool _expandMagicItemOptions;

	private bool _expandChallengesOptions;

	private bool _expandLoggingOptions;

	private int bounty0Override;

	private int bounty1Override;

	private int bounty2Override;

	private bool showSceneList;

	private int scenePage;

	private Vector2 scrollListPosition = Vector2.zero;

	private List<string> _overrideScenes = new List<string>();

	private List<string> _allScenes = new List<string>();

	private int _rank;

	private Elements.Type _elementOfTheDay;

	private List<string> _powerupValues = new List<string>();

	private PresentBoxRewardTypes?[] _presentOverrideValues = new PresentBoxRewardTypes?[4]
	{
		null,
		PresentBoxRewardTypes.Coins,
		PresentBoxRewardTypes.Gem,
		PresentBoxRewardTypes.MagicItem
	};

	private List<Elements.Type> _elementOfTheDayValues = new List<Elements.Type>(EnumUtils.GetValues<Elements.Type>());

	private SwrveUserData.PlayerType _playerType;

	private string _overrideSwrveId;

	private string[] _buildIdStrings;

	private bool _showRestartButton;

	public static bool BuildWithMinimumRooms
	{
		get
		{
			return false;
		}
	}

	public static bool BuildWithTriggerHappyOnly
	{
		get
		{
			return false;
		}
	}

	public static bool BuildWithGhostSwordsOnly
	{
		get
		{
			return false;
		}
	}

	public static bool ForcePlaceholderChallengeData
	{
		get
		{
			return mForcePlaceholderChallengeData;
		}
		set
		{
			mForcePlaceholderChallengeData = value;
		}
	}

	public static bool ShowChallengeDebugUiElements
	{
		get
		{
			if (!_showChallengeDebugUiElements.HasValue)
			{
				return false;
			}
			return _showChallengeDebugUiElements.Value;
		}
		set
		{
			_showChallengeDebugUiElements = value;
		}
	}

	public static DebugSettingsUI GetOrCreateInstance()
	{
		return SingletonMonoBehaviour.GetAutoGeneratedSingletonInstance<DebugSettingsUI>();
	}

	public void Start()
	{
		bounty0Override = BountyChooser.Instance.ActiveBounties[0].bountyData.Id;
		bounty1Override = BountyChooser.Instance.ActiveBounties[1].bountyData.Id;
		bounty2Override = BountyChooser.Instance.ActiveBounties[2].bountyData.Id;
		debugFPS = Application.targetFrameRate;
		HashSet<string> hashSet = new HashSet<string>();
		Level[] array = levels;
		foreach (Level level in array)
		{
			foreach (string allRoomSceneName in level.GetAllRoomSceneNames())
			{
				hashSet.Add(allRoomSceneName);
			}
		}
		foreach (string allRoomSceneName2 in tutorialData.GetAllRoomSceneNames())
		{
			hashSet.Add(allRoomSceneName2);
		}
		_allScenes.AddRange(hashSet);
		_allScenes.Sort();
		_overrideScenes.AddRange(StartGameSettings.Instance.sceneOverrides);
		_powerupValues.Add(null);
		_powerupValues.AddRange(powerups.powerups.Select((PowerupData item) => item.name));
		TextAsset textAsset = (TextAsset)Resources.Load("BuildId", typeof(TextAsset));
		if (textAsset != null)
		{
			_buildIdStrings = textAsset.text.Split(new string[1] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
		}
	}

	public void OnGUI()
	{
		GUI.skin = skin;
		GUILayout.BeginArea(new Rect(0f, 0f, Screen.width, Screen.height));
		scrollListPosition = GUILayout.BeginScrollView(scrollListPosition);
		if (_buildIdStrings != null)
		{
			string[] buildIdStrings = _buildIdStrings;
			foreach (string text in buildIdStrings)
			{
				GUILayout.Label(text, GUILayout.ExpandWidth(true));
			}
		}
		GUILayout.Label("Bedrock Version: " + Bedrock.GetBedrockVersionString(), GUILayout.ExpandWidth(true));
		GUILayout.Label("UserID: " + Bedrock.getDefaultOnlineId(), GUILayout.ExpandWidth(true));
		GUILayout.Label("Emergency Ongoing?: " + Bedrock.HasEmegencyMessage());
		_expandBountyOptions = GUILayout.Toggle(_expandBountyOptions, "Bounty Overrides", GUILayout.ExpandWidth(true));
		if (_expandBountyOptions)
		{
			BeginIndent();
			DrawBountiesSection();
			EndIndent();
		}
		_expandSceneOptions = GUILayout.Toggle(_expandSceneOptions, "Scene Overrides");
		if (_expandSceneOptions)
		{
			BeginIndent();
			DrawScenesSection();
			EndIndent();
		}
		_expandSaveGameOptions = GUILayout.Toggle(_expandSaveGameOptions, "SaveGame Options");
		if (_expandSaveGameOptions)
		{
			BeginIndent();
			DrawSaveGameSection();
			EndIndent();
		}
		_expandEconomyOptions = GUILayout.Toggle(_expandEconomyOptions, "Economy Options");
		if (_expandEconomyOptions)
		{
			BeginIndent();
			DrawEconomySection();
			EndIndent();
		}
		_expandGameplayOptions = GUILayout.Toggle(_expandGameplayOptions, "Gameplay Options");
		if (_expandGameplayOptions)
		{
			BeginIndent();
			DrawGameplaySection();
			EndIndent();
		}
		_expandSwrveOptions = GUILayout.Toggle(_expandSwrveOptions, "Swrve Options");
		if (_expandSwrveOptions)
		{
			BeginIndent();
			DrawSwrveSection();
			EndIndent();
		}
		_expandLoggingOptions = GUILayout.Toggle(_expandLoggingOptions, "Logging Options");
		if (_expandLoggingOptions)
		{
			BeginIndent();
			DrawLoggingSection();
			EndIndent();
		}
		_expandChallengesOptions = GUILayout.Toggle(_expandChallengesOptions, "Challenge Options");
		if (_expandChallengesOptions)
		{
			BeginIndent();
			DrawChallengesSection();
			EndIndent();
		}
		if (GUILayout.Button("Close Debug Menu"))
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
		GUILayout.EndScrollView();
		GUILayout.EndArea();
		GUI.skin = null;
	}

	private void BeginIndent()
	{
		GUILayout.BeginHorizontal();
		GUILayout.Space(15f);
		GUILayout.BeginVertical();
	}

	private void EndIndent()
	{
		GUILayout.EndVertical();
		GUILayout.EndHorizontal();
	}

	private void DrawEconomySection()
	{
		if (GUILayout.Button("Add 40 Gems"))
		{
		}
		if (GUILayout.Button("Add 5000 Coins"))
		{
		}
		GUILayout.BeginHorizontal();
		GUILayout.Label("Bonus: " + _elementOfTheDay);
		if (GUILayout.Button("Previous", GUILayout.ExpandWidth(false)))
		{
			_elementOfTheDay = GetPreviousElementOfTheDay(_elementOfTheDayValues, _elementOfTheDay);
			ElementOfTheDayChanger.QAOverrideInEffect = true;
		}
		if (GUILayout.Button("Next", GUILayout.ExpandWidth(false)))
		{
			_elementOfTheDay = GetNextElementOfTheDay(_elementOfTheDayValues, _elementOfTheDay);
			ElementOfTheDayChanger.QAOverrideInEffect = true;
		}
		GUILayout.EndHorizontal();
		forceSaleIcons = GUILayout.Toggle(forceSaleIcons, "Force Sale Icons");
		forceFailReadingPurchaseQueue = GUILayout.Toggle(forceFailReadingPurchaseQueue, "Force Fail Reading Purchase Queue");
		uint numberOfItems = 0u;
		if (Bedrock.GetInAppPurchasingStoredCompletedPurchaseCount(out numberOfItems))
		{
			GUILayout.Label("    Number of items currently in queue: " + numberOfItems);
		}
		else
		{
			GUILayout.Label("    Error Reading from item queue.");
		}
		forceFailProductRetrieval = GUILayout.Toggle(forceFailProductRetrieval, "Force Fail Product List Retrieval");
		forceFailValidationTask = GUILayout.Toggle(forceFailValidationTask, "Force Fail Validation Task");
		forceInvalidProductInResult = GUILayout.Toggle(forceInvalidProductInResult, "Force Invalid Product Data");
	}

	private void DrawGameplaySection()
	{
		forceOneEnemyInBossRooms = GUILayout.Toggle(forceOneEnemyInBossRooms, "Single enemy required for boss rooms");
		GameManager.debugMode = GUILayout.Toggle(GameManager.debugMode, "Show Statistics");
		GameManager.invincible = GUILayout.Toggle(GameManager.invincible, "God Mode");
		PlatformUtils.IsLowQualityPlatform = GUILayout.Toggle(PlatformUtils.IsLowQualityPlatform, "Is Low Quality Device");
		forcePresentSpawn = GUILayout.Toggle(forcePresentSpawn, "Always Spawn Presents");
		preventChangeAfterSpawn = GUILayout.Toggle(preventChangeAfterSpawn, "Prevent Magic Items From Changing After Spawn");
		GUILayout.BeginHorizontal();
		string text = ((!forcePresentType.HasValue) ? "No Override" : forcePresentType.Value.ToString());
		GUILayout.Label("Present: " + text);
		if (GUILayout.Button("Prev", GUILayout.ExpandWidth(false)))
		{
			forcePresentType = GetPreviousRewardType(_presentOverrideValues, forcePresentType);
		}
		if (GUILayout.Button("Next", GUILayout.ExpandWidth(false)))
		{
			forcePresentType = GetNextRewardType(_presentOverrideValues, forcePresentType);
		}
		GUILayout.EndHorizontal();
		debugFPS = Spinner("Frame Rate:", debugFPS);
		if (GUILayout.Button("Apply Frame Rate"))
		{
			Application.targetFrameRate = debugFPS;
		}
		forceMagicItemSpawn = GUILayout.Toggle(forceMagicItemSpawn, "Always Spawn Magic Items");
		GUILayout.BeginHorizontal();
		string text2 = ((!string.IsNullOrEmpty(magicItemToSpawn)) ? magicItemToSpawn.Substring(0, magicItemToSpawn.Length - "DATA".Length) : "No Override");
		GUILayout.Label("MagicItem: " + text2);
		if (GUILayout.Button("Prev", GUILayout.ExpandWidth(false)))
		{
			magicItemToSpawn = _powerupValues.ElementBefore(magicItemToSpawn);
		}
		if (GUILayout.Button("Next", GUILayout.ExpandWidth(false)))
		{
			magicItemToSpawn = _powerupValues.ElementAfter(magicItemToSpawn);
		}
		GUILayout.EndHorizontal();
	}

	private PresentBoxRewardTypes? GetPreviousRewardType(PresentBoxRewardTypes?[] rewardTypes, PresentBoxRewardTypes? current)
	{
		if (current.HasValue)
		{
			int num = Array.IndexOf(rewardTypes, current);
			if (num == 0)
			{
				return rewardTypes[rewardTypes.Length];
			}
			return rewardTypes[num - 1];
		}
		return rewardTypes[rewardTypes.Length - 1];
	}

	private PresentBoxRewardTypes? GetNextRewardType(PresentBoxRewardTypes?[] rewardTypes, PresentBoxRewardTypes? current)
	{
		if (current.HasValue)
		{
			int num = Array.IndexOf(rewardTypes, current);
			if (num == rewardTypes.Length - 1)
			{
				return rewardTypes[0];
			}
			return rewardTypes[num + 1];
		}
		return rewardTypes[1];
	}

	private Elements.Type GetPreviousElementOfTheDay(List<Elements.Type> elements, Elements.Type current)
	{
		int num = _elementOfTheDayValues.IndexOf(_elementOfTheDay);
		if (num == 0)
		{
			return _elementOfTheDayValues[_elementOfTheDayValues.Count - 1];
		}
		return _elementOfTheDayValues[num - 1];
	}

	private Elements.Type GetNextElementOfTheDay(List<Elements.Type> elements, Elements.Type current)
	{
		int num = _elementOfTheDayValues.IndexOf(_elementOfTheDay);
		if (num == _elementOfTheDayValues.Count - 1)
		{
			return _elementOfTheDayValues[0];
		}
		return _elementOfTheDayValues[num + 1];
	}

	private void DrawSwrveSection()
	{
		if (GUILayout.Button("Player Type: " + _playerType))
		{
			int num = (int)(_playerType + 1);
			if (num >= 6)
			{
				num = 0;
			}
			_playerType = (SwrveUserData.PlayerType)num;
		}
		if (Bedrock.Instance != null)
		{
			GUILayout.Label("Override SwrveID");
			Bedrock.Instance._swrveOverrideUsername = GUILayout.TextField(Bedrock.Instance._swrveOverrideUsername);
			string[] texts = new string[3]
			{
				Bedrock.brReportPriority.BR_NOTICE.ToString(),
				Bedrock.brReportPriority.BR_WARNING.ToString(),
				Bedrock.brReportPriority.BR_ERROR.ToString()
			};
			GUILayout.Label("Bedrock LogLevel:");
			Bedrock.Instance._logLevel = (Bedrock.brReportPriority)GUILayout.SelectionGrid((int)Bedrock.Instance._logLevel, texts, 3);
			Bedrock.Instance._useDebugSwrveAPI = GUILayout.Toggle(Bedrock.Instance._useDebugSwrveAPI, "Use Debug Swrve API");
		}
		if (GUILayout.Button("Restart Bedrock"))
		{
			Bedrock.Shutdown();
			Bedrock.StartUp();
		}
		if (GUILayout.Button("Download Swrve resources"))
		{
			Bedrock.AsyncRefreshUserResources();
		}
		forceShowSaleDialog = GUILayout.Toggle(forceShowSaleDialog, "Force Sale Dialog");
		if (GUILayout.Button("Check For Emergency Message"))
		{
			Bedrock.CheckForNewEmergencyMessage();
		}
	}

	private void DrawBountiesSection()
	{
		bounty0Override = Spinner("Bounty 0:", bounty0Override);
		bounty1Override = Spinner("Bounty 1:", bounty1Override);
		bounty2Override = Spinner("Bounty 2:", bounty2Override);
		if (GUILayout.Button("Set progress = Goal-1"))
		{
			for (int i = 0; i < 3; i++)
			{
				BountyChooser.Instance.ActiveBounties[i].SetProgressFromLoad(BountyChooser.Instance.ActiveBounties[i].bountyData.Goal - 1);
				if (BountyChooser.Instance.ActiveBounties[i].RememberProgress)
				{
					BountyChooser.Instance.SaveBountyProgress(i);
				}
			}
		}
		if (GUILayout.Button("Apply Bounties"))
		{
			ApplyBountyOverrides();
		}
	}

	private void DrawScenesSection()
	{
		string text = ((!showSceneList) ? "Show Scene List" : "Hide Scene List");
		if (GUILayout.Button(text))
		{
			showSceneList = !showSceneList;
		}
		if (showSceneList)
		{
			GUILayout.BeginHorizontal();
			if (GUILayout.Button("<"))
			{
				scenePage--;
			}
			GUILayout.Label("Page " + scenePage);
			if (GUILayout.Button(">"))
			{
				scenePage++;
			}
			GUILayout.EndHorizontal();
			for (int i = scenePage * 5; i < Math.Min(_allScenes.Count, (scenePage + 1) * 5); i++)
			{
				string text2 = _allScenes[i];
				GUILayout.BeginHorizontal();
				GUILayout.Label(i + ": " + text2);
				if (GUILayout.Button("Add", GUILayout.ExpandWidth(false)))
				{
					_overrideScenes.Add(text2);
				}
				GUILayout.EndHorizontal();
			}
		}
		GUILayout.Label("Scene Overrides:");
		GUILayout.BeginHorizontal();
		GUILayout.Space(15f);
		GUILayout.BeginVertical();
		for (int j = 0; j < _overrideScenes.Count; j++)
		{
			string text3 = _overrideScenes[j];
			GUILayout.BeginHorizontal();
			GUILayout.Label(j + " " + text3);
			if (GUILayout.Button("X", GUILayout.ExpandWidth(false)))
			{
				_overrideScenes.RemoveAt(j);
			}
			GUILayout.EndHorizontal();
		}
		GUILayout.EndVertical();
		GUILayout.EndHorizontal();
		if (GUILayout.Button("Apply Scene Overrides"))
		{
			ApplySceneOverrides();
		}
	}

	private void DrawChallengesSection()
	{
		ShowChallengeDebugUiElements = GUILayout.Toggle(ShowChallengeDebugUiElements, "Show Challenge Debug Elements (Persistent)");
		forceShowChallengeMedalAwards = GUILayout.Toggle(forceShowChallengeMedalAwards, "Award dummy medals at game over");
		bool forcePlaceholderChallengeData = ForcePlaceholderChallengeData;
		forcePlaceholderChallengeData = GUILayout.Toggle(forcePlaceholderChallengeData, "Use Placeholder Data (Requires Restart)");
		if (forcePlaceholderChallengeData != ForcePlaceholderChallengeData)
		{
			ForcePlaceholderChallengeData = forcePlaceholderChallengeData;
			_showRestartButton = true;
		}
		if (_showRestartButton && GUILayout.Button("QUIT"))
		{
			Application.Quit();
		}
		Bedrock.FacebookEnabled = GUILayout.Toggle(Bedrock.FacebookEnabled, "Facebook Enabled");
		if (GUILayout.Button("Dump Challenge Data To Console"))
		{
			StringBuilder stringBuilder = new StringBuilder();
			Debug.Log(stringBuilder.ToString());
		}
	}

	private void DrawLoggingSection()
	{
		if (GUILayout.Button("Log Debug Messages To XCode"))
		{
			LogLevelExtensions.ForceAllLogsEnabled = true;
		}
		if (GUILayout.Button("Start Logging"))
		{
			LogCapturer instance = LogCapturer.Instance;
			if (instance != null)
			{
			}
			Debug.Log("Started Logging... " + instance.name);
		}
		if (GUILayout.Button("Dump Log"))
		{
			string text = LogCapturer.Instance.Output.ToString();
			DateTime now = DateTime.Now;
			string text2 = string.Format("{0}_{1}_{2}-{3}_{4}_{5}.log", now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second);
			string text3 = Application.persistentDataPath + "/" + text2;
			File.WriteAllText(text3, text);
			Debug.Log("Dumped log (" + text.Length + " charachters) to " + text3);
		}
	}

	private void DrawSaveGameSection()
	{
		if (GUILayout.Button("Reset All Data"))
		{
		}
		if (GUILayout.Button("Reset Achievements"))
		{
			AchievementManager.Instance.ResetAll();
		}
		if (GUILayout.Button("Reset 'Whats New' Dialog State"))
		{
		}
		_rank = Spinner("Rank", _rank);
		if (GUILayout.Button("Apply Rank Change"))
		{
			ApplyRankChange();
		}
		RateAppDialog.ShowDialogOverride = GUILayout.Toggle(RateAppDialog.ShowDialogOverride, "Show Rate App Dialog");
		if (GUILayout.Button("Unlock All Skylanders (gems)"))
		{
			CharacterData[] allReleasedSkylanders = characters.GetAllReleasedSkylanders();
			foreach (CharacterData cd in allReleasedSkylanders)
			{
				CharacterUserData characterUserData = new CharacterUserData(cd);
				characterUserData.UnlockCharacter(40, CharacterUserData.ToyLink.None);
			}
		}
		if (GUILayout.Button("Unlock All Skylanders (toylink)"))
		{
			CharacterData[] allReleasedSkylanders2 = characters.GetAllReleasedSkylanders();
			foreach (CharacterData cd2 in allReleasedSkylanders2)
			{
				CharacterUserData characterUserData2 = new CharacterUserData(cd2);
				characterUserData2.UnlockCharacter(-1, CharacterUserData.ToyLink.Skylanders2011);
			}
		}
		if (GUILayout.Button("Upgrade All Skylanders"))
		{
		}
		if (GUILayout.Button("Unlock All Spells"))
		{
		}
		if (!GUILayout.Button("Push Current Save To Cloud"))
		{
		}
	}

	public static int Spinner(string label, int currentValue)
	{
		GUILayout.BeginHorizontal();
		if (label != null)
		{
			GUILayout.Label(label);
		}
		string s = GUILayout.TextField(currentValue.ToString());
		int result;
		if (int.TryParse(s, out result))
		{
			if (GUILayout.Button("+", GUILayout.ExpandWidth(false)))
			{
				result++;
			}
			if (GUILayout.Button("-", GUILayout.ExpandWidth(false)))
			{
				result--;
			}
			GUILayout.EndHorizontal();
			return result;
		}
		GUILayout.EndHorizontal();
		return currentValue;
	}

	private void ApplyBountyOverrides()
	{
		BountyChooser.Instance.SetBounty(0, BountyChooser.Instance.GetBountyDataForId(bounty0Override));
		BountyChooser.Instance.SetBounty(1, BountyChooser.Instance.GetBountyDataForId(bounty1Override));
		BountyChooser.Instance.SetBounty(2, BountyChooser.Instance.GetBountyDataForId(bounty2Override));
	}

	private void ApplySceneOverrides()
	{
		StartGameSettings.Instance.sceneOverrides = _overrideScenes.ToArray();
	}

	private void ApplyRankChange()
	{
	}
}