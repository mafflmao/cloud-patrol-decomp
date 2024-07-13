using System;
using System.Collections.Generic;
using UnityEngine;

public class LinkedContentManager : SingletonMonoBehaviour
{
	private const uint MAX_CONTENT_KEYS = 256u;

	private const uint MAX_INDIVIDUAL_NOTIFICATIONS = 1u;

	private static ILogger _log = LogBuilder.Instance.GetLogger(typeof(LinkedContentManager), LogLevel.Debug);

	private HashSet<string> _itemsNotifiedThisSession = new HashSet<string>();

	private bool _isWaitingForContentUpdate;

	private bool _hasContentBeenUpdatedSinceConnnectionStatusChange;

	public static LinkedContentManager Instance
	{
		get
		{
			return SingletonMonoBehaviour.GetSingletonInstance<LinkedContentManager>();
		}
	}

	protected override void AwakeOnce()
	{
		base.AwakeOnce();
		UnityEngine.Object.DontDestroyOnLoad(this);
	}

	private void OnEnable()
	{
		Bedrock.UnlockContentChanged += HandleUnlockContentChanged;
		Bedrock.CloudStorageConnected += HandleCloudStorageConnected;
		ActivateWatcher.ConnectionStatusChange += HandleActivateWatcherConnectionStatusChange;
	}

	private void OnDisable()
	{
		Bedrock.UnlockContentChanged -= HandleUnlockContentChanged;
		Bedrock.CloudStorageConnected -= HandleCloudStorageConnected;
		ActivateWatcher.ConnectionStatusChange -= HandleActivateWatcherConnectionStatusChange;
	}

	private void HandleCloudStorageConnected(object sender, EventArgs e)
	{
		_log.LogDebug("HandleCloudStorageConnected");
		if (Bedrock.isDeviceAnonymouslyLoggedOn())
		{
			_log.LogDebug("Anonymous user connected when cloud storage finished connecting. Ignoring.");
			return;
		}
		Bedrock.CloudConflictInfo userCacheVariablesCloudConflictInfo = Bedrock.GetUserCacheVariablesCloudConflictInfo();
		_log.LogDebug("Connection status is '{0}'.", userCacheVariablesCloudConflictInfo.connectionStatus);
		if (userCacheVariablesCloudConflictInfo.connectionStatus == Bedrock.brCloudStorageConnectionStatus.BR_CLOUDSTORAGE_CONNECTION_ONLINE)
		{
			if (userCacheVariablesCloudConflictInfo.fileConflictStatus == Bedrock.brCloudStorageFileConflictStatus.BR_CLOUDSTORAGE_FILECONFLICT_NONE)
			{
				_log.LogDebug("No file conflict, checking for new content.");
				TryUpdateContentAndDisplayNotification();
			}
			else
			{
				_log.LogDebug("File conflict status '{0}' might result in a pull. We don't want to check until it is resolved.", userCacheVariablesCloudConflictInfo.fileConflictStatus);
			}
		}
		else
		{
			_log.LogWarning("HandleCloudStorageConnected(...) event happened in offline state. Whaaa?");
		}
	}

	private void HandleUnlockContentChanged(object sender, EventArgs e)
	{
		_hasContentBeenUpdatedSinceConnnectionStatusChange = true;
		_log.LogDebug("HandleUnlockContentChanged");
		if (_isWaitingForContentUpdate)
		{
			_log.Log("Checking for unclaimed content.");
			CheckForUnclaimedContent();
		}
		else
		{
			_log.LogDebug("Wasn't waiting for unlocked content update. Ignoring event.");
		}
	}

	private void HandleActivateWatcherConnectionStatusChange(object sender, ConnectionStatusChangeEventArgs e)
	{
		_log.LogDebug("HandleActivateWatcherUserLoggedOn");
		_hasContentBeenUpdatedSinceConnnectionStatusChange = false;
		if (e.OldStatus.IsRegistered() != e.NewStatus.IsRegistered())
		{
			_log.LogDebug("Old connection status ({0}) is not same registration type as new connection status ({1}). Clearing list.", e.OldStatus, e.NewStatus);
			_itemsNotifiedThisSession.Clear();
			_isWaitingForContentUpdate = false;
		}
	}

	public void TryUpdateContentAndDisplayNotification()
	{
		_log.LogDebug("TryUpdateContentAndDisplayNotification");
		if (!_hasContentBeenUpdatedSinceConnnectionStatusChange)
		{
			_log.LogDebug("Content list has not been updated since last connection change. Starting new task.");
			StartUnlockedContentCacheUpdateTask();
		}
		else
		{
			_log.LogDebug("Content list has been updated since last connection change. Checking for claimed content with current cached values.");
			CheckForUnclaimedContent();
		}
	}

	private void StartUnlockedContentCacheUpdateTask()
	{
		_log.LogDebug("StartUnlockedContentCacheUpdateTask()");
		Bedrock.brUserConnectionStatus userConnectionStatus = Bedrock.getUserConnectionStatus();
		if (userConnectionStatus.IsOnline())
		{
			_log.LogDebug("Status is '{0}'. Starting content refresh.", userConnectionStatus);
			_isWaitingForContentUpdate = true;
			Bedrock.StartUnlockedContentCacheUpdateTask(Bedrock.brLobbyServerTier.BR_LOBBY_SERVER_FRANCHISE);
		}
		else
		{
			_log.LogDebug("Status is '{0}'. Skipping content refresh.", userConnectionStatus);
		}
	}

