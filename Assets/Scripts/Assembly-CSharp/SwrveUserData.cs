using System;
using System.Collections.Generic;
using UnityEngine;

public class SwrveUserData : MonoBehaviour
{
	public enum PlayerType
	{
		Normal = 0,
		QA1 = 1,
		QA2 = 2,
		QA3 = 3,
		QA4 = 4,
		QA5 = 5,
		Total = 6
	}

	private static readonly ILogger _log = LogBuilder.Instance.GetLogger(typeof(SwrveUserData), LogLevel.Log);

	public static void UploadAllAttributes()
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("AppVersion", 1.8f.ToString());
		Bedrock.brKeyValueArray parameters = default(Bedrock.brKeyValueArray);
		parameters.size = dictionary.Count;
		parameters.pairs = new Bedrock.brKeyValuePair[parameters.size];
		int num = 0;
		foreach (KeyValuePair<string, string> item in dictionary)
		{
			parameters.pairs[num].key = item.Key;
			parameters.pairs[num].val = item.Value;
			num++;
			_log.LogDebug("key = {0} : value = {1} ", item.Key, item.Value);
		}
		Bedrock.AnalyticsSetCustomUserInformation(parameters);
	}

	private static int ConvertToUnixTime(DateTime time)
	{
		return (int)(time - TimeUtils.UnixEpoch).TotalSeconds;
	}

	public static int GetUnlockedSkylanderCount()
	{
		int num = 0;
		CharacterData[] allReleasedSkylanders = ElementDataManager.Instance.characterDataList.GetAllReleasedSkylanders();
		foreach (CharacterData cd in allReleasedSkylanders)
		{
			if (ElementDataManager.Instance.GetCharacterUserData(cd).IsUnlocked)
			{
				num++;
			}
		}
		return num;
	}

	public static int GetMagicItemsUnlockedCount()
	{
		return MagicItemManager.Instance.powerups.powerups.Count;
	}
}
