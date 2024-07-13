using System.Collections.Generic;
using UnityEngine;

public class AchievementManager : SingletonMonoBehaviour
{
	private enum AchievementState
	{
		Starting = 0,
		Login = 1,
		Updating = 2,
		UpdatingFromTitle = 3,
		UpSync = 4,
		DownSync = 5,
		Idle = 6
	}

	private const string AppRanEverStorageKey = "app.ran.ever";

	public const string KEY_PREFIX = "player.achievement.";

	private static readonly ILogger _log = LogBuilder.Instance.GetLogger(typeof(AchievementManager), LogLevel.Debug);

	public Achievements achievements;

	private Dictionary<string, Achievements.AchievementData> achievementData = new Dictionary<string, Achievements.AchievementData>();

	private AchievementState currentState;

	public bool autoSync;

	public static AchievementManager Instance
	{
		get
		{
			return SingletonMonoBehaviour.GetSingletonInstance<AchievementManager>();
		}
	}

	public bool IsLoggingIn
	{
		get
		{
			return currentState == AchievementState.Login;
		}
	}

	public bool Ready
	{
		get
		{
			currentState = AchievementState.Idle;
			return true;
		}
	}

	public bool Working
	{
		get
		{
			return currentState == AchievementState.Login || currentState == AchievementState.UpSync || currentState == AchievementState.DownSync || currentState == AchievementState.Updating;
		}
	}

	public bool HasShownErrorForFailedConnection { get; private set; }

	private void Start()
	{
		Load();
		Object.DontDestroyOnLoad(base.gameObject);
	}

	public void Connect()
	{
		_log.LogDebug("Connect()");
	}

	public void Load()
	{
		_log.LogDebug("Load()");
		Achievements.AchievementData[] array = achievements.achievements;
		foreach (Achievements.AchievementData achievementData in array)
		{
			Achievements.AchievementData achievementData2 = new Achievements.AchievementData();
			achievementData2.id = achievementData.id;
			achievementData2.name = achievementData.name;
			achievementData2.stepCount = achievementData.stepCount;
			achievementData2.LoadProgressFromPersistentStorage();
			this.achievementData[achievementData2.GameCenterIdentifier] = achievementData2;
		}
	}

	public void SyncAchievements()
	{
		_log.LogDebug("SyncAchievements()");
	}

	public void SetStep(string id, int step)
	{
		_log.LogDebug("SetStep({0}, {1})", id, step);
		Achievements.AchievementData value = null;
		if (achievementData.TryGetValue(id, out value))
		{
			if (value.step != step)
			{
				value.step = step;
				if (autoSync)
				{
					_log.LogDebug("autoSync enabled. Saving and syncing progress.");
					value.SaveProgressToPersistentStorage();
					value.NotDirty();
					if (value.step >= value.stepCount)
					{
						_log.Log("Awarding achievement. Step count ({0}) is now >= total step count ({1})", value.step, value.stepCount);
						SwrveEventsProgression.AchievementAwarded(value.name);
					}
				}
			}
			else
			{
				_log.LogDebug("Current step value ({0}) is the same as value to set. Skipping.", step);
			}
		}
		else
		{
			_log.LogError("Failed to get achievement data for '{0}'", id);
		}
	}

	public void IncrementStep(string id)
	{
		_log.LogDebug("IncrementStep({0})", id);
		IncrementStepBy(id, 1);
	}

	public void IncrementStepBy(string id, int increment)
	{
		_log.LogDebug("IncrementStepBy({0}, {1})", id, increment);
		Achievements.AchievementData value = null;
		if (achievementData.TryGetValue(id, out value))
		{
			if (value.step < value.stepCount)
			{
				value.step += increment;
				if (autoSync)
				{
					_log.LogDebug("autoSync enabled. Saving and syncing progress.");
					value.SaveProgressToPersistentStorage();
					value.NotDirty();
					if (value.step >= value.stepCount)
					{
						_log.Log("Awarding achievement. Step count ({0}) is now >= total step count ({1})", value.step, value.stepCount);
						SwrveEventsProgression.AchievementAwarded(value.name);
					}
					else
					{
						_log.LogDebug("Current progress ({0}) is still less than total step count ({1}). Not awarding achievement.", value.step, value.stepCount);
					}
				}
			}
			else
			{
				_log.LogDebug("Current step value ({0}) is not less than total step count ({1}), not updating.", value.step, value.stepCount);
			}
		}
		else
		{
			_log.LogError("Failed to get achievement data for '{0}'", id);
		}
	}

	public void ResetAll()
	{
		_log.LogWarning("Resetting all achievements");
		foreach (Achievements.AchievementData value in achievementData.Values)
		{
			value.step = 0;
			value.SaveProgressToPersistentStorage();
		}
	}

	public Achievements.AchievementData GetAchievement(string id)
	{
		return achievementData[id];
	}

	private void HandlePlayerAuthenticated(bool success)
	{
		_log.LogDebug("HandlePlayerAuthenticated({0})", success);
		if (success)
		{
			Bedrock.AnalyticsLogEvent("Metagame.Social.GameCenterConnected");
			_log.Log("Player authenticated successfully. Loading achievements.");
			currentState = AchievementState.DownSync;
		}
		else
		{
			_log.Log("Player failed to authenticate.");
			currentState = AchievementState.Idle;
		}
	}

	private void AchivementReportProgressCompleteCallback(bool wasSuccessful)
	{
		if (wasSuccessful)
		{
			_log.Log("Updated achievement progress successfully.");
		}
		else
		{
			_log.LogError("Failed to update achievement progress.");
		}
	}

	public void TrackCoinsSpent(int spentAmount)
	{
		autoSync = true;
		IncrementStepBy(Achievements.CoinsSpend, spentAmount);
		autoSync = false;
	}
}