	private void CheckForUnclaimedContent()
	{
		_log.LogDebug("CheckForUnclaimedContent()");
		_isWaitingForContentUpdate = false;
		Bedrock.brContentUnlockInfo[] array = Bedrock.ListUnlockedContent(Bedrock.brLobbyServerTier.BR_LOBBY_SERVER_FRANCHISE, 256u);
		_log.LogDebug("Building list of claimable skylanders and magic items.");
		HashSet<string> hashSet = new HashSet<string>();
		HashSet<string> hashSet2 = new HashSet<string>();
		CharacterData[] allReleasedSkylanders = BountyChooser.Instance.allCharacters.GetAllReleasedSkylanders();
		List<PowerupData> powerups = BountyChooser.Instance.allPowerups.powerups;
		Bedrock.brContentUnlockInfo[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			Bedrock.brContentUnlockInfo brContentUnlockInfo = array2[i];
			_log.LogDebug("Examining {0}.{1}", brContentUnlockInfo.contentKey, brContentUnlockInfo.subType);
			CharacterData[] array3 = allReleasedSkylanders;
			foreach (CharacterData characterData in array3)
			{
				if (characterData.MatchesToyAndSubtype(brContentUnlockInfo.contentKey, brContentUnlockInfo.subType))
				{
					CharacterUserData characterUserData = new CharacterUserData(characterData);
					_log.LogDebug("Matches character '{0}'", characterData.charName);
					if (!characterUserData.IsUnlocked || (characterUserData.IsUnlocked && !characterUserData.IsToyLinked))
					{
						_log.LogDebug("Character is claimable!");
						hashSet.Add(characterData.charName);
					}
				}
			}
			foreach (PowerupData item in powerups)
			{
				if (item.MatchesToyAndSubtype(brContentUnlockInfo.contentKey, brContentUnlockInfo.subType))
				{
					_log.LogDebug("Matches magic item '{0}'", item.name);
					if (item.canLinkWithToy && (item.IsLocked || (!item.IsLocked && !item.IsToyLinked)))
					{
						_log.LogDebug("Powerup is claimable!");
						hashSet2.Add(item.LocalizedName);
					}
				}
			}
		}
		_log.LogDebug("Done processing content. Found {0} claimable skylander(s) and {1} claimable poweup(s).", hashSet.Count, hashSet2.Count);
		_log.LogDebug("Stripping repeated messages (from list of {0} items)...", _itemsNotifiedThisSession.Count);
		foreach (string item2 in _itemsNotifiedThisSession)
		{
			hashSet2.Remove(item2);
			hashSet.Remove(item2);
		}
		_log.LogDebug("Done stripping. Remaining: {0} claimable skylander(s) and {1} claimable poweup(s).", hashSet.Count, hashSet2.Count);
		_log.LogDebug("Recording notification triggers for session...");
		foreach (string item3 in hashSet2)
		{
			_itemsNotifiedThisSession.Add(item3);
		}
		foreach (string item4 in hashSet)
		{
			_itemsNotifiedThisSession.Add(item4);
		}
		_log.LogDebug("Now {0} entries in session list.", _itemsNotifiedThisSession.Count);
		if ((long)hashSet.Count > 1L)
		{
			_log.LogDebug("Raising multiple skylander message.");
			CollectionNotificationPanelSettings settings = new CollectionNotificationPanelSettings(LocalizationManager.Instance.GetString("CLAIM_CHARACTERS_NOTIFICATION"));
			NotificationPanel.Instance.Display(settings);
		}
		else
		{
			_log.LogDebug("Raising individual skylander available messages.");
			foreach (string item5 in hashSet)
			{
				string formatString = LocalizationManager.Instance.GetFormatString("CLAIM_CHARACTER_NOTIFICATION", item5);
				_log.LogDebug("Raising message '{0}", formatString);
				CollectionNotificationPanelSettings settings2 = new CollectionNotificationPanelSettings(formatString);
				NotificationPanel.Instance.Display(settings2);
			}
		}
		if ((long)hashSet2.Count > 1L)
		{
			_log.LogDebug("Raising combined magic item message.");
			CollectionNotificationPanelSettings settings3 = new CollectionNotificationPanelSettings(LocalizationManager.Instance.GetString("CLAIM_MAGIC_ITEMS_NOTIFICATION"));
			NotificationPanel.Instance.Display(settings3);
			return;
		}
		_log.LogDebug("Raising individual skylander available messages.");
		foreach (string item6 in hashSet2)
		{
			string formatString2 = LocalizationManager.Instance.GetFormatString("CLAIM_MAGIC_ITEM_NOTIFICATION", item6);
			_log.LogDebug("Raising message '{0}", formatString2);
			CollectionNotificationPanelSettings settings4 = new CollectionNotificationPanelSettings(formatString2);
			NotificationPanel.Instance.Display(settings4);
		}
	}

	private void OnApplicationPause(bool pause)
	{
		_log.LogDebug("OnApplicationPause({0})", pause);
		if (!pause)
		{
			_log.LogDebug("Resuming from suspend... Checking connection status.");
			Bedrock.brUserConnectionStatus userConnectionStatus = Bedrock.getUserConnectionStatus();
			switch (userConnectionStatus)
			{
			case Bedrock.brUserConnectionStatus.BR_LOGGED_IN_ANONYMOUSLY_ONLINE:
				TryUpdateContentAndDisplayNotification();
				break;
			case Bedrock.brUserConnectionStatus.BR_LOGGED_IN_REGISTERED_ONLINE:
				break;
			default:
				_log.LogDebug("Status is '{0}'. Skipping content refresh.", userConnectionStatus);
				break;
			}
		}
	}
}
