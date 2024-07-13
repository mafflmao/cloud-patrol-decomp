using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ElementOfTheDayChanger : MonoBehaviour
{
	private const string BedrockElementOverrideId = "override.elementOfTheDay";

	private static readonly ILogger _log = LogBuilder.Instance.GetLogger(typeof(ElementOfTheDayChanger), LogLevel.Debug);

	private Dictionary<string, Elements.Type> _dailyBonuses = new Dictionary<string, Elements.Type>();

	private static Elements.Type[] elementProgression = new Elements.Type[8]
	{
		Elements.Type.Air,
		Elements.Type.Earth,
		Elements.Type.Fire,
		Elements.Type.Life,
		Elements.Type.Magic,
		Elements.Type.Tech,
		Elements.Type.Undead,
		Elements.Type.Water
	};

	private static readonly DateTime StartDate = DateTime.Parse("Jan 01, 2000");

	public static string ElementOfTheDayNagText;

	public static bool UpdatedElementOfTheDay = false;

	public static bool QAOverrideInEffect = false;

	public static bool AllowElementOfTheDayChanges = true;

	private void Start()
	{
		ScreenSequenceController.SequenceComplete += EnableElementOfTheDayChanges;
		Bedrock.UserVarUpdatedFromCloud += HandleBedrockUserVarUpdatedFromCloud;
		UpdateElementOfTheDay();
	}

	private void HandleBedrockUserVarUpdatedFromCloud(object sender, EventArgs e)
	{
		UpdateElementOfTheDay();
	}

	private void OnDisable()
	{
		ScreenSequenceController.SequenceComplete -= EnableElementOfTheDayChanges;
		Bedrock.UserVarUpdatedFromCloud -= HandleBedrockUserVarUpdatedFromCloud;
	}

	private void OnApplicationPause(bool pause)
	{
		if (!pause)
		{
			UpdateElementOfTheDay();
		}
	}

	private void EnableElementOfTheDayChanges(object sender, EventArgs args)
	{
		AllowElementOfTheDayChanges = true;
		UpdateElementOfTheDay();
	}

	private void UpdateElementOfTheDay()
	{
		if (AllowElementOfTheDayChanges)
		{
			ImportOverridesFromSwrve();
			SetElementOfTheDay();
			NotifyNextElementOfTheDay();
		}
	}

	private void ImportOverridesFromSwrve()
	{
		Dictionary<string, string> resourceDictionary;
		if (!Bedrock.GetRemoteUserResources("override.elementOfTheDay", out resourceDictionary))
		{
			return;
		}
		foreach (KeyValuePair<string, string> item in resourceDictionary)
		{
			Elements.Type value;
			if (EnumUtils.TryParse<Elements.Type>(item.Value, out value))
			{
				_dailyBonuses.Add(item.Key, value);
			}
			else
			{
				_log.LogWarning("Unable to parse '" + item.Value + "' into valid element name.");
			}
		}
	}

	private void SetElementOfTheDay()
	{
		DateTime now = DateTime.Now;
		Elements.Type value;
		Elements.Type type;
		if (_dailyBonuses.TryGetValue(GetDateKey(now), out value) || _dailyBonuses.TryGetValue(GetDayOnlyKey(now), out value))
		{
			type = value;
		}
		else
		{
			Elements.Type elementForDay = GetElementForDay(DateTime.Now);
			type = elementForDay;
		}
		Elements.Type randomElement = Elements.GetRandomElement();
		if (randomElement != type && !QAOverrideInEffect)
		{
			UpdatedElementOfTheDay = true;
			ElementOfTheDayNagText = LocalizationManager.Instance.GetFormatString("FOOTER_EOTD_NAG", Elements.GetLocalizedName(type));
			randomElement = type;
		}
		else
		{
			ElementOfTheDayNagText = LocalizationManager.Instance.GetFormatString("FOOTER_EOTD_NAG", Elements.GetLocalizedName(randomElement));
		}
	}

	private void NotifyNextElementOfTheDay()
	{
	}

	public static Elements.Type GetElementForDay(DateTime day)
	{
		int num = Mathf.Abs(Mathf.RoundToInt((float)(day.Date - StartDate.Date).TotalDays));
		return elementProgression[num % elementProgression.Length];
	}

	public static Elements.Type GetNextElementInProgression()
	{
		return elementProgression.First();
	}

	private static string GetDateKey(DateTime dateTime)
	{
		return string.Format("{0}/{1}/{2}", dateTime.Year, dateTime.Month, dateTime.Day);
	}

	private static string GetDayOnlyKey(DateTime dateTime)
	{
		return string.Format("{0}/{1}", dateTime.Month, dateTime.Day);
	}
}
